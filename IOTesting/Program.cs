using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lin = MathNet.Numerics.LinearAlgebra;
using dbl = MathNet.Numerics.LinearAlgebra.Double;
using ZachLib;
using ZachLib.Statistics;

namespace IOTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            /*Console.WriteLine(Stopwatch.Frequency);
            Console.WriteLine(Stopwatch.IsHighResolution);
            Console.WriteLine(Stopwatch.GetTimestamp());
            DateTime now = new DateTime();
            long totalMS = 0;
            long totalTicks = 0;
            long[] msHistory = new long[10000];
            long[] ticksHistory = new long[10000];
            for (int i = 0; i < 10000; ++i)
            {
                timer.Restart();
                now = DateTime.Now;
                timer.Stop();
                totalMS += timer.ElapsedMilliseconds;
                totalTicks += timer.ElapsedTicks;
                msHistory[i] = timer.ElapsedMilliseconds;
                ticksHistory[i] = timer.ElapsedTicks;
                System.Threading.Thread.Sleep(20);
            }
            Console.WriteLine(Stopwatch.GetTimestamp());
            Console.WriteLine("{0} - {1}", msHistory.Average(), ticksHistory.Average());
            Console.ReadLine();*/

            //TestMatrix();
            //Console.ReadLine();

            var matrix = Matrix.Deserialize(@"E:\Work Programming\Insight Program Files\Neural Network\AreaStats2.mtx");
            lin.Matrix<double> matrix2 = null;
            var compacted = new double[matrix.Count];
            Array.Copy(matrix._compactedMatrix, compacted, matrix.Count);
            int numCols = matrix.NumCols;
            int numRows = matrix.NumRows;
            var storage = lin.Storage.SparseCompressedRowMatrixStorage<double>.OfRowArrays(matrix.ToArray());

            Console.WriteLine("Compacted Initialization:");
            long matrixMS = 0;
            long matrixTicks = 0;
            long matrix2MS = 0;
            long matrix2Ticks = 0;
            for (int i = 0; i < 1024; ++i)
            {
                matrix = null;
                timer.Restart();
                matrix = new Matrix(compacted, numCols);
                timer.Stop();
                matrixMS += timer.ElapsedMilliseconds;
                matrixTicks += timer.ElapsedTicks;

                matrix2 = null;
                timer.Restart();
                matrix2 = new dbl.SparseMatrix(storage);
                timer.Stop();
                matrix2MS += timer.ElapsedMilliseconds;
                matrix2Ticks += timer.ElapsedTicks;
            }
            Console.WriteLine("\tMatrix1: {0} - {1}", matrixMS, matrixTicks);
            Console.WriteLine("\tMatrix2: {0} - {1}", matrix2MS, matrix2Ticks);

            matrixMS = 0;
            matrixTicks = 0;
            matrix2MS = 0;
            matrix2Ticks = 0;
            Console.WriteLine("Transposition: ");
            for (int i = 0; i < 1024; ++i)
            {
                timer.Restart();
                matrix = matrix.T();
                timer.Stop();
                matrixMS += timer.ElapsedMilliseconds;
                matrixTicks += timer.ElapsedTicks;

                timer.Restart();
                matrix2 = matrix2.Transpose();
                timer.Stop();
                matrix2MS += timer.ElapsedMilliseconds;
                matrix2Ticks += timer.ElapsedTicks;
            }

            Console.WriteLine("\tMatrix1: {0} - {1}", matrixMS, matrixTicks);
            Console.WriteLine("\tMatrix2: {0} - {1}", matrix2MS, matrix2Ticks);

            matrixMS = 0;
            matrixTicks = 0;
            matrix2MS = 0;
            matrix2Ticks = 0;

            Console.ReadLine();

            /*long loadFileSumMS = 0;
            long loadFileSumTicks = 0;
            long serializeFileSumMS = 0;
            long serializeFileSumTicks = 0;

            Matrix matrix = null;
            
            Console.WriteLine("Human-readable:");

            for (int i = 0; i < 1024; ++i)
            {
                timer.Restart();
                matrix = Matrix.LoadFile(@"E:\Insight Temp Files\Lists\AreaStats.mtx");
                timer.Stop();
                loadFileSumMS += timer.ElapsedMilliseconds;
                loadFileSumTicks += timer.ElapsedTicks;
                timer.Restart();
                matrix.SerializeToFile(@"E:\Insight Program Files\Neural Network", "AreaStats");
                timer.Stop();
                matrix = null;
                serializeFileSumMS += timer.ElapsedMilliseconds;
                serializeFileSumTicks += timer.ElapsedTicks;
            }

            Console.WriteLine("\tLoad File: {0} - {1}", loadFileSumMS / 1024, loadFileSumTicks / 1024);
            Console.WriteLine("\tSerialize to File:  {0} - {1}", serializeFileSumMS / 1024, serializeFileSumTicks/ 1024);
            GC.Collect();
            loadFileSumMS = 0;
            loadFileSumTicks = 0;
            serializeFileSumMS = 0;
            serializeFileSumTicks = 0;

            Console.WriteLine();
            Console.WriteLine("Binary format:");
            matrix = Matrix.LoadFile(@"E:\Insight Temp Files\Lists\AreaStats.mtx");
            
            for (int i = 0; i < 1024; ++i)
            {
                timer.Restart();
                matrix.SerializeToFile2(@"E:\Insight Program Files\Neural Network", "AreaStats2");
                timer.Stop();
                serializeFileSumMS += timer.ElapsedMilliseconds;
                serializeFileSumTicks += timer.ElapsedTicks;
                matrix = null;
                timer.Restart();
                matrix = Matrix.LoadFile2(@"E:\Insight Program Files\Neural Network\AreaStats2.mtx");
                timer.Stop();
                loadFileSumMS += timer.ElapsedMilliseconds;
                loadFileSumTicks += timer.ElapsedTicks;
            }

            Console.WriteLine("\tLoad File:  {0} - {1}", loadFileSumMS / 1024, loadFileSumTicks / 1024);
            Console.WriteLine("\tSerialize to File:  {0} - {1}", serializeFileSumMS / 1024, serializeFileSumTicks / 1024);

            Console.ReadLine();*/
        }

        public static void TestMatrix()
        {
            long matrixMS = 0;
            long matrixTicks = 0;
            long[] MS = new long[1024];
            long[] ticks = new long[1024];

            Stopwatch timer = new Stopwatch();
            var matrix = Matrix.Deserialize(@"E:\Work Programming\Insight Program Files\Neural Network\AreaStats2.mtx");

            for (int i = 0; i < 1024; ++i)
            {
                timer.Restart();
                matrix.Serialize(@"E:\Work Programming\Insight Program Files\Neural Network", "AreaStats2");
                timer.Stop();
                matrixMS += timer.ElapsedMilliseconds;
                matrixTicks += timer.ElapsedTicks;
                MS[i] = timer.ElapsedMilliseconds;
                ticks[i] = timer.ElapsedTicks;
            }

            Console.WriteLine("Serialization: {0} - {1}", matrixMS / 1024, matrixTicks / 1024);
        }
    }
}
