using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworksLib.NetworkModels
{
    internal class NeuronBackfiredEventArgs : EventArgs
    {
        public double[] Weights { get; private set; }
        public double Delta { get; private set; }
    }
}
