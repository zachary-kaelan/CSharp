using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.GeneticAlgorithm
{
    // a single slot
    public class Gene
    {
        protected byte[] Bytes { get; set; }
        public byte this[int index] => Bytes[index];

        public Gene(byte[] bytes)
        {
            Bytes = bytes;
        }

        public bool Swap(Gene other, int paramIndex)
        {
            byte param = Bytes[paramIndex];
            Bytes[paramIndex] = other.Bytes[paramIndex];
            if (Bytes[paramIndex] == param)
                return false;
            other.Bytes[paramIndex] = param;
            return true;
        }

        public bool PointShift(GeneParameter param)
        {
            byte prevVal = Bytes[param.Index];
            return true;
        }
    }
}
