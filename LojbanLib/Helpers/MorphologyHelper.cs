using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LojbanLib.Models;

namespace LojbanLib.Helpers
{
    public enum RafsiForm
    {
        None = 0,
        CVC_CV = 1,
        CVC_C,
        CCVCV,
        CCVC,
        CVC,
        CV_V,
        CCV,
        CVV
    }

    public enum ConsonantClusterType
    {
        Invalid,
        NonInitial,
        Initial
    }

    public static class MorphologyHelper
    {
        internal static readonly SortedSet<char> CLUSTER_FORBIDDEN_SHARED_CHARS = new SortedSet<char>() { 'c', 'j', 's', 'z' };
        private static readonly SortedSet<char> INITIAL_PAIRS_ONLY_L_R = new SortedSet<char>() { 'p', 'b', 'f', 'v', 'm', 'k', 'g', 'x' };

        public static RafsiForm GetRafsiForm(string rafsi, out string rafsiFormStr)
        {
            char lastChar = rafsi[0];
            bool lastCharIsVowel = lastChar.IsVowel();
            StringBuilder sb = new StringBuilder(3, 6);
            sb.Append(lastCharIsVowel ? 'V' : 'C');
            for (int i = 1; i < rafsi.Length; ++i)
            {
                char chr = rafsi[i];
                if (chr.IsVowel())
                {
                    if (lastCharIsVowel && lastChar == '\'')
                        sb.Append('_');
                    sb.Append('V');

                    lastChar = chr;
                    lastCharIsVowel = true;
                }
                else if (chr == '\'')
                {
                    if (lastCharIsVowel)
                        lastChar = chr;
                    else
                        lastCharIsVowel = false;
                }
                else
                {
                    if (!lastCharIsVowel && i > 1)
                        sb.Append('_');
                    sb.Append('C');

                    lastChar = chr;
                    lastCharIsVowel = false;
                }
            }
            rafsiFormStr = sb.ToString();
            sb = null;
            return Enum.TryParse(rafsiFormStr, out RafsiForm form) ? form : RafsiForm.None;
        }

        public static ConsonantClusterType GetClusterType(char chr1, char chr2, out Pair pair)
        {
            pair = new Pair(chr1, chr2);
            if (pair.IsValid)
                return pair.IsConsonantPair && IsPermissibleInitialPair(chr1, chr2) ?
                    ConsonantClusterType.Initial : ConsonantClusterType.NonInitial;
            else
                return ConsonantClusterType.Invalid;
        }

        public static ConsonantClusterType GetClusterType(char chr1, char chr2)
        {
            throw new NotImplementedException();
        }

        // Assuming it is valid
        internal static bool IsPermissibleInitialPair(char chr1, char chr2)
        {
            if (CLUSTER_FORBIDDEN_SHARED_CHARS.Contains(chr1))
                return chr1.IsVoiced() ? true :
                    !LojbanData.SYLLABIC_CONSONANTS.Contains(chr2) || chr2 == 'm';
            if (INITIAL_PAIRS_ONLY_L_R.Contains(chr1))
                return chr2 == 'l' || chr2 == 'r';
            return chr1.IsVoiced() ?
                chr1 == 't' && (
                    chr2 == 'c' ||
                    chr2 == 'r' ||
                    chr2 == 's'
                ) : chr1 == 'd' && (
                    chr2 == 'j' ||
                    chr2 == 'r' ||
                    chr2 == 'z'
                );
        }
    }
}
