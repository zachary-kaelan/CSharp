using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper.Expressions;
using CsvHelper;
using Jil;
using ZachLib.Logging;

namespace ZachLib
{
    public static class Utils
    {
        static Utils()
        {
            csvConfig = new Configuration()
            {
                PrepareHeaderForMatch = (h, i) => ZachRGX.SYMBOLS.Replace(h, "").ToLower(),
                HeaderValidated = null,
                MissingFieldFound = null
            };
        }

        public static DateTime Now { get => _now.Value; }
        private static readonly Lazy<DateTime> _now = new Lazy<DateTime>(() => DateTime.Now);

        public static void DoNothing()
        {

        }

        public static DateTime ConvertUnixTimestamp(long timestamp)
        {
            return DateTimeExtensions.UNIX_START_DATE.AddSeconds(timestamp).ToLocalTime();
        }

        public static readonly CompareInfo COMPARE_INFO = CultureInfo.CurrentCulture.CompareInfo;
        public static readonly CompareOptions IGNORE_CASE_AND_SYMBOLS = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase;

        public static readonly Random RANDOM = new Random();

        private const string ALPHA_NUMERIC_LOWER = "abcdefghijklmnopqrstuvwxyz1234567890";
        private const string ALPHA_NUMERIC_MULTI = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + ALPHA_NUMERIC_LOWER;        

