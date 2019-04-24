using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RGX;
using RGX.UTILS;
using RGX.HTML;
using RGX.Reddit;
using RGX.Reddit.LFS;

namespace ZachLib
{
    public static class ZachRGX
    {
        static ZachRGX()
        {
            Assembly rgx = Assembly.Load(
                new AssemblyName("ZachRGX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=41ff425b50d3cee4")
            );

            FILE_DICTIONARY = (Regex)rgx.CreateInstance("RGX.UTILS.FileDictionary");
            FILE_DIGIT = (Regex)rgx.CreateInstance("RGX.UTILS.FileDigit");
            FILENAME_DISALLOWED_CHARACTERS = (Regex)rgx.CreateInstance("RGX.UTILS.MakeFilenameFriendly");
            PHONE = (Regex)rgx.CreateInstance("RGX.UTILS.Phone");
            SECONDARY_FORMATTING = (Regex)rgx.CreateInstance("RGX.UTILS.SecondaryFormatting");
            SYMBOLS = (Regex)rgx.CreateInstance("RGX.UTILS.Symbols");
            NON_ALPHA_NUMERIC = (Regex)rgx.CreateInstance("RGX.UTILS.NonAlphaNumeric");
            XML_KEY_VALUE = (Regex)rgx.CreateInstance("RGX.UTILS.XmlKeyValue");
            XML_SINGLE_VALUE = (Regex)rgx.CreateInstance("RGX.UTILS.XmlValue");

            HTML_LINKS = (Regex)rgx.CreateInstance("RGX.HTML.Links");
            HTML_PARAGRAPHS = (Regex)rgx.CreateInstance("RGX.HTML.Paragraphs");
            HTML_TAGS = (Regex)rgx.CreateInstance("RGX.HTML.WebTags");

            LFS_CheckQuote1 = (Regex)rgx.CreateInstance("RGX.Reddit.LFS.CheckQuote1");
            LFS_CheckQuote2 = (Regex)rgx.CreateInstance("RGX.Reddit.LFS.CheckQuote2");
            CheckSarcasm = (Regex)rgx.CreateInstance("RGX.Reddit.CheckSarcasm");
        }

        public static Regex PHONE { get; private set; }
        public static Regex FILE_DICTIONARY { get; private set; }
        public static Regex FILE_DIGIT { get; private set; }
        public static Regex FILENAME_DISALLOWED_CHARACTERS { get; private set; }
        public static Regex SECONDARY_FORMATTING { get; private set; }
        public static Regex SYMBOLS { get; private set; }
        public static Regex NON_ALPHA_NUMERIC { get; private set; }
        public static Regex XML_KEY_VALUE { get; private set; }
        public static Regex XML_SINGLE_VALUE { get; private set; }

        public static Regex HTML_LINKS { get; private set; }
        public static Regex HTML_PARAGRAPHS { get; private set; }
        public static Regex HTML_TAGS { get; private set; }

        public static Regex LFS_CheckQuote1 { get; private set; }
        public static Regex LFS_CheckQuote2 { get; private set; }
        public static Regex CheckSarcasm { get; private set; }
    }
}
