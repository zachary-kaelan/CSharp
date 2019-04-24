using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Antlr4.Runtime;
using LojbanLib.Models;
using LojbanLib.Helpers;
using ZachLib;

namespace LojbanLib
{
    public static class LojbanAnalyzer
    {
        public static bool IsValidWord(string word, out string reason)
        {
            int length = word.Length;
            List<char> invalidChars = new List<char>();
            for (int i = 0; i < length; ++i)
            {
                if (!LojbanData.LETTERS.Contains(word[i]))
                    invalidChars.Add(word[i]);
            }
            if (invalidChars.Count > 0)
            {
                reason = "Invalid characters: " + invalidChars.ToArrayString();
                return false;
            }
            reason = "Word is valid!";
            return true;
        }

        public static Tuple<string, int, string>[] GetLujvoVariants(params string[][] rafsi)
        {
            int count = rafsi.Length;
            var rafsiScores = new Tuple<string, RafsiForm, string>[count][];
            int lastIndex = count - 1;
            int variantsCount = 1;
            var counts = new int[count];
            for (int i = 0; i < count; ++i)
            {
                rafsiScores[i] = (
                        i == lastIndex ?
                            rafsi[i].Where(r => r.Count(c => c != '\'') <= 4) :
                            rafsi[i].Where(r => r.Count(c => c != '\'') != 4)
                    ).Select(
                        r => new Tuple<string, RafsiForm, string>(r, MorphologyHelper.GetRafsiForm(r, out string rafsiForm), rafsiForm)
                    ).ToArray();
                int rafsiCount = rafsiScores[i].Length;
                variantsCount *= rafsiCount;
                counts[i] = rafsiCount;
            }

            bool isGreaterThanTwo = count > 2;
            var variants = new List<Tuple<string, int, string>>(variantsCount);
            var indices = new int[count];
            int mainIndex = 0;
            while (mainIndex < variantsCount)
            {
                for (int i = lastIndex; i >= 0; --i)
                {
                    ++indices[i];
                    if (indices[i] == counts[i])
                        indices[i] = 0;
                    else
                        break;
                }

                int variantScore = 0;
                var variant = new KeyValuePair<string, RafsiForm>[count];
                string variantStr = "";
                string variantForm = "";
                int variantLength = 0;
                char lastRafsiLastChar = '_';
                bool lastRafsiRequiresHyphenation = false;
                bool doHyphenation = isGreaterThanTwo || variant[lastIndex].Value != RafsiForm.CVC;
                var joints = new List<KeyValuePair<int, int>>();
                int firstYHyphen = -1;

                for (int i = 0; i < count; ++i)
                {
                    var gismuRafsi = rafsiScores[i][indices[i]];

                    bool notLast = i != lastIndex;
                    bool first = i == 0;
                    bool doConsonantCheck = !first && lastRafsiLastChar != '_';
                    int rafsiLength = gismuRafsi.Item1.Length;
                    char lastChar = gismuRafsi.Item1[rafsiLength - 1];
                    if (lastChar.IsVowel())
                        lastChar = '_';

                    variantScore -= ((int)gismuRafsi.Item2) * 10;
                    variantScore -= gismuRafsi.Item1.Count(c => c.IsVowel() && c != 'y');

                    if (doConsonantCheck)
                    {
                        Pair pair = new Pair(lastRafsiLastChar, gismuRafsi.Item1[0], false);
                        if (!pair.IsValid)
                        {
                            variantStr += 'y';
                            variantForm += '-';
                            variantScore += 100;
                            if (firstYHyphen == -1)
                                firstYHyphen = variantLength;
                            ++variantLength;
                        }
                        else if (firstYHyphen == -1)
                        {
                            joints.Add(new KeyValuePair<int, int>(variantLength - 1, variantForm.Length));
                            variantForm += '_';
                        }
                    }
                    else if (lastRafsiRequiresHyphenation)
                    {
                        // if previous rafsi ends with 'r', must hyphenate with an 'n' instead
                        variantStr += gismuRafsi.Item1[0] == 'r' ? 'n' : 'r';
                        variantForm += '-';
                        variantScore += 100;
                        ++variantLength;
                    }

                    variantStr += gismuRafsi.Item1;
                    variantForm += gismuRafsi.Item3;

                    if (gismuRafsi.Item2 == RafsiForm.CV_V)
                        variantScore -= 500;

                    if (doHyphenation && notLast && (gismuRafsi.Item2 == RafsiForm.CVV || gismuRafsi.Item2 == RafsiForm.CV_V) && (isGreaterThanTwo || first))
                    {
                        doHyphenation = false;
                        lastRafsiRequiresHyphenation = true;
                    }
                    else
                    {
                        if ((gismuRafsi.Item2 == RafsiForm.CVC_C || gismuRafsi.Item2 == RafsiForm.CVC_CV) && firstYHyphen == -1)
                            joints.Add(new KeyValuePair<int, int>(variantLength + 2, variantForm.Length + 3));
                        if (notLast && gismuRafsi.Item3.Where(c => c != '_').Count() == 4)
                        {
                            variantStr += 'y';
                            variantForm += '-';
                            variantScore += 100;
                            if (firstYHyphen == -1)
                                firstYHyphen = variantLength;
                            ++variantLength;
                        }
                    }
                    
                    variant[i] = new KeyValuePair<string, RafsiForm>(gismuRafsi.Item1, gismuRafsi.Item2);
                    variantLength += rafsiLength;
                }

                if (joints.Count > 0)
                {
                    // if there are no y-hyphens, searches joints throughout the entire word
                    // only for debugging purposes, as no further joints are added after the y-hyphen is set
                    if (firstYHyphen == -1)
                        firstYHyphen = variantLength;
                    if (joints.All(j => MorphologyHelper.IsPermissibleInitialPair(variantStr[j.Key], variantStr[j.Key + 1])))
                    {
                        var firstJoint = joints[0];
                        variantStr = variantStr.Insert(firstJoint.Key, "y");
                        variantForm = variantForm.Substring(0, firstJoint.Value) + '-' + variantForm.Substring(firstJoint.Value + 1);
                        ++variantLength;
                        variantScore += 100;
                    }
                }
                variantScore += variantLength * 1000;

                variants.Add(new Tuple<string, int, string>(variantStr, variantScore, variantForm));

                ++mainIndex;
            }

            return variants.OrderBy(v => v.Item2).ToArray();
        }
    }

}
