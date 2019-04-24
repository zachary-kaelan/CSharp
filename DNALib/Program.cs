using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace DNALib
{
    class Program
    {
        static void Main(string[] args)
        {
            /*MemoryCache.Default.Add(
                "complement", new byte[] {
                    0, 8, 4, 12, 2,
                    10, 9, 14, 1, 6,
                    5, 13, 3, 11, 7, 15
                }, new CacheItemPolicy() { Priority = CacheItemPriority.NotRemovable }
            );*/

            

            Stopwatch timer = Stopwatch.StartNew();
            Nucleotide complement = Nucleotide.Zero;
            for (int i = 0; i < 1000; ++i)
            {
                for (byte j = 0; j < 16; ++j)
                {
                    //complement = (byte)Constants.COMPLEMENTS[(Nucleotide)j];
                    //complement = ((byte[])MemoryCache.Default.Get("complement"))[j];
                    complement = (Nucleotide)bytes[j];
                }
            }
            timer.Stop();
            Console.WriteLine(timer.Elapsed);
            Console.WriteLine(timer.ElapsedMilliseconds);
            Console.WriteLine(timer.ElapsedTicks);
            Console.ReadLine();
        }
    }
}
