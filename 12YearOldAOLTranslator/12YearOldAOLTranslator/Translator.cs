using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _12YearOldAOLTranslator
{
    class Translator
    {
        public static readonly Dictionary<string[], string> flatTranslate = new Dictionary<string[], string>()
        {
            {
                new string[] {
                    "one"
                }, "1"
            },
            {
                new string[] {
                    "to", "too",
                    "two", "tu"
                }, "2"
            },
            {
                new string[]
                {
                    "three", "free"
                }, "3"
            },
            {
                new string[]
                {
                    "for", "four",
                    "fore"
                }, "4"
            },
            {
                new string[]
                {
                    "six", "sex"
                }, "6"
            },
            {
                new string[]
                {
                    "eight", "ate"
                }, "8"
            },
            {
                new string[]
                {
                    "ten"
                }, "10"
            },

            // ---------------

            {
                new string[]
                {
                    "ck"
                }, "k"
            },

            // --------------

            {
                new string[]
                {
                    "you"
                }, "u"
            },
            {
                new string[]
                {
                    "oo"
                }, "ew"
            }
        };

        public static readonly Dictionary<char[], char> singles = new Dictionary<char[], char>()
        {
            {
                new char[]
                {
                    'i', 'l', '!'
                }, '1'
            },
            {
                new char[]
                {
                    'e'
                }, '3'
            },
            {
                new char[]
                {
                    'a', 'p', 'h'
                }, '4'
            },
            {
                new char[]
                {
                    's'
                }, '5'
            },
            {
                new char[]
                {
                    'u'
                }, 'o'
            },
            {
                new char[]
                {
                    't'
                }, '7'
            },
            {
                new char[]
                {
                    'b'
                }, '8'
            },
            {
                new char[]
                {
                    'o'
                }, '0'
            }
        };

        public static readonly char[][] mixMatchChars = new char[][]
        {
            new char[] {'g', 'j'},
            new char[] {'r', 'w'},
            new char[] {'a', 'e'},
            new char[] {'m', 'n'},
            new char[] {'z', 's'},
            new char[] {'c', 'k'}
        };

        public static readonly string[][] mixMatchChunks = new string[][]
        {
            new string[] {"ew", "u", "oo"},
            new string[] {"z", "ese", "ase"},
            new string[] {"eks", "ks", "x", "cs", "ecs", "ex"},
            new string[] {"eme", "eam", "eem"},
            new string[] {"h", ""}
        };
    }
}
