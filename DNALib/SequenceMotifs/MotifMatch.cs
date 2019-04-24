using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.SequenceMotifs
{
    public class MotifMatch<TElement>
    {
        public int Index { get; private set; }
        public int Length { get; private set; }
        public TElement[] Value { get; private set; }

        internal MotifMatch(int index, int length, TElement[] value)
        {
            Index = index;
            Length = length;
            Value = value;
        }
    }
}
