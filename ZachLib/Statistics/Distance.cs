using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Statistics
{
    public static class Distance
    {
        public static double DistanceTo(this double dbl, double other)
        {
            return Math.Abs(dbl - other);
        }

        private static double SampleDistanceCovariance(double[] inputs, double[] outputs)
        {
            int length = inputs.Length;
            Matrix inputsDist = new Matrix(length);
            double[] xRowMeans = new double[length];
            double[] xColMeans = new double[length];
            Matrix outputsDist = new Matrix(length);
            double[] yRowMeans = new double[length];
            double[] yColMeans = new double[length];
            for (int i = 0; i < length - 1; ++i)
            {
                double x = inputs[i];
                double y = outputs[i];
                for (int j = i + 1; j < length; ++j)
                {
                    double dist = x.DistanceTo(inputs[j]);
                    inputsDist[i, j] = dist;
                    inputsDist[j, i] = dist;
                    xRowMeans[i] += dist;
                    xColMeans[j] += dist;

                    dist = y.DistanceTo(outputs[j]);
                    outputsDist[i, j] = dist;
                    outputsDist[j, i] = dist;
                    yRowMeans[i] += dist;
                    yColMeans[j] += dist;
                }
            }

            double xGrandMean = 0;
            double yGrandMean = 0;
            for (int i = 0; i < length; ++i)
            {
                double xMean = xRowMeans[i] / length;
                xGrandMean += xMean;
                xRowMeans[i] = xMean;
                xColMeans[i] /= length;

                double yMean = yRowMeans[i] / length;
                yGrandMean += yMean;
                yRowMeans[i] = yMean;
                yColMeans[i] /= length;
            }
            xGrandMean /= length;
            yGrandMean /= length;

            Matrix inputsCentered = new Matrix(length);
            Matrix outputsCentered = new Matrix(length);
            double distCovar = 0;
            for (int j = 0; j < length; ++j)
            {
                double[] xRow = inputsDist[j];
                double[] yRow = outputsDist[j];
                double xRowMean = xRowMeans[j];
                double yRowMean = yRowMeans[j];
                for (int k = 0; k < length; ++k)
                {
                    var xCentered = (xRow[k] - (xRowMean + xColMeans[k])) + xGrandMean;
                    var yCentered = (yRow[k] - (yRowMean + yColMeans[k])) + yGrandMean;
                    inputsCentered[j, k] = xCentered;
                    outputsCentered[j, k] = yCentered;
                    distCovar += (xCentered * yCentered);
                }
            }

            return Math.Sqrt(distCovar / (length * length));
        }

        public static double SampleDistanceCorrelationTo(this double[] inputs, double[] outputs)
        {
            return SampleDistanceCovariance(inputs, outputs) / Math.Sqrt(
                SampleDistanceCovariance(inputs, inputs) * SampleDistanceCovariance(outputs, outputs)
            );
        }
    }
}
