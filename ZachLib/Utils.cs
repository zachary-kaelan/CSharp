using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper.Expressions;
using CsvHelper;

namespace ZachLib
{
    public static class Utils
    {
        static Utils()
        {
            CosturaUtility.Initialize();
        }

        public static readonly CompareInfo COMPARE_INFO = CultureInfo.CurrentCulture.CompareInfo;
        public static readonly CompareOptions IGNORE_CASE_AND_SYMBOLS = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase;

        public static readonly Random RANDOM = new Random();

        private const string ALPHA_NUMERIC_LOWER = "abcdefghijklmnopqrstuvwxyz1234567890";
        private const string ALPHA_NUMERIC_MULTI = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + ALPHA_NUMERIC_LOWER;

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

        public static readonly Configuration csvConfig = new Configuration()
        {
            PrepareHeaderForMatch = h => ZachRGX.SYMBOLS.Replace(h, "").ToLower()
        };

        public static readonly Dictionary<string, System.Drawing.Imaging.ImageCodecInfo> Encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
            .ToDictionary(e => e.FilenameExtension, e => e);

        public static Dictionary<string, string> LoadDictionary(string path, string delimiter = " :=: ", bool reverse = false)
        {
            string[] delimit = new string[] { delimiter };
            return !reverse ? File.ReadAllLines(path)
                .Select(l => l.Split(delimit, StringSplitOptions.None))
                .ToDictionary(l => l[0], l => l[1]) :
                File.ReadAllLines(path)
                .Select(l => l.Split(delimit, StringSplitOptions.None))
                .ToDictionary(l => l[1], l => l[0]);
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
