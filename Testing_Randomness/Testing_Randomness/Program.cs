using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing_Randomness
{
    class Program
    {
        static void Main(string[] args)
        {
            TestGenTime();
            Console.ReadLine();

            Random gen = null;
            Stopwatch timer = Stopwatch.StartNew();
            for (int i = 0; i < Int16.MaxValue; ++i)
            {
                gen = new Random();
            }
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);

            timer.Restart();
            byte[] buffer = new byte[16];
            for (int i = 0; i < Int16.MaxValue; ++i)
            {
                gen.NextBytes(buffer);
            }
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);

            timer.Restart();
            for (int iteration = 0; iteration < Int16.MaxValue; ++iteration)
            {
                int index = 0;
                for (int i = 0; i < 4; ++i)
                {
                    var bytes = BitConverter.GetBytes(gen.Next());
                    for (int b = 0; b < 4; ++b)
                    {
                        buffer[index] = bytes[b];
                        ++index;
                    }
                }
            }
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);

            Console.ReadLine();
        }

        public static void TestGenTime()
        {
            Random GEN = new Random();
            double mem = 0;
            Stopwatch timer = Stopwatch.StartNew();
            for (int i = 0; i < Int16.MaxValue; ++i)
            {
                mem = GEN.NextDouble();
            }
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);
            Console.WriteLine(timer.ElapsedMilliseconds / Int16.MaxValue);
        }
    }
}
