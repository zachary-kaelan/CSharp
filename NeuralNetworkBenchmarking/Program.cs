using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworksLib;
using NeuralNetworksLib.NetworkModels;

namespace NeuralNetworkBenchmarking
{
    class Program
    {
        public const string SETS_PATH = @"E:\Programming\Neural Networks\MNIST\";

        static void Main(string[] args)
        {
            FileStream fs = new FileStream(SETS_PATH + "train-images.idx3-ubyte", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        }
    }
}
