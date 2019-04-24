using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3.GeneticAlgorithm
{
    // a single slot
    public class Gene : IEquatable<Gene>, IComparable<Gene>
    {
        internal static byte NumParameters = 5;
        public bool IsConstant { get; private set; }
        private ushort ID { get; set; }
        private byte[] Bytes { get; set; }
        public byte this[int index] { get => Bytes[index]; set => Bytes[index] = value; }

        public Gene(ushort ID, byte[] bytes, bool isConstant = false)
        {
            this.ID = ID;
            Bytes = bytes;
            IsConstant = isConstant;
        }

        public void SerializeWith(FileStream stream)
        {
            stream.Write(BitConverter.GetBytes(ID), 0, 2);
            stream.Write(Bytes, 0, Bytes.Length);
        }

        // randomize the mutable parameters
        public Gene Randomize(byte numMutable = 2)
        {
            byte[] copy = new byte[NumParameters];
            Array.Copy(Bytes, copy, NumParameters);
            if (!IsConstant)
            {
                for (byte i = (byte)(NumParameters - numMutable); i < NumParameters; ++i)
                {
                    copy[i] = GenePool.GetGeneParamRandomValue(i);
                }
                return new Gene(ID, copy);
            }
            else
                throw new NotImplementedException();
        }

        public byte Swap(Gene other, byte paramIndex)
        {
            byte param = Bytes[paramIndex];
            if (other.Bytes[paramIndex] == param)
                return FullSwap(other);
            Bytes[paramIndex] = other.Bytes[paramIndex];
            other.Bytes[paramIndex] = param;
            return 0;
        }

        public byte FullSwap(Gene other)
        {
            byte numEqual = GenePool.NUM_PARAMS - GenePool.NUM_IMMUTABLE;
            for (byte paramIndex = GenePool.NUM_IMMUTABLE; paramIndex < GenePool.NUM_PARAMS; ++paramIndex)
            {
                byte temp = Bytes[paramIndex];
                if (temp != other[paramIndex])
                {
                    --numEqual;
                    Bytes[paramIndex] = other[paramIndex];
                    other[paramIndex] = temp;
                }
            }
            return numEqual;
        }

        public bool Equals(Gene other) =>
            ID == other.ID;

        public int CompareTo(Gene other) =>
            ID - other.ID;
    }
}
