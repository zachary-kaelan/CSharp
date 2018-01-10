using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TaskBarApp
{
	public static class StringExtensions
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly StringExtensions.<>c <>9 = new StringExtensions.<>c();

			public static Func<char, string> <>9__1_0;

			internal string <Asciify>b__1_0(char c)
			{
				return StringExtensions.Asciify(c);
			}
		}

		private static readonly Dictionary<char, string> Replacements;

		public static string Asciify(this string s)
		{
			string arg_2F_0 = string.Empty;
			Func<char, string> arg_25_1;
			if ((arg_25_1 = StringExtensions.<>c.<>9__1_0) == null)
			{
				arg_25_1 = (StringExtensions.<>c.<>9__1_0 = new Func<char, string>(StringExtensions.<>c.<>9.<Asciify>b__1_0));
			}
			return string.Join(arg_2F_0, s.Select(arg_25_1).ToArray<string>());
		}

		private static string Asciify(char x)
		{
			if (!StringExtensions.Replacements.ContainsKey(x))
			{
				return x.ToString();
			}
			return StringExtensions.Replacements[x];
		}

		static StringExtensions()
		{
			StringExtensions.Replacements = new Dictionary<char, string>();
			StringExtensions.Replacements['\r'] = "";
			StringExtensions.Replacements['’'] = "'";
			StringExtensions.Replacements['‐'] = "-";
			StringExtensions.Replacements['–'] = "-";
			StringExtensions.Replacements['‘'] = "'";
			StringExtensions.Replacements['”'] = "\"";
			StringExtensions.Replacements['“'] = "\"";
			StringExtensions.Replacements['…'] = "...";
			StringExtensions.Replacements['£'] = "GBP";
			StringExtensions.Replacements['•'] = "*";
			StringExtensions.Replacements[' '] = " ";
			StringExtensions.Replacements['é'] = "e";
			StringExtensions.Replacements['ï'] = "i";
			StringExtensions.Replacements['´'] = "'";
			StringExtensions.Replacements['—'] = "-";
			StringExtensions.Replacements['·'] = "*";
			StringExtensions.Replacements['„'] = "\"";
			StringExtensions.Replacements['€'] = "EUR";
			StringExtensions.Replacements['®'] = "(R)";
			StringExtensions.Replacements['¹'] = "(1)";
			StringExtensions.Replacements['«'] = "\"";
			StringExtensions.Replacements['è'] = "e";
			StringExtensions.Replacements['á'] = "a";
			StringExtensions.Replacements['™'] = "TM";
			StringExtensions.Replacements['»'] = "\"";
			StringExtensions.Replacements['ç'] = "c";
			StringExtensions.Replacements['½'] = "1/2";
			StringExtensions.Replacements['­'] = "-";
			StringExtensions.Replacements['°'] = " degrees ";
			StringExtensions.Replacements['ä'] = "a";
			StringExtensions.Replacements['É'] = "E";
			StringExtensions.Replacements['‚'] = ",";
			StringExtensions.Replacements['ü'] = "u";
			StringExtensions.Replacements['í'] = "i";
			StringExtensions.Replacements['ë'] = "e";
			StringExtensions.Replacements['ö'] = "o";
			StringExtensions.Replacements['à'] = "a";
			StringExtensions.Replacements['¬'] = " ";
			StringExtensions.Replacements['ó'] = "o";
			StringExtensions.Replacements['â'] = "a";
			StringExtensions.Replacements['ñ'] = "n";
			StringExtensions.Replacements['ô'] = "o";
			StringExtensions.Replacements['¨'] = "";
			StringExtensions.Replacements['å'] = "a";
			StringExtensions.Replacements['ã'] = "a";
			StringExtensions.Replacements['ˆ'] = "";
			StringExtensions.Replacements['©'] = "(c)";
			StringExtensions.Replacements['Ä'] = "A";
			StringExtensions.Replacements['Ï'] = "I";
			StringExtensions.Replacements['ò'] = "o";
			StringExtensions.Replacements['ê'] = "e";
			StringExtensions.Replacements['î'] = "i";
			StringExtensions.Replacements['Ü'] = "U";
			StringExtensions.Replacements['Á'] = "A";
			StringExtensions.Replacements['ß'] = "ss";
			StringExtensions.Replacements['¾'] = "3/4";
			StringExtensions.Replacements['È'] = "E";
			StringExtensions.Replacements['¼'] = "1/4";
			StringExtensions.Replacements['†'] = "+";
			StringExtensions.Replacements['³'] = "'";
			StringExtensions.Replacements['²'] = "'";
			StringExtensions.Replacements['Ø'] = "O";
			StringExtensions.Replacements['¸'] = ",";
			StringExtensions.Replacements['Ë'] = "E";
			StringExtensions.Replacements['ú'] = "u";
			StringExtensions.Replacements['Ö'] = "O";
			StringExtensions.Replacements['û'] = "u";
			StringExtensions.Replacements['Ú'] = "U";
			StringExtensions.Replacements['Œ'] = "Oe";
			StringExtensions.Replacements['º'] = "?";
			StringExtensions.Replacements['‰'] = "0/00";
			StringExtensions.Replacements['Å'] = "A";
			StringExtensions.Replacements['ø'] = "o";
			StringExtensions.Replacements['˜'] = "~";
			StringExtensions.Replacements['æ'] = "ae";
			StringExtensions.Replacements['ù'] = "u";
			StringExtensions.Replacements['‹'] = "<";
			StringExtensions.Replacements['±'] = "+/-";
		}
	}
}