        #region LoadCSV
        public static T[] LoadCSV<T>(string path, Configuration config, Encoding encoding)
        {
            T[] records = null;
            CsvReader cr = null;
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(file, encoding))
                {
                    if (config != null)
                        config.Encoding = encoding;
                    try
                    {
                        cr = config == null ? new CsvReader(sr) : new CsvReader(sr, config);
                        records = cr.GetRecords<T>().ToArray();
                    }
                    catch
                    {
                        if (config == null)
                            config = new Configuration()
                            {
                                Delimiter = "\t"
                            };
                        else
                            config.Delimiter = "\t";
                        cr = new CsvReader(sr, config);
                    }
                    cr.Dispose();
                    cr = null;
                }
            }
            return records;
        }

        public static T[] LoadCSV<T>(string path, Encoding encoding)
        {
            return LoadCSV<T>(path, csvConfig, encoding);
        }

        public static T[] LoadCSV<T>(string path, Configuration config)
        {
            StreamReader sr = new StreamReader(path);
            CsvReader cr = config == null ? new CsvReader(sr) : new CsvReader(sr, config);
            T[] records = cr.GetRecords<T>().ToArray();
            cr.Dispose();
            cr = null;
            sr.Close();
            sr = null;
            return records;
        }

        public static T[] LoadCSV<T>(string path)
        {
            return LoadCSV<T>(path, csvConfig);
        }
        #endregion

        #region LoadJSON
        public static T LoadJSON<T>(string path, Options opts)
        {
            return JSON.Deserialize<T>(File.ReadAllText(path), opts);
        }

        public static T LoadJSON<T>(string path)
        {
            return LoadJSON<T>(path, Options.ExcludeNullsIncludeInherited);
        }

        public static T LoadJSON<T>(string path, Options opts, Encoding encoding)
        {
            return JSON.Deserialize<T>(File.ReadAllText(path, encoding), opts);
        }

        public static T LoadJSON<T>(string path, Encoding encoding)
        {
            return LoadJSON<T>(path, Options.ExcludeNullsIncludeInherited, encoding);
        }
        #endregion

        public static Dictionary<string, string> LoadCSVDictionary(string path)
        {
            return LoadCSV<CSVKeyValuePair<string, string>>(path).ToDictionary();
        }

        public static string GetRandomString(int length)
        {
            char[] str = new char[length];

            for (int i = 0; i < length; ++i)
            {
                str[i] = ALPHA_NUMERIC_MULTI[RANDOM.Next(62)];
            }

            return new string(str);
        }

        public static string GetRandomString(int length, bool multicase)
        {
            if (multicase)
                return GetRandomString(length);
            else
            {
                char[] str = new char[length];

                for (int i = 0; i < length; ++i)
                {
                    str[i] = ALPHA_NUMERIC_LOWER[RANDOM.Next(36)];
                }

                return new string(str);
            }
        }

        public static Configuration csvConfig { get; private set; } 

        public static readonly Dictionary<string, System.Drawing.Imaging.ImageCodecInfo> Encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
            .ToDictionary(e => e.FilenameExtension, e => e);

        public static Dictionary<string, string> LoadDictionary(string path, string delimiter = " :=: ", bool reverse = false)
        {
            string[] delimit = new string[] { delimiter };
            var lines = File.ReadAllLines(path).Where(
                l => !String.IsNullOrWhiteSpace(l)
            ).Select(
                l => l.Split(delimit, StringSplitOptions.None)
            );
            try
            {
                return !reverse ?
                    lines.ToDictionary(l => l.First().Trim(), l => l.Length == 1 ? "" : l.Last()) :
                    lines.ToDictionary(l => l[1].Trim(), l => l[0].Trim());
            }
            catch (ArgumentException e)
            {
                var duplicateKeys = lines.GroupBy(
                    l => l.First(),
                    l => l.Length == 1 ? "" : l.Last()
                ).Where(g => g.Count() > 1);
                LogManager.Enqueue(
                    "ZachLib",
                    "LoadDictionaryError.txt",
                    new object[]
                    {
                        "LoadDictionary found duplicate keys",
                        duplicateKeys.Select(g => g.Key).ToArrayString()
                    },
                    duplicateKeys,
                    e
                );
                throw e;
            }
        }

        public static Dictionary<string, int> LoadIntDictionary(string path, string delimiter = " :=: ")
        {
            string[] delimit = new string[] { delimiter };
            var lines = File.ReadAllLines(path).Where(
                l => !String.IsNullOrWhiteSpace(l)
            ).Select(
                l => l.Split(delimit, StringSplitOptions.None)
            );
            try
            {
                return lines.ToDictionary(l => l.First().Trim(), l => l.Length == 1 ? 0 : Convert.ToInt32(l.Last()));
            }
            catch (ArgumentException e)
            {
                var duplicateKeys = lines.GroupBy(
                    l => l.First(),
                    l => l.Length == 1 ? "" : l.Last()
                ).Where(g => g.Count() > 1);
                LogManager.Enqueue(
                    "ZachLib",
                    "LoadDictionaryError.txt",
                    new object[]
                    {
                        "LoadDictionary found duplicate keys",
                        duplicateKeys.Select(g => g.Key).ToArrayString()
                    },
                    duplicateKeys,
                    e
                );
                throw e;
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> LoadKeyValues(string path, string delimiter = " :=: ", bool reverse = false)
        {
            string[] delimit = new string[] { delimiter };
            var lines = File.ReadAllLines(path).Where(
                l => !String.IsNullOrWhiteSpace(l)
            ).Select(
                l => l.Split(delimit, StringSplitOptions.None)
            );
            return !reverse ?
                    lines.Select(l => new KeyValuePair<string, string>(l.First().Trim(), l.Length == 1 ? "" : l.Last())) :
                    lines.Select(l => new KeyValuePair<string, string>(l[1].Trim(), l[0].Trim()));
        }

        public static Dictionary<string, string> LoadDictionary(string path, Encoding encoding, string delimiter = " :=: ", bool reverse = false)
        {
            string[] delimit = new string[] { delimiter };
            var lines = File.ReadAllLines(path, encoding).Where(
                l => !String.IsNullOrWhiteSpace(l)
            ).Select(
                l => l.Split(delimit, StringSplitOptions.None)
            );
            return !reverse ?
                lines.ToDictionary(l => l[0].Trim(), l => l[1].Trim()) :
                lines.ToDictionary(l => l[1].Trim(), l => l[0].Trim());
        }

        public static Dictionary<int, double> LoadIntDblDictionary(string path, string delimiter = " :=: ", bool reverse = false)
        {
            string[] delimit = new string[] { delimiter };
            var lines = File.ReadAllLines(path).Where(
                l => !String.IsNullOrWhiteSpace(l)
            ).Select(
                l => l.Split(delimit, StringSplitOptions.None)
            );
            return !reverse ?
                lines.ToDictionary(l => Convert.ToInt32(l[0].Trim()), l => Convert.ToDouble(l[1].Trim())) :
                lines.ToDictionary(l => Convert.ToInt32(l[1].Trim()), l => Convert.ToDouble(l[0].Trim()));
        }

        public static void SaveDictionary(Dictionary<string, string> dict, string path, string separator = " :=: ")
        {
            File.WriteAllLines(
                path, dict.Select(
                    kv => kv.Key + separator + kv.Value
                )
            );
        }
    }
}
