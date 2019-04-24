using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.SequenceMotifs
{
    internal struct MotifOp<TElement> where TElement : Enum
    {
        public MotifCode Code { get; set; }
        public TElement Value { get; set; }
        public int Lower { get; set; }
        public int Upper { get; set; }

        public MotifOp(MotifCode code, TElement codon, int lower, int upper)
        {
            Code = code;
            Value = codon;
            Lower = lower;
            Upper = upper;
        }
    }
}
