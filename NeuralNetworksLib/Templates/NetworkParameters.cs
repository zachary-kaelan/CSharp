using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworksLib.Templates
{
    public class NetworkParameters
    {
        public double LearningRate { get; protected set; }
        public double Momentum { get; protected set; }
        public double DropoutRate { get; protected set; }
        public double DropoutMultiplier { get; protected set; }
        public int NumIterations { get; protected set; }
        internal NetworkOptions Options { get; private set; }

        public NetworkParameters(double learningRate, double dropoutRate, double momentum, bool doBiases, int numIterations = 0)
        {
            Options = NetworkOptions.None;
            if (learningRate > 0 && learningRate != 1)
            {
                LearningRate = learningRate;
                Options |= NetworkOptions.LearningRate;
            }

            if (dropoutRate > 0)
            {
                DropoutRate = dropoutRate;
                DropoutMultiplier = 1.0 / (1 - DropoutRate);
                Options |= NetworkOptions.Dropout;
            }

            if (momentum > 0)
            {
                Momentum = momentum;
                Options |= NetworkOptions.Momentum;
            }

            if (doBiases)
                Options |= NetworkOptions.Biases;

            NumIterations = numIterations;
        }

        public NetworkParameters(NetworkParameters parameters)
        {
            Options = parameters.Options;
            LearningRate = parameters.LearningRate;
            DropoutRate = parameters.DropoutRate;
            DropoutMultiplier = parameters.DropoutMultiplier;
            Momentum = parameters.Momentum;
            NumIterations = parameters.NumIterations;
        }
    }
}
