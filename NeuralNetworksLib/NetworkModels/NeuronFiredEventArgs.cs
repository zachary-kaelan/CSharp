using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworksLib.NetworkModels
{
    internal class NeuronFiredEventArgs : EventArgs
    {
        public double Value { get; private set; }
        public int LayerIndex { get; private set; }

        public NeuronFiredEventArgs(double val, int index)
        {
            Value = val;
            LayerIndex = index;
        }
    }
}
