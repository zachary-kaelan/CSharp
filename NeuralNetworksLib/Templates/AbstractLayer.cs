using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworksLib.NetworkModels;
using ZachLib.Statistics;

namespace NeuralNetworksLib.Templates
{
    [Flags]
    public enum NetworkOptions
    {
        None = 0,
        LearningRate = 1,
        Momentum = 2,
        Biases = 4,
        Dropout = 8
    }

    class AbstractLayer
    {
        public void DisableDropout()
        {
            
        }
    }
}
