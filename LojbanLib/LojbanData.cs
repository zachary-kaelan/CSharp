using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LojbanLib.Models;

namespace LojbanLib
{
    internal static class LojbanData
    {
        public static readonly SortedSet<char> LETTERS = new SortedSet<char>() { '\'', ',', '.', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z' };
        public static readonly SortedSet<char> VOWELS = new SortedSet<char>() { 'a', 'e', 'i', 'o', 'u', 'y' };
        public static readonly SortedSet<char> CONSONANTS = new SortedSet<char>() { 'b', 'c', 'd', 'f', 'g', 'j', 'k', 'l', 'm', 'n', 'p', 'r', 's', 't', 'v', 'x', 'z' };
        public static readonly SortedSet<char> SEMI_LETTERS = new SortedSet<char>() { '\'', ',', '.' };
        public static readonly SortedSet<char> SYLLABIC_CONSONANTS = new SortedSet<char>() { 'l', 'm', 'n', 'r' };
        public static readonly SortedDictionary<char, char> VOICED_COUNTERPARTS = new SortedDictionary<char, char>()
        {
            { 'p', 'b' },
            { 't', 'd' },
            { 'k', 'g' },
            { 'f', 'v' },
            { 'c', 'j' },
            { 's', 'z' }
        };

        //public static readonly SortedSet<char> UNVOICED_CONSONANTS = new SortedSet<char>(VOICED_COUNTERPARTS.Values);

        public static readonly SortedSet<Pair> DIPHTHONGS = new SortedSet<Pair>()
        {
            new Pair(true, 'a', 'i', true),
            new Pair(true, 'e', 'i', true),
            new Pair(true, 'o', 'i', true),
            new Pair(true, 'a', 'u', true),
            new Pair(true, 'i', 'a', true),
            new Pair(true, 'i', 'e', true),
            new Pair(true, 'i', 'i', true),
            new Pair(true, 'i', 'o', true),
            new Pair(true, 'i', 'u', true),
            new Pair(true, 'u', 'a', true),
            new Pair(true, 'u', 'e', true),
            new Pair(true, 'u', 'i', true),
            new Pair(true, 'u', 'o', true),
            new Pair(true, 'u', 'u', true),
            new Pair(true, 'i', 'y', true),
            new Pair(true, 'u', 'y', true)
        };

        internal static string RemoveSemiletters(this string str) =>
            new string(str.Where(c => !SEMI_LETTERS.Contains(c)).ToArray());

        internal static string RemoveSemilettersExceptApostrophe(this string str) =>
            new string(str.Where(c => c != ',' && c != '.').ToArray());

        internal static bool IsVowel(this char chr) =>
            VOWELS.Contains(chr);

        public static bool IsVoiced(this char chr) =>
            !chr.IsVowel() && (
                VOICED_COUNTERPARTS.ContainsKey(chr) ||
                SYLLABIC_CONSONANTS.Contains(chr)
            );
    }
}
