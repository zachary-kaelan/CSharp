using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace DnDLib.Tools
{
    public static class DiceRoll
    {
        private static readonly ByteGenerator _d4Gen = new ByteGenerator(4);
        private static readonly ByteGenerator _d6Gen = new ByteGenerator(6);
        private static readonly ByteGenerator _d8Gen = new ByteGenerator(8);
        private static readonly ByteGenerator _d10Gen = new ByteGenerator(10);
        private static readonly ByteGenerator _d12Gen = new ByteGenerator(12);
        private static readonly ByteGenerator _d20Gen = new ByteGenerator(20);

        public static byte d4() => _d4Gen.Next();
        public static byte d4(byte count)
        {
            byte total = 0;
            for (byte i = 0; i < count; ++i)
            {
                total += _d4Gen.Next();
            }
            return total;
        }

        public static byte d6() => _d6Gen.Next();
        public static byte d6(byte count)
        {
            byte total = 0;
            for (byte i = 0; i < count; ++i)
            {
                total += _d6Gen.Next();
            }
            return total;
        }

        public static byte d8() => _d8Gen.Next();
        public static byte d8(byte count)
        {
            byte total = 0;
            for (byte i = 0; i < count; ++i)
            {
                total += _d8Gen.Next();
            }
            return total;
        }

        public static byte d10() => _d10Gen.Next();
        public static byte d10(byte count)
        {
            byte total = 0;
            for (byte i = 0; i < count; ++i)
            {
                total += _d10Gen.Next();
            }
            return total;
        }

        public static byte d12() => _d12Gen.Next();
        public static byte d12(byte count)
        {
            byte total = 0;
            for (byte i = 0; i < count; ++i)
            {
                total += _d12Gen.Next();
            }
            return total;
        }

        public static byte d20() => _d20Gen.Next();
        public static byte d20(byte count)
        {
            byte total = 0;
            for (byte i = 0; i < count; ++i)
            {
                total += _d20Gen.Next();
            }
            return total;
        }
    }
}
