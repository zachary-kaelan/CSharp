using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.SequenceMotifs
{
    internal enum MotifCode
    {
        NIL = 0,
        One = 1,
        Set = 2,
        Multi = 3,
        Nol = 4,
        Col = 5,

        Skip = 16,
        Not = 64,
        Loop = 128,

        NotOne = Not | One,
        OneLoop = One | Loop,
        NotOneLoop = Not | One | Loop,
        NotSet = Not | Set,
        SetLoop = Set | Loop,
        NotSetLoop = Not | Set | Loop
    }

    internal enum MotifNodeType
    {
        OneLoop = MotifCode.OneLoop,
        One = MotifCode.One,
        NotOne = MotifCode.NotOne,
        Set = MotifCode.Set,
        Multi = MotifCode.Multi,
        Nol = MotifCode.Nol,
        Col = MotifCode.Col,

        Loop
    }

    internal static class MotifConstants
    {
        public static readonly SortedDictionary<char, DNACodon> CODON_CHARS = new SortedDictionary<char, DNACodon>()
        {
            { 'F', DNACodon.Phe },
            { 'L', DNACodon.Leu },
            { 'I', DNACodon.Ile },
            { 'M', DNACodon.Met },
            { 'V', DNACodon.Val },
            { 'S', DNACodon.Ser },
            { 'P', DNACodon.Pro },
            { 'T', DNACodon.Thr },
            { 'A', DNACodon.Ala },
            { 'O', DNACodon.Och },
            { 'J', DNACodon.Amb },
            { 'H', DNACodon.His },
            { 'Q', DNACodon.Gln },
            { 'N', DNACodon.Asn },
            { 'K', DNACodon.Lys },
            { 'D', DNACodon.Asp },
            { 'E', DNACodon.Glu },
            { 'C', DNACodon.Cys },
            { 'U', DNACodon.Opl },
            { 'W', DNACodon.Trp },
            { 'R', DNACodon.Arg },
            { 'G', DNACodon.Gly }
        };

        public static readonly DNACodon ALL_CODONS = Enum.GetValues(typeof(DNACodon)).Cast<DNACodon>().Aggregate(DNACodon.NIL, (sum, codon) => sum | codon);
    }
}
