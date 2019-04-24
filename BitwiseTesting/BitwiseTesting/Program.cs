using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitwiseTesting
{
    class Program
    {
        private static readonly IEnumerable<byte> POWERS_OF_2 = new byte[] { 128, 64, 32, 16, 8, 4, 2, 1 };

        static void Main(string[] args)
        {
            const int N = 1000000;

            Console.WriteLine("Testing with {0} operations:", N);


            Thread.Sleep(5000);
            Console.WriteLine(GC.TryStartNoGCRegion(4000000));
            Console.WriteLine("   Optimized bitfield took {0} ms.", TestOptimized(N));
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(1);
            Console.WriteLine("   A byte bitfield took {0} ms.", TestBitField(N));
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(1);
            Console.WriteLine("   Bit getting took {0} ms.", TestGetBit(N));
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(1);
            Console.WriteLine("   Bit setting took {0} ms.", TestSetBit(N));
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(1);
            Console.WriteLine("   BitArray took {0} ms.", TestBitArray(N));
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(1);
            Console.WriteLine("   Bool array initialization took {0} ms.", TestBoolArrayInitialization(N));
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(1);
            Console.WriteLine("   Bool array reading took {0} ms.", TestBoolArrayReading(N));
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(1);
            GC.EndNoGCRegion();

            Console.ReadLine();
        }

        public static long TestOptimized(int n)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; ++i)
            {
                for (byte bits = 0; bits <= 255; ++bits)
                {
                    var enumerator = POWERS_OF_2.GetEnumerator();
                    byte temp = bits;
                    byte result = 0;
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        enumerator.MoveNext();
                        if (temp > enumerator.Current)
                        {
                            temp -= enumerator.Current;
                            result += enumerator.Current;
                        }
                        else if (temp < enumerator.Current)
                        {
                            if (temp == enumerator.Current - 1)
                            {
                                result += enumerator.Current;
                                --result;
                                break;
                            }
                        }
                        else
                        {
                            result += enumerator.Current;
                            break;
                        }
                    }
                    enumerator.Dispose();
                    if (bits == 255)
                        break;
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds / 255;
        }

        static long TestBitField(int n)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                for (byte bits = 0; bits <= 255; ++bits)
                {
                    byte result = 0;
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        if (GetBit(bits, bit))
                            SetBit(ref result, bit, true);
                    }
                    if (bits == 255)
                        break;
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds / 255;
        }

        static long TestGetBit(int n)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                for (byte bits = 0; bits <= 255; ++bits)
                {
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        bool b = GetBit(bits, bit);
                    }
                    if (bits == 255)
                        break;
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds / 255;
        }

        static long TestSetBit(int n)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                for (byte bits = 0; bits <= 255; ++bits)
                {
                    byte result = 0;
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        SetBit(ref result, bit, true);
                    }
                    if (bits == 255)
                        break;
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds / 255;
        }

        static bool GetBit(byte x, int bitnum) => 
            (x & (1 << bitnum)) != 0;

        static void SetBit(ref byte x, int bitnum, bool val)
        {
            if (val)
                x |= (byte)(1 << bitnum);
            else
                x &= (byte)(~(byte)(1 << bitnum));
        }

        static long TestBitArray(int n)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                for (byte bits = 0; bits <= 255; ++bits)
                {
                    BitArray result = new BitArray(8, false);
                    BitArray array = new BitArray(new byte[] { bits });
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        if (array.Get(bit))
                            result.Set(bit, true);
                    }
                    if (bits == 255)
                        break;
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds / 255;
        }

        static long TestBoolArrayInitialization(int n)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                for (byte bits = 0; bits <= 255; ++bits)
                {
                    byte temp = bits;
                    bool[] ba = new bool[8];
                    byte bitValue = 128;
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        if (temp > bitValue)
                        {
                            temp -= bitValue;
                            ba[bit] = true;
                        }
                        bitValue /= 2;
                    }
                    if (bits == 255)
                        break;
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds / 255;
        }

        static long TestBoolArrayReading(int n)
        {
            bool[][] arrays = new bool[256][];
            for (byte bits = 0; bits <= 255; ++bits)
            {
                byte temp = bits;
                bool[] ba = new bool[8];
                byte bitValue = 128;
                for (int bit = 0; bit < 8; ++bit)
                {
                    if (temp > bitValue)
                    {
                        temp -= bitValue;
                        ba[bit] = true;
                    }
                    bitValue /= 2;
                }
                arrays[bits] = ba;
                if (bits == 255)
                    break;
            }

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                for (byte bits = 0; bits <= 255; ++bits)
                {
                    bool[] ba = arrays[bits];
                    byte bitValue = 128;
                    byte result = 0;
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        if (ba[bit])
                            result += bitValue;
                        bitValue /= 2;
                    }
                    if (bits == 255)
                        break;
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds / 255;
        }
    }
}
