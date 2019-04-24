using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworksLib.Templates;
using ZachLib.Statistics;

namespace NeuralNetworksLib.NetworkModels
{
    internal class ConvolutionalLayer
    {
        private double[] Kernel { get; set; }
        private int KernelSide { get; set; }
        private Matrix InputValues { get; set; }
        private NetworkParameters Parameters { get; set; }

        public ConvolutionalLayer(int inputRows, int inputCols, int kernelRows, int kernelCols)
        {
            var hiddenSize = (
                (inputRows - kernelRows) *
                (inputCols - kernelCols)
            );
            int kernelSize = kernelRows * kernelCols;
            Kernel = Enumerable.Repeat(0, kernelSize).Select(i => NNFunctions.GetRandom()).ToArray();
        }

        public Matrix 
            
            ForwardPropogate(Matrix inputs)
        {
            InputValues = inputs;
            int iStart = KernelSide / 2;
            int iEnd = inputs.NumCols - iStart;
            int jEnd = inputs.NumRows - iStart;
            Matrix outputs = new Matrix(jEnd - iStart, iEnd - iStart);

            int kIStart = 0;
            int kJStart = 0;
            int kIEnd = KernelSide;
            int kJEnd = KernelSide;

            int outputsI = 0;
            int outputsJ = 0;

            for (int inputsI = iStart; inputsI < iEnd; ++inputsI)
            {
                for (int inputsJ = iStart; inputsJ < jEnd; ++inputsJ)
                {
                    // iterating through the section of the image,
                    // applying the kernel to it
                    int kernelIndex = 0;
                    double total = 0;
                    for (int kI = kIStart; kI < kIEnd; ++kI)
                    {
                        for (int kJ = kJStart; kJ < kJEnd; ++kJ)
                        {
                            // multiplying the pixel by the kernel weight
                            if (NNFunctions.GetRandom(false) > Parameters.DropoutRate)
                                total += Kernel[kernelIndex] * inputs[kI, kJ];
                            ++kernelIndex;
                        }
                    }

                    outputs[outputsI, outputsJ] = Math.Tanh(total * Parameters.DropoutMultiplier);

                    ++outputsJ;
                    ++kJStart;
                    ++kJEnd;
                }

                ++outputsI;
                ++kIStart;
                ++kIEnd;

                outputsJ = 0;
                kJStart = 0;
                kJEnd = KernelSide;
            }

            return null;
        }
    }
}
