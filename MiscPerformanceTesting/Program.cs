using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MiscPerformanceTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch tst2 = new Stopwatch();
            Stopwatch tst = new Stopwatch();
            tst.Start();
            tst2.Start();
            tst.Stop();
            tst.Start();
            tst2.Stop();
            tst.Stop();
            Console.WriteLine(tst2.ElapsedMilliseconds);
            Console.WriteLine(tst.ElapsedMilliseconds);
            tst = null;
            tst2 = null;
            Console.WriteLine();
            GC.Collect();

            double tmp = 0;
            long memoryTracker = 0;
            double[] row = null;
            Console.WriteLine("Jagged");
            Stopwatch timer = new Stopwatch();
            GCLatencyMode oldMode = GCSettings.LatencyMode;
            memoryTracker = GC.GetTotalMemory(true);
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;

                memoryTracker = GC.GetTotalMemory(true);
                long firstSize = 0;
                timer.Start();
                // Initialization
                double[][] jagged = new double[1024][];
                /*timer.Stop();
                long firstSize = GC.GetTotalMemory(true) - memoryTracker;
                timer.Start();*/
                for (int i = 0; i < 1024; ++i)
                {
                    jagged[i] = new double[1024];
                }
                timer.Stop();
                long secondSize = GC.GetTotalMemory(true) - firstSize;
                Console.WriteLine("\tInitialization: " + timer.ElapsedMilliseconds + " ms");
                timer.Restart();
                // Setting
                for (int i = 0; i < 1024; ++i)
                {
                    for (int j = 0; j < 1024; ++j)
                    {
                        jagged[i][j] = 0;
                    }
                }
                timer.Stop();
                Console.WriteLine("\tSetting: " + timer.ElapsedMilliseconds + " ms");
                long thirdSize = GC.GetTotalMemory(true) - memoryTracker;
                row = new double[1024];
                timer.Restart();
                // Row Setting
                for (int i = 0; i < 1024; ++i)
                {
                    jagged[i] = row;
                }
                timer.Stop();
                Console.WriteLine("\tSetting Row: " + timer.ElapsedMilliseconds + " ms");
                timer.Restart();
                // Getting
                for (int i = 0; i < 1024; ++i)
                {
                    for (int j = 0; j < 1024; ++j)
                    {
                        tmp = jagged[i][j];
                    }
                }
                timer.Stop();
                Console.WriteLine("\tGetting: " + timer.ElapsedMilliseconds + " ms");
                // Getting Rows
                timer.Restart();
                for (int i = 0; i < 1024; ++i)
                {
                    row = jagged[i];
                }
                timer.Stop();
                Console.WriteLine("\tGetting Rows: " + timer.ElapsedMilliseconds + " ms");
                Console.WriteLine("\tInitial Size: " + firstSize + " bytes");
                Console.WriteLine("\tColumns Size: " + secondSize + " bytes");
                Console.WriteLine("\tValues Size: " + thirdSize + " bytes");
                Console.WriteLine();
                jagged = null;
                row = null;
            }
            finally
            {
                GCSettings.LatencyMode = oldMode;
            }
            
            GC.Collect();

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;
                Console.WriteLine("Flattened");
                memoryTracker = GC.GetTotalMemory(true);
                timer.Restart();
                // Initialization
                double[] flattened = new double[1024 * 1024];
                timer.Stop();
                Console.WriteLine("\tInitialization: " + timer.ElapsedMilliseconds + " ms");
                long firstSize = GC.GetTotalMemory(true) - memoryTracker;
                timer.Restart();
                // Setting All
                for (int e = 0; e < 1048576; ++e)
                {
                    flattened[e] = 0;
                }
                timer.Stop();
                Console.WriteLine("\tSetting All: " + timer.ElapsedMilliseconds + " ms");
                long thirdSize = GC.GetTotalMemory(true) - memoryTracker;
                // Setting By Index
                timer.Restart();
                for (int i = 0; i < 1024; ++i)
                {
                    for (int j = 0; j < 1024; ++j)
                    {
                        flattened[i * 1024 + j] = 0;
                    }
                }
                timer.Stop();
                Console.WriteLine("\tSetting Index: " + timer.ElapsedMilliseconds + " ms");
                row = new double[1024];
                timer.Restart();
                // Setting Rows
                for (int i = 0; i < 1024; ++i)
                {
                    int start = i * 1024;
                    int end = start + 1024;
                    int e = 0;
                    for (int j = start; j < end; ++j)
                    {
                        flattened[j] = row[e];
                    }
                }
                timer.Stop();
                Console.WriteLine("\tSetting Rows: " + timer.ElapsedMilliseconds + " ms");
                tmp = 0;
                timer.Restart();
                // Getting All
                for (int e = 0; e < 1048576; ++e)
                {
                    tmp = flattened[e];
                }
                timer.Stop();
                Console.WriteLine("\tGetting All: " + timer.ElapsedMilliseconds + " ms");
                timer.Restart();
                // Getting By Index
                for (int i = 0; i < 1024; ++i)
                {
                    for (int j = 0; j < 1024; ++j)
                    {
                        tmp = flattened[i * 1024 + j];
                    }
                }
                timer.Stop();
                Console.WriteLine("\tGetting Index: " + timer.ElapsedMilliseconds + " ms");
                timer.Restart();
                // Getting Rows
                for (int i = 0; i < 1024; ++i)
                {
                    int start = i * 1024;
                    for (int j = 0; j < 1024; ++j)
                    {
                        row[j] = flattened[start + j];
                    }
                }
                timer.Stop();
                Console.WriteLine("\tGetting Rows: " + timer.ElapsedMilliseconds + " ms");
                timer.Restart();
                // Getting Rows 2
                for (int i = 0; i < 1024; ++i)
                {
                    row = Enumerable.Range(i * 1024, 1024).Select(e => flattened[e]).ToArray();
                }
                timer.Stop();
                Console.WriteLine("\tGetting Rows 2: " + timer.ElapsedMilliseconds + " ms");

                Console.WriteLine("\tInitial Size: " + firstSize + " bytes");
                Console.WriteLine("\tValues Size: " + thirdSize + " bytes");
            }
            finally
            {
                GCSettings.LatencyMode = oldMode;
            }
            GC.Collect();
            Console.ReadLine();
        }
    }
}
