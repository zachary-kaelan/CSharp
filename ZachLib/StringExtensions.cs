using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    public static class StringExtensions
    {
        public static string ReplaceAll(this string str, Dictionary<string, string> dict)
        {
            string[] keys = dict.Keys.ToArray();
            foreach (string key in keys)
            {
                str.Replace(key, dict[key]);
            }
            return str;
        }

        private static readonly char[] VOWELS = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        public static int CountSyllables(this string str)
        {
            string[] phonemes = str.Split(
                VOWELS,
                StringSplitOptions.RemoveEmptyEntries
            );
            int syllables = phonemes.Length - 1;

            if (str.EndsWith("le"))
                syllables += 1;
            else if (str.EndsWith("ed") && phonemes[1].Length == 1)
                syllables -= 1;

            phonemes = null;
            return syllables;
        }

        public static string TitleCapitalization(this string str)
        {
            return String.Join(
                " ", str.Split(' ').Select(
                    s => new string(
                        s.ToUpper().Take(1).Concat(
                            s.ToLower().Skip(1)
                        ).ToArray()
                    )
                )
            );
        }
    }

    public static class ExtensionToStrings
    {
        public static string ToErrString(this Exception e, int tabCount = 0)
        {
            StringBuilder sb = new StringBuilder("");

            sb.Append("MESSAGE:\t\t");
            if (!String.IsNullOrWhiteSpace(e.Message))
                sb.AppendLine(e.Message);
            else if (e.InnerException != null && !String.IsNullOrWhiteSpace(e.InnerException.Message))
                sb.AppendLine(e.InnerException.Message);
            else
                sb.AppendLine("None.");

            sb.Append(new string('\t', tabCount));
            sb.Append("SOURCE:\t\t");
            sb.AppendLine(e.Source);

            sb.Append(new string('\t', tabCount));
            sb.Append("STACKTRACE:\t");
            sb.AppendLine(e.StackTrace.Replace("\n", "\n" + new string('\t', tabCount + 3)));

            sb.Append(new string('\t', tabCount));
            sb.Append("TARGETSITE:\t");
            sb.AppendLine(e.TargetSite.Name);

            return sb.ToString();
        }
    }
}
