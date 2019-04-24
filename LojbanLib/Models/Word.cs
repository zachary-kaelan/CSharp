using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LojbanLib.Models
{
    public enum WordForm
    {
        None,
        CmavoCompound,
        V,
        CV,
        VV,
        CVV,
        CVCCV,  // CVC/CV
        CCVCV
    }

    [Flags]
    public enum WordType
    {
        None,
        Cmavo,
        Gismu,
        Lujvo,
        Fu_ivla,
        Cmene
    }

    public class Word
    {
        public bool IsValid { get; private set; }
        public string InvalidReason { get; private set; }
        public WordForm Form { get; private set; }
        public WordType Type { get; private set; }
        private bool EndsInVowel { get; set; }
        private bool EndsInConsonant { get; set; }

        private static readonly SortedSet<string> FORBIDDEN_CMAVO_DIPHTHONGS = new SortedSet<string>() { "ai", "iy", "uy", "oi" };

        public Word(string word)
        {
            InvalidReason = null;
            Type = WordType.None;
            Form = WordForm.None;
            // Input testing
            if (String.IsNullOrWhiteSpace(word))
            {
                InvalidReason = "Word is null, empty, or whitespace";
                return;
            }
            var wordNoSemis = new string(word.Where(c => !LojbanData.SEMI_LETTERS.Contains(c)).ToArray());
            int length = wordNoSemis.Length;

            // Iterate through the word
            List<char> invalidChars = new List<char>();
            char[] wordFormArr = new char[length];
            for (int i = 0; i < length; ++i)
            {
                char chr = wordNoSemis[i];
                // Make sure all the characters are indeed within the Lojban alphabet
                if (!LojbanData.LETTERS.Contains(chr))
                    invalidChars.Add(chr);
                else if (chr.IsVowel())
                    wordFormArr[i] = 'V';
                else
                    wordFormArr[i] = 'C';
            }

            if (invalidChars.Count > 0)
            {
                InvalidReason = "Invalid characters: [" + String.Join(", ", invalidChars) + "]";
                IsValid = false;
            }
            else
            {
                bool containsApostrophe = word.Contains('\'');
                EndsInVowel = wordFormArr[length - 1] == 'V';
                string wordForm = new string(wordFormArr);
                if (Enum.TryParse(wordForm, out WordForm form))
                {
                    IsValid = true;
                    Form = form;
                    switch (length)
                    {
                        case int n when n < 4:
                            Type = WordType.Cmavo;
                            if (Form == WordForm.CVV)
                                IsValid = !FORBIDDEN_CMAVO_DIPHTHONGS.Contains(wordForm.Substring(1));
                            else if (Form == WordForm.VV)
                                IsValid = !FORBIDDEN_CMAVO_DIPHTHONGS.Contains(wordForm);
                            break;

                        case 5:
                            Type = WordType.Gismu;
                            IsValid = Form == WordForm.CVCCV ?
                                !containsApostrophe && new Pair(word[2], word[3]).IsValid :
                                !containsApostrophe;
                            break;

                        default:
                            IsValid = false;
                            break;
                    }
                }

                if (!IsValid)
                {
                    if (EndsInVowel)
                    {
                        var clusterIndex = wordForm.IndexOf("CC");
                        if (clusterIndex == -1)
                            Form = WordForm.CmavoCompound;
                        else
                        {
                            int hyphenIndex = wordNoSemis.IndexOf('y');
                            bool hyphenIrrelevant = hyphenIndex == -1 || hyphenIndex > 4 || clusterIndex < hyphenIndex;
                            bool isBrivla = false;
                            if (hyphenIrrelevant && clusterIndex < 4)
                                isBrivla = true;
                            else if (!hyphenIrrelevant)
                            {
                                int max = 6;
                                for (int i = hyphenIndex + 1; i < max; ++i)
                                {
                                    if (wordNoSemis[i] == 'y')
                                        ++max;
                                }
                                if (clusterIndex < max - 1)
                                    isBrivla = true;
                            }


                        }
                    }
                    if (EndsInVowel && !wordForm.Contains("CC"))
                        Form = WordForm.CmavoCompound;
                    else
                    {
                        // Lojbanized word
                    }
                }
            }
            
        }
    }
}
