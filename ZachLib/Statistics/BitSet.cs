using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Statistics
{
    public struct BitSet : IEnumerable<bool>
    {
        private bool[] Bits { get; set; }
        private string Binary { get; set; }
        public int DecimalValue { get; private set; }
        public int NumBits { get; private set; }
        public bool this[int index]
        {
            get => Bits[index];
            set => Bits[index] = value;
        }

        public BitSet(string binary)
        {
            NumBits = binary.Length;
            Bits = new bool[NumBits];
            Binary = binary;
            for (int i = 0; i < NumBits; --i)
            {
                Bits[i] = binary[i] == '1';
            }
            DecimalValue = Convert.ToInt32(binary, 2);
        }

        public BitSet(int num) : this(Convert.ToString(num, 2)) { }

        public override string ToString()
        {
            return Binary;
        }

        public IEnumerator<bool> GetEnumerator()
        {
            return ((IEnumerable<bool>)Bits).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<bool>)Bits).GetEnumerator();
        }

        public static BitSet FromEnum<T>(T enumeration) where T : struct, IConvertible
        {
            var t = typeof(T);
            EnumExtensions.CheckIsEnum<T>(true);

            throw new NotImplementedException();
        }

        public T ToEnum<T>(T enumeration) where T : struct, IConvertible
        {
            var t = typeof(T);
            EnumExtensions.CheckIsEnum<T>(true);
            if (Enum.GetValues(t).Length != NumBits)
                throw new ArgumentException(String.Format("{0} does not have as many flags as this BitSet has bits.", t.FullName));
            long value = 0;

            int max = NumBits - 1;
            for (int i = max; i >= 0; --i)
            {
                if (Bits[i])
                    value |= Convert.ToInt64(Math.Pow(2, max - i));
            }
            return (T)Enum.ToObject(t, value);
        }
    }
}
