using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.GeneticAlgorithm
{
    // template for gene parameters
    public struct GeneParameter
    {
        public byte MaxValue { get; private set; }  // number of possible values, minus 1
        public byte Bits { get; private set; }
        public byte Index { get; private set; }
        public bool Mutable { get; private set; }

        public GeneParameter(byte index, bool mutable, params int[] values)
        {
            Index = index;
            Mutable = mutable;
            MaxValue = (byte)(values.Length - 1);
            byte temp = MaxValue;
            Bits = (byte)(Constants.ALL_POWERS_OF_2_REVERSE.FindLastIndex(p => p < temp) + 1);
        }
    }
}
