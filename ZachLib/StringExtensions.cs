using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    public static class StringExtensions
    {
        public static string ReplaceAll(this string str, Dictionary<string, string> dict)
        {
            if (dict.TryGetValue(str, out string newStr))
                return newStr;

            string[] keys = dict.Keys.Where(k => k.Length <= str.Length).ToArray();
            if (keys.Length == 0)
                return str;
            int minLength = keys.Min(k => k.Length);
            foreach (string key in keys)
            {
                if (str.Contains(key))
                {
                    str.Replace(key, dict[key]);
                    if (str.Length <= minLength)
                        return str;
                }
            }
            return str;
        }

        public static string TrimMiddle(this string str)
        {
            return str.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
        }

        public static string ReplaceAll(this string str, Dictionary<string, string> dict, int maxReplacements)
        {
            if (dict.TryGetValue(str, out string newStr))
                return newStr;

            int minLength = dict.Keys.Min(k => k.Length);
            if (str.Length <= minLength)
                return str;
            string[] keys = dict.Keys.Where(k => k.Length < str.Length).ToArray();
            
            foreach (string key in keys)
            {
                if (str.Contains(key))
                {
                    str.Replace(key, dict[key]);
                    --maxReplacements;
                    if (str.Length <= minLength || maxReplacements == 0)
                        return str;
                }
            }
            return str;
        }

        #region CharExtensions
        public static bool IsVowel(this char chr)
        {
            return VOWELS.Contains(Char.ToLower(chr));
        }

        private static readonly SortedDictionary<char, char> CAPITALS = new SortedDictionary<char, char>()
        {
            { 'a', 'A' },
            { 'b', 'B' },
            { 'c', 'C' },
            { 'd', 'D' },
            { 'e', 'E' },
            { 'f', 'F' },
            { 'g', 'G' },
            { 'h', 'H' },
            { 'i', 'I' },
            { 'j', 'J' },
            { 'k', 'K' },
            { 'l', 'L' },
            { 'm', 'M' },
            { 'n', 'N' },
            { 'o', 'O' },
            { 'p', 'P' },
            { 'q', 'Q' },
            { 'r', 'R' },
            { 's', 'S' },
            { 't', 'T' },
            { 'u', 'U' },
            { 'v', 'V' },
            { 'w', 'W' },
            { 'x', 'X' },
            { 'y', 'Y' },
            { 'z', 'Z' }
        };

        private static readonly char[] VOWELS = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', };
        private static readonly char[] VOWELS_SPACE = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', ' ' };
        /* public static int CountSyllables(this string str)
         {
             string[] phonemes = str.ToLower().Trim().Split(
                 VOWELS_SPACE,
                 StringSplitOptions.RemoveEmptyEntries
             );
             int syllables = phonemes.Length - 1;

             if (str.EndsWith("le"))
                 syllables += 1;
             else if (str.Last() != 'e' && VOWELS.Contains(str.Last()))
                 syllables += 1;
             else if (str.EndsWith("ed") && phonemes[1].Length == 1)
                 syllables -= 1;

             if (VOWELS.Contains(str.First()))
                 syllables += 1;
             phonemes = null;
             return Math.Max(1, syllables);
         }*/
        private static readonly char[] CONSONANTS = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        #endregion

        #region CountSyllables
        public static int CountSyllables(this string str)
        {
            int syllables = str.ToLower().Trim().Split(
                CONSONANTS, 
                StringSplitOptions.RemoveEmptyEntries
            ).Sum(
                p => p.EndsWith("i") ? 1 : p.Distinct().Count()
            );

            if (
                    (   
                        str.Last() == 'e' && 
                        !str.EndsWith("le")
                    ) || str.EndsWith("ey") || (
                        str.EndsWith("ed") && 
                        str.Split(
                            VOWELS_SPACE, 
                            StringSplitOptions.RemoveEmptyEntries
                        )[1].Length == 1
                    )
                )
                syllables -= 1;
            return syllables;
        }

        public static int CountSyllables(this string str, bool containsSpaces)
        {
            
            return !containsSpaces ? 
                CountSyllables(str) :
                str.Trim().Split(' ').Sum(w => w.CountSyllables());
            
        }
        #endregion

        #region IsTitleWord
        private static readonly string[] ARTICLES = new string[] { "a", "an", "the" };
        private static readonly string[] COORD_CONJ = new string[] { "and", "but", "for", "or", "nor", "yet", "so" };
        private static readonly string[] PREPOSITIONS = new string[] { "at", "by", "from", "of", "to", "at", "in", "on", "off", "onto", "with", "upon" };
        private static bool IsTitleWord(this string str)
        {
            str = str.ToLower().Trim();
            return !(ARTICLES.Contains(str) || COORD_CONJ.Contains(str) || PREPOSITIONS.Contains(str));
        }
        #endregion

        #region Abbreviate
        private static readonly char[] LETTER_FREQUENCY = new char[] { 'e', 't', 'a', 'o', 'i', 'n', 's', 'r', 'h', 'l', 'd', 'c', 'u', 'm', 'f', 'p', 'g', 'w', 'y', 'b', 'v', 'k', 'x', 'j', 'q', 'z' };
        private static readonly string[] ES_PLURALS = new string[] {"ses", "ches", "shes", "xes", "zes"};
        private static readonly Lazy<Dictionary<string, string>> _presets = new Lazy<Dictionary<string, string>>(() => Utils.LoadDictionary(@"E:\Work Programming\Insight Temp Files\Address Abbreviations.txt"));
        private static Dictionary<string, string> PRESETS { get => _presets.Value;  }
        public static string AbbreviatePhrase(this string str, int maxLetters = 0)
        {
            str = str.Trim().ToLower();
            if (!str.Contains(" "))
                return str.Abbreviate(maxLetters);
            int length = str.Length;

            int total = maxLetters;
            int totalToNum = 0;
            int lettersRemaining = 0;

            var splitBySpaces = str.Split(' ').Select(
                (w, i) => new { Word = w.Trim(), Syllables = w.CountSyllables(), Index = i}
            ).ToList();

            string[] finalStrings = new string[splitBySpaces.Count];
            int count = splitBySpaces.Count;
            for (int i = 0; i < count; ++i)
            {
                string word = splitBySpaces[i].Word;

                if (i > 0 && i < count - 1 && !word.IsTitleWord() && splitBySpaces[i].Syllables == 1)
                {
                    finalStrings[i] = word.First().ToString();
                    --maxLetters;
                    splitBySpaces.RemoveAt(i);
                    --i;
                    --count;
                }
                else
                {
                    word = word.ToUpper();

                    if (PRESETS.TryGetValue(word, out string abbreviated))
                    {
                        if (abbreviated.Length + splitBySpaces.Count - 1 <= maxLetters)
                        {
                            finalStrings[i] = abbreviated;
                            maxLetters -= abbreviated.Length;
                            splitBySpaces.RemoveAt(i);
                            --i;
                            --count;
                        }
                    }
                    else
                    {
                        word = word.ReplaceAll(PRESETS, 1).ToLower();
                        if (word != splitBySpaces[i].Word)
                            splitBySpaces[i] = new { Word = word, Syllables = splitBySpaces[i].Syllables, Index = splitBySpaces[i].Index };

                        /*foreach (var abbreviation in PRESETS)
                        {
                            if (word.StartsWith(abbreviation.Key) || word.EndsWith(abbreviation.Key))
                            {
                                if (abbreviation.Value.Length + splitBySpaces.Count - 1 <= maxLetters)
                                {
                                    finalStrings[i] = abbreviation.Value;
                                    maxLetters -= abbreviated.Length;
                                    splitBySpaces.RemoveAt(i);
                                    --i;
                                    --count;
                                }
                                break;
                            }
                        }*/
                    }
                }
                
            }

            if (maxLetters > 0)
            {
                total = splitBySpaces.Sum(w => w.Syllables);

                if (total == maxLetters)
                    totalToNum = 1;
                else if (maxLetters > total)
                {
                    totalToNum = maxLetters / total;
                    lettersRemaining = maxLetters % total;
                    

                    while (lettersRemaining > 0)
                    {
                        var word = splitBySpaces.OrderByDescending(
                            w => Math.Abs(1.0 - (w.Word.Length / w.Syllables))
                        ).First();

                        splitBySpaces[splitBySpaces.IndexOf(word)] = new {
                            Word = word.Word,
                            Syllables = word.Syllables + 1,
                            Index = word.Index
                        };

                        --lettersRemaining;
                    }
                }
                else
                {
                    totalToNum = total / maxLetters;
                    lettersRemaining = total % maxLetters;

                    while (lettersRemaining > 0)
                    {
                        var word = splitBySpaces.Where(
                            w => w.Syllables > 1
                        ).OrderBy(
                            w => Math.Abs(1.0 - (w.Word.Length / w.Syllables))
                        ).First();

                        --lettersRemaining;
                        splitBySpaces[splitBySpaces.IndexOf(word)] = new
                        {
                            Word = word.Word,
                            Syllables = word.Syllables - 1,
                            Index = word.Index
                        };
                    }
                }
            }

            foreach (var word in splitBySpaces)
            {
                finalStrings[word.Index] = word.Word.Abbreviate(word.Syllables / totalToNum).ToUpper();
            }

            return String.Join("", finalStrings);
        }

        public static string Abbreviate(this string word, int maxLetters)
        {
            word = word.Trim().ToLower();
            int length = word.Length;
            if (maxLetters <= 0)
                maxLetters = word.CountSyllables(false);

            if (length <= maxLetters)
                return word.ToUpper();

            char last = word.Last();
            char first = word.First();
            if (word.StartsWith("ex"))
            {
                word = word.Substring(1);
                first = 'x';
            }

            bool plural = false;
            length = word.Length;
            if (word.EndsWith("s"))
            {
                char letterCheck = word[length - 2];
                int start = Math.Max(0, length - 5);
                string end = word.Substring(start, Math.Min(4, length - start));

                if (end.EndsWith("les"))
                {
                    word = word.Substring(0, length - 2);
                    last = 'l';
                    plural = true;
                }
                else if (letterCheck != 's' && !letterCheck.IsVowel())
                {
                    word = word.Substring(0, length - 1);
                    last = word.Last();
                    plural = true;
                }
                else if (ES_PLURALS.Any(p => end.EndsWith(p)))
                {
                    word = word.Substring(0, length - 2);
                    last = word.Last();
                    plural = true;
                }
                length = word.Length;
            }

            
            /*if (word.EndsWith("le"))
            {
                word = word.Substring(0, length - 1);
                last = 'l';
            }
            else */
            if (last == 'e' && !word[word.Length - 2].IsVowel())
            {
                word = word.Substring(0, length - 1);
                last = word.Last();
            }

            string consonantsOnly = (first + new string(
                word.Substring(1, length - 2).Split(
                    VOWELS, StringSplitOptions.RemoveEmptyEntries
                ).SelectMany(
                    p => p.Distinct()
                ).ToArray()
            ) + last + (plural ? 's' : ' ')).Trim();

            if (consonantsOnly.Length <= maxLetters)
                return consonantsOnly.ToUpper();

            if (plural)
                --maxLetters;
            char pluralChar = (plural ? 's' : ' ');

            if (length <= maxLetters)
                return (word + 's').ToUpper();
            else if (maxLetters == 2)
                return new string(new char[] { first, last, pluralChar }).Trim().ToUpper();
            else if (maxLetters == 1)
                return new string(new char[] { first, pluralChar }).Trim().ToUpper();
            else
            {
                bool prelast = false;
                char prelastChar = ' ';
                StringBuilder builder = new StringBuilder();
                maxLetters -= 2;
                builder.Append(first);

                word = new string(word.Substring(1, word.Length - 2).SkipWhile(c => c.IsVowel()).Reverse().SkipWhile(c => c.IsVowel()).Reverse().ToArray());
                length = word.Length;

                if (length > 3)
                {
                    char chrTemp = word[0];
                    if (maxLetters > 1 && !chrTemp.IsVowel() && chrTemp != first)
                    {
                        builder.Append(chrTemp);
                        --maxLetters;
                        word = new string(word.SkipWhile(c => c == chrTemp).ToArray());
                    }

                    chrTemp = word.Last();
                    if (maxLetters > 1 && !chrTemp.IsVowel() && chrTemp != last)
                    {
                        prelast = true;
                        prelastChar = chrTemp;
                        --maxLetters;
                        word = new string(word.Reverse().SkipWhile(c => c == chrTemp).Reverse().ToArray());
                    }

                    length = word.Length;
                }
                
                if (length <= 3)
                {
                    var consonants = word.Where(c => !c.IsVowel()).ToArray();
                    if (consonants.Length == 1)
                        builder.Append(word.First(c => !c.IsVowel()));
                    else if (consonants.Length > 0)
                        builder.Append(consonants.Take(maxLetters).ToArray());
                }
                else
                {
                    string temp = word;
                    word = new string(
                        word.SkipWhile(
                            c => !c.IsVowel()
                        ).Reverse().SkipWhile(
                            c => !c.IsVowel()
                        ).Reverse().ToArray()
                    );

                    if (!word.Any() || word.All(c => c.IsVowel()))
                        word = temp;

                    /*if (!VOWELS.Contains(temp.First()))
                    {
                        var tempList = reduced.ToList();
                        int firstVowel = tempList.FindIndex(c => VOWELS.Contains(c));
                        reduced = VOWELS.Contains(temp.Last()) ?
                            reduced.Substring(
                                firstVowel + 1,
                                tempList.FindLastIndex(
                                    c => VOWELS.Contains(c)
                                ) - firstVowel
                            ) : reduced.Substring(
                                firstVowel + 1
                            );
                    }
                    else if (!VOWELS.Contains(temp.Last()))
                        reduced = reduced.Substring(0, reduced.ToList().FindLastIndex(c => VOWELS.Contains(c)));*/

                    length = word.Length;

                    if (word.Any() && word.Any(c => !c.IsVowel()))
                    {
                        var splitByVowels = word.Split(
                            VOWELS,
                            StringSplitOptions.RemoveEmptyEntries
                        ).Select(
                            p => (p.EndsWith("h") || p.EndsWith("l") ?
                                    p.Reverse() :
                                    p.Reverse().TakeWhile(
                                        c => c != 'h' && c != 'l'
                                    )
                                ).Take(2).Reverse().Distinct().ToArray()
                        ).ToList();

                        while (splitByVowels.Any() && maxLetters > 0)
                        {
                            int midpoint = (splitByVowels.Count / 2) - (splitByVowels.Count % 2 == 1 ? 0 : 1);
                            var midChar = splitByVowels[midpoint];
                            if (maxLetters > 1 && midChar.Length == 2)
                                builder.Append(midChar);
                            else
                                builder.Append(midChar.Last());
                            --maxLetters;
                            splitByVowels.RemoveRange(0, Math.Max(1, midpoint - 1));
                        }
                    }
                }

                if (prelast)
                    builder.Append(prelastChar);
                builder.Append(last);
                if (plural)
                    builder.Append('s');
                return builder.ToString().ToUpper();
            }
        }
        #endregion

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

        /*private static readonly Graphics gfxText = Graphics.FromImage(new Bitmap(2200, 2000));
        private static readonly Font CONSOLE_FONT = new Font("Courier New", 16);
        private static readonly float DASH_SIZE = gfxText.MeasureString("-", CONSOLE_FONT).Width;
        private static readonly float BASE_SIZE = GetConsoleTitleWidth();*/
        private const string CONSOLE_TITLE_FORMAT =
            " ~ --------{1} ~ \r\n" +
            " ~ --- {0} ---{2} ~ \r\n" +
            " ~ --------{1} ~ ";
        private const string BASE_SIZE_TESTER = "\t ~ --- {0} --- ~ \t";
        public static string ToConsoleTitle(this string str) =>
            String.Format(BASE_SIZE_TESTER, str);

        public static void ConsoleWriteTable(this string[,] table)
        {
            int rows = table.GetLength(0);
            int cols = table.GetLength(1);

            int[] maxLengths = new int[cols];
            for (int j = 0; j < cols; ++j)
            {
                int maxLength = -1;
                for (int i = 0; i < rows; ++i)
                {
                    var length = table[i, j].Length;
                    if (length > maxLength)
                        maxLength = length;
                }

                if (maxLength <= 4)
                    maxLength = 4;
                else if (maxLength % 8 != 0)
                {
                    if (maxLength % 2 == 1)
                        ++maxLength;
                    if (maxLength % 8 != 0)
                    {
                        if (maxLength % 4 != 0)
                            maxLength += 2;
                        if (maxLength % 8 != 0)
                            maxLength += 4;
                    }
                }

                maxLengths[j] = maxLength;
            }

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    Console.Write(table[i, j].PadRight(maxLengths[j], ' ') + "    ");
                }
                Console.WriteLine();
            }
        }
    }
}
