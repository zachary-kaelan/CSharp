using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashCracking
{
    class Program
    {
        private static readonly string[] HASHES = new string[]
        {
            "c2543238423484f4fb48ed84566f7054",
            "1b28425afae3620dcba519a07c693cba",
            "8ebba8abdad509495def29d10a31d582",
            "0e74ce59d64b0048fffa923855f1c73c",
            "28c25b045164d94c61235f99a11ba6b4",
            "9ed23c81e59bbab386afb9ce8a03ea96",
            "5f741f64c301e8840ccb98817787bd08",
            "2c88bf311f50ab9760650563ba779f5f",
            "b26fe413f1bfa2ae218e166e31fc5dba",
            "6d38ffd973ed9dcce696bf20ced5808f",
            "0c21ca1a37c8024ae71b2d0bd75dfa5a",
            "658a73c84e1e471725838d8dc4400787",
            "eb5c59a8e4b0a740e93424ff4572ab45",
            "99c4f85b6c584474b777457d2b2956de"
        };

        private static List<byte[]> HASH_BYTES = HASHES.Select(h => StringToByteArrayFastest(h)).ToList();
        private const ulong MIN_VALUE = 1530000000000;
        private const ulong MAX_VALUE = 1545000000000;

        static void Main(string[] args)
        {
            Console.WriteLine(GC.MaxGeneration);

            var count = MAX_VALUE - MIN_VALUE;

            ulong interval = 15000000; //1000000;//(count / 256) - 1;

            var md5 = MD5.Create();
            md5.Initialize();

            Stopwatch timer = Stopwatch.StartNew();
            ulong benchmarkIndex = 0;
            ulong GCIndex = 0;
            ulong totalCompleted = 0;

            Console.WriteLine(GC.GetTotalMemory(false));

            for (ulong i = MIN_VALUE; i < MAX_VALUE; ++i)
            {
                var bytes = BitConverter.GetBytes(i);
                var output = md5.ComputeHash(bytes);
                timer.Stop();
                for (int j = 0; j < HASH_BYTES.Count; ++j)
                {
                    if (HASH_BYTES[j].SequenceEqual(output))
                    {
                        Console.WriteLine(
                            String.Join(
                                "", HASH_BYTES[j].Select(
                                    b => Convert.ToString(b, 16).PadLeft(2, '0')
                                )
                            ) + " : " + String.Join(
                                "", output.Select(
                                    b => Convert.ToString(b, 16).PadLeft(2, '0')
                                )
                            )
                        );
                        HASH_BYTES.RemoveAt(j);
                    }
                }
                timer.Start();

                ++benchmarkIndex;
                if (benchmarkIndex == interval)
                {
                    timer.Stop();
                    totalCompleted += benchmarkIndex;
                    Console.WriteLine("{0} hashes/second\t\t{1} hashes completed", (benchmarkIndex / (timer.ElapsedMilliseconds / 1000.0)).ToString("#.0000"), totalCompleted);
                    benchmarkIndex = 0;
                    timer.Restart();
                }

                ++GCIndex;
                if (GCIndex == 1000000000)
                {
                    timer.Stop();
                    GCIndex = 0;
                    int gens = GC.MaxGeneration;
                    long before = GC.GetTotalMemory(false);
                    long total = GC.GetTotalMemory(true);
                    Console.WriteLine("{0}\t{1} -> {2}", gens, before, total);
                    timer.Start();
                }
            }

            Console.ReadLine();
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
