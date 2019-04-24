using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NeuralNetworksLib;
using NeuralNetworksLib.NetworkModels;
using ZachLib;
using ZachLib.Statistics;

namespace NNConsoleTester
{
    class Program
    {
        public static NeuralNetwork network = null;
        public static Matrix inputs = null;
        public static Matrix outputs = null;

        static void Main(string[] args)
        {
            
            network = new NeuralNetwork(0.15, 0.5, 0.1, true, new int[] { 11, 8, 3 });
            inputs = Matrix.Deserialize(@"E:\Work Programming\Insight Program Files\Neural Network\AreaStats2.mtx");
            outputs = Matrix.Deserialize(@"E:\Work Programming\Insight Program Files\Neural Network\Outputs.mtx");
            RunNetwork();
            //TestOR();

            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        //private static string[] inputsStrings = inputs.ToArray().Select(i => String.Join(" | ", i) + " = ").ToArray();
        public static void Test()
        {
            SystemSounds.Beep.Play();
            Console.WriteLine("Running Tests...");
            Console.WriteLine("\t0 | 0 | 1 = " + network.Run(new Vector(inputs[0]))[0].ToString());
            Console.WriteLine("\t0 | 1 | 1 = " + network.Run(new Vector(inputs[1]))[0].ToString());
            Console.WriteLine("\t1 | 0 | 1 = " + network.Run(new Vector(inputs[2]))[0].ToString());
            Console.WriteLine("\t1 | 1 | 1 = " + network.Run(new Vector(inputs[3]))[0].ToString());
            Console.WriteLine();
            Thread.Sleep(5000);
        }

        private static void TestOR()
        {
            network = new NeuralNetwork(0.5, 0.2, 0, false, new int[] { 3, 4, 1 });
            inputs = new Matrix(
                new double[] {
                    0, 0, 1,
                    0, 1, 1,
                    1, 0, 1,
                    1, 1, 1
                }, 3
            );
            outputs = new Matrix(new double[] { 0, 1, 1, 0 }, 1);
            SystemSounds.Beep.Play();
            //Test();
            RunNetwork();
        }

        private static void RunNetwork()
        {
            var err = 0.0;
            var oldErr = 0.0;
            bool doBreak = false;
            for (int i = 0; i < 16; ++i)
            {
                var epochErr = 0.0;
                var oldEpochErr = 0.0;
                int endEpochs = i * 2500;
                for (int j = 0; j < 10; ++j)
                {
                    oldEpochErr = epochErr;
                    epochErr = network.Train(inputs, outputs, 250);
                    Console.WriteLine("\t" + (endEpochs + ((j + 1) * 250)).ToString() + " Epochs: " + epochErr);
                    doBreak = epochErr == 0;// || oldEpochErr == epochErr;
                    if (doBreak)
                        break;
                }
                oldErr = err;
                err = epochErr;
                Console.WriteLine("From {0} to {1} epochs: {2} -> {3}", endEpochs, endEpochs + 2500, oldErr.ToString("#.0000"), err.ToString("#.0000"));
                if (i % 50 == 0)
                    Test();
                if (doBreak)
                    break;
                else if (Math.Round(err, 4) == Math.Round(oldErr, 4))
                    break;
            }
        }
    }
}
