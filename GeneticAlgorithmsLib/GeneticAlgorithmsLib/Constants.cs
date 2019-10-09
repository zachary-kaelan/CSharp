using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmsLib
{
    internal static class Constants
    {
        static Constants()
        {
            ALL_POWERS_OF_2_REVERSE = new List<ulong>(64);
            ALL_POWERS_OF_2_REVERSE.Add(1);
            ulong prev = 1;
            for (int i = 1; i < 64; ++i)
            {
                prev *= 2;
                ALL_POWERS_OF_2_REVERSE.Add(prev);
            }
        }

        public static readonly Random GEN = new Random();

        public static readonly List<byte> POWERS_OF_2 = new List<byte>() { 128, 64, 32, 16, 8, 4, 2, 1 };
        public static List<ulong> ALL_POWERS_OF_2_REVERSE { get; private set; }
        public static readonly List<byte> POWERS_OF_2_REVERSE = new List<byte>() { 1, 2, 4, 8, 16, 32, 64, 128 };
    }
}
