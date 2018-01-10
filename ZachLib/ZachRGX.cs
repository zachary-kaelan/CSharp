using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RGX.UTILS;
using RGX.HTML;

namespace ZachLib
{
    public static class ZachRGX
    {
        public static readonly Phone PHONE = new Phone();
        public static readonly FileDictionary FILE_DICTIONARY = new FileDictionary();
        public static readonly FileDigit FILE_DIGIT = new FileDigit();
        public static readonly SecondaryFormatting SECONDARY_FORMATTING = new SecondaryFormatting();
        public static readonly Symbols SYMBOLS = new Symbols();

        public static readonly Links HTML_LINKS = new Links();
        public static readonly Paragraphs HTML_PARAGRAPHS = new Paragraphs();
        public static readonly WebTags HTML_TAGS = new WebTags();
    }
}
