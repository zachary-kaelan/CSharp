using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3.GeneticAlgorithm
{
    // template for gene parameters
    public struct GeneParameter
    {
        public byte MaxValue { get; private set; }  // number of possible values, minus 1
        public byte Bits { get; private set; }
        public byte Index { get; private set; }
        public bool Mutable { get; private set; }

        public GeneParameter(byte index, bool mutable, params byte[] values)
        {
            Index = index;
            Mutable = mutable;
            //MaxValue = (byte)(values.Length - 1);
            MaxValue = (byte)values.Length; // upper exclusive bound
            byte temp = MaxValue;
            Bits = (byte)(Constants.ALL_POWERS_OF_2_REVERSE.FindLastIndex(p => p < temp) + 1);
        }

        public byte GetRandomValue() =>
            (byte)Constants.GEN.Next(MaxValue + 1);
    }
}
