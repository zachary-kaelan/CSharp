namespace TaskBarApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class StringExtensions
    {
        private static readonly Dictionary<char, string> Replacements = new Dictionary<char, string>();

        static StringExtensions()
        {
            Replacements['\r'] = "";
            Replacements['’'] = "'";
            Replacements['‐'] = "-";
            Replacements['–'] = "-";
            Replacements['‘'] = "'";
            Replacements['”'] = "\"";
            Replacements['“'] = "\"";
            Replacements['…'] = "...";
            Replacements['\x00a3'] = "GBP";
            Replacements['•'] = "*";
            Replacements[' '] = " ";
            Replacements['\x00e9'] = "e";
            Replacements['\x00ef'] = "i";
            Replacements['\x00b4'] = "'";
            Replacements['—'] = "-";
            Replacements['\x00b7'] = "*";
            Replacements['„'] = "\"";
            Replacements['€'] = "EUR";
            Replacements['\x00ae'] = "(R)";
            Replacements['\x00b9'] = "(1)";
            Replacements['\x00ab'] = "\"";
            Replacements['\x00e8'] = "e";
            Replacements['\x00e1'] = "a";
            Replacements['™'] = "TM";
            Replacements['\x00bb'] = "\"";
            Replacements['\x00e7'] = "c";
            Replacements['\x00bd'] = "1/2";
            Replacements['\x00ad'] = "-";
            Replacements['\x00b0'] = " degrees ";
            Replacements['\x00e4'] = "a";
            Replacements['\x00c9'] = "E";
            Replacements['‚'] = ",";
            Replacements['\x00fc'] = "u";
            Replacements['\x00ed'] = "i";
            Replacements['\x00eb'] = "e";
            Replacements['\x00f6'] = "o";
            Replacements['\x00e0'] = "a";
            Replacements['\x00ac'] = " ";
            Replacements['\x00f3'] = "o";
            Replacements['\x00e2'] = "a";
            Replacements['\x00f1'] = "n";
            Replacements['\x00f4'] = "o";
            Replacements['\x00a8'] = "";
            Replacements['\x00e5'] = "a";
            Replacements['\x00e3'] = "a";
            Replacements['ˆ'] = "";
            Replacements['\x00a9'] = "(c)";
            Replacements['\x00c4'] = "A";
            Replacements['\x00cf'] = "I";
            Replacements['\x00f2'] = "o";
            Replacements['\x00ea'] = "e";
            Replacements['\x00ee'] = "i";
            Replacements['\x00dc'] = "U";
            Replacements['\x00c1'] = "A";
            Replacements['\x00df'] = "ss";
            Replacements['\x00be'] = "3/4";
            Replacements['\x00c8'] = "E";
            Replacements['\x00bc'] = "1/4";
            Replacements['†'] = "+";
            Replacements['\x00b3'] = "'";
            Replacements['\x00b2'] = "'";
            Replacements['\x00d8'] = "O";
            Replacements['\x00b8'] = ",";
            Replacements['\x00cb'] = "E";
            Replacements['\x00fa'] = "u";
            Replacements['\x00d6'] = "O";
            Replacements['\x00fb'] = "u";
            Replacements['\x00da'] = "U";
            Replacements['Œ'] = "Oe";
            Replacements['\x00ba'] = "?";
            Replacements['‰'] = "0/00";
            Replacements['\x00c5'] = "A";
            Replacements['\x00f8'] = "o";
            Replacements['˜'] = "~";
            Replacements['\x00e6'] = "ae";
            Replacements['\x00f9'] = "u";
            Replacements['‹'] = "<";
            Replacements['\x00b1'] = "+/-";
        }

        private static string Asciify(char x)
        {
            if (!Replacements.ContainsKey(x))
            {
                return x.ToString();
            }
            return Replacements[x];
        }

        public static string Asciify(this string s) => 
            string.Join(string.Empty, (from c in s select Asciify(c)).ToArray<string>());

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly StringExtensions.<>c <>9 = new StringExtensions.<>c();
            public static Func<char, string> <>9__1_0;

            internal string <Asciify>b__1_0(char c) => 
                StringExtensions.Asciify(c);
        }
    }
}

