using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.Helpers
{
    public static class EnumHelper
    {
        private static readonly byte[] COMPLEMENTS = new byte[] {
            0, 8, 4, 12, 2,
            10, 9, 14, 1, 6,
            5, 13, 3, 11, 7, 15
        };

        public static Nucleotide GetComplement(this Nucleotide nt) =>
            (Nucleotide)COMPLEMENTS[(byte)nt];
    }
}
