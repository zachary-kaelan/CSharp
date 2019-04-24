using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jil;
using CsvHelper;
using CsvHelper.Configuration;

namespace ZachLib
{
    public static class IOExtensions
    {
        public static void TryWriteLineAsync(this StreamWriter streamWriter, string line, int lowBound = 5)
        {
            int upperBound = lowBound * 3;
            while (true)
            {
                try
                {
                    //Console.WriteLine(line);
                    streamWriter.WriteLineAsync(line);
                    break;
                }
                catch
                {
                    Thread.Sleep(Utils.RANDOM.Next(lowBound, upperBound));
                }
            }
        }

        #region SaveAs
        private static readonly Type[] NON_PRIMITIVE_STRINGABLE = new Type[]
        {
            typeof(string),
            typeof(TimeSpan),
            typeof(DateTime),
            typeof(DateTimeOffset)
        };
        internal static bool IsString(this Object obj, out string str)
        {
            str = null;
            Type type = obj.GetType();

            if (/*NON_PRIMITIVE_STRINGABLE.Contains(type) || */type.IsPrimitive || type.GetMethod("ToString").DeclaringType == type)
            {
                str = obj.ToString();
                return true;
            }

            return false;
        }

        public static void SaveAs(this Object obj, string path)
        {
            obj.SaveAs(path, Options.ExcludeNullsIncludeInherited);
        }

        public static void SaveAs(this Object obj, string path, Options opts)
        {
            if (obj != null)
                File.WriteAllText(path, obj.IsString(out string str) ? str : JSON.Serialize(obj, opts));
        }

        public static void SaveAs(this Object obj, string path, Encoding encoding)
        {
            obj.SaveAs(path, Options.ExcludeNullsIncludeInherited, encoding);
        }

        public static void SaveAs(this Object obj, string path, Options opts, Encoding encoding)
        {
            if (obj != null)
                File.WriteAllText(path, obj.IsString(out string str) ? str : JSON.Serialize(obj, opts), encoding);
        }

        /*public static void SaveAs(this Object obj, string path, Formatting format)
        {
            if (obj != null)
                File.WriteAllText(path, JsonConvert.SerializeObject(obj, format));
        }*/

        private const string FILE_DIVISOR = "\r\n\r\n\t\t~~~\t\t\r\n\r\n";
        public static void SaveDividedAs<T>(this IEnumerable<T> objects, string path, string fileDivisor = FILE_DIVISOR)
        {
            objects.SaveDividedAs(path, Options.Default, fileDivisor);
        }

        public static void SaveDividedAs<T>(this IEnumerable<T> objects, string path, Options opts, string fileDivisor = FILE_DIVISOR)
        {
            File.WriteAllText(
                path,
                String.Join(
                    fileDivisor,
                    objects.Where(o => o != null).Select(
                        o => o.IsString(out string str) ? str : JSON.Serialize(o, opts)
                    )
                )
            );
        }

        /*public static void SaveDividedAs<T>(this IEnumerable<T> objects, string path, Formatting formatting, string fileDivisor = FILE_DIVISOR)
        {
            File.WriteAllText(
                path,
                String.Join(
                    fileDivisor,
                    objects.Where(o => o != null).Select(
                        o => JsonConvert.SerializeObject(o, formatting)
                    )
                )
            );
        }*/

        public static void SaveCSV<T>(this IEnumerable<T> objects, string path)
        {
            objects.SaveCSV(path, false);
        }

        public static void SaveCSV<T>(this IEnumerable<T> objects, string path, Encoding encoding)
        {
            objects.SaveCSV(path, encoding, false);
        }

        public static void SaveCSV<T>(this IEnumerable<T> objects, string path, bool append)
        {
            StreamWriter sr = new StreamWriter(path, append);
            CsvWriter cr = new CsvWriter(sr);
            cr.Configuration.HasHeaderRecord = !append;
            cr.WriteRecords<T>(objects);
            cr.Flush();
            sr.Flush();
            cr.Dispose();
            cr = null;
            sr.Close();
            sr = null;
        }

        public static void SaveCSV<T>(this IEnumerable<T> objects, string path, Encoding encoding, bool append)
        {
            StreamWriter sr = new StreamWriter(path, append, encoding);
            CsvWriter cr = new CsvWriter(sr, new Configuration() { Encoding = encoding });
            cr.Configuration.HasHeaderRecord = !append;
            cr.WriteRecords<T>(objects);
            cr.Flush();
            sr.Flush();
            cr.Dispose();
            cr = null;
            sr.Close();
            sr = null;
        }
        #endregion

        public static void SaveDictAs<K,V>(this IEnumerable<KeyValuePair<K, V>> dict, string path, string separator = " :=: ")
        {
            if (dict != null)
                File.WriteAllLines(
                    path, dict.Select(
                        kv => kv.Key.ToString() + separator + kv.Value.ToString()
                    )
                );
        }

        public static long GetTotalSize(this DirectoryInfo directory) => directory.GetFiles().Sum(f => f.Length);
    }
}
