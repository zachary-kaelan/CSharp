using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib
{
    public static class EncodingHelper
    {
        public static byte Encode(byte firstBase, byte secondBase) =>
            Convert.ToByte(firstBase + (0x01 * secondBase));

        public static byte Encode(Nucleotide firstBase, Nucleotide secondBase) =>
            Encode((byte)firstBase, (byte)secondBase);

        public static (byte, byte) Encode(byte firstBase, byte secondBase, byte thirdBase, byte otherInfo) =>
            (Encode(firstBase, secondBase), Encode(thirdBase, otherInfo));

        public static (byte, byte) Encode(Nucleotide firstBase, Nucleotide secondBase, Nucleotide thirdBase, byte otherInfo) =>
            (Encode((byte)firstBase, (byte)secondBase), Encode((byte)thirdBase, otherInfo));

    }
}
