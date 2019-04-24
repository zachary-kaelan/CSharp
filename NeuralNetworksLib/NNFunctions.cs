using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Redzen.Random;
using Redzen.Random.Double;

namespace NeuralNetworksLib
{
    public static class NNFunctions
    {
        static NNFunctions()
        {
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                DateTime now = DateTime.Now;
                ZIGGURAT = new ZigguratGaussianDistribution(provider.GetHashCode() ^ now.TimeOfDay.Milliseconds ^ (int)now.TimeOfDay.TotalMilliseconds, 0, 0.01);
            }
        }

        /*private static ConcurrentBag<double> RANDOM_BAG = new ConcurrentBag<double>();
        public static double GetRandom(bool isWeight = true)
        {
            if (!RANDOM_BAG.TryTake(out double result))
            {
                FillBag();
                double random = GetRandom();
                return isWeight ? Round((random * 2) - 1) : random;
            }
            return result;
        }

        private static void FillBag()
        {
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                DateTime now = DateTime.Now;
                Random random = new Random(provider.GetHashCode() ^ now.TimeOfDay.Milliseconds ^ (int)now.TimeOfDay.TotalMilliseconds ^ Convert.ToInt32(DriveInfo.GetDrives().Sum(d => d.AvailableFreeSpace) / 1000));
                for (int i = 0; i < 25; ++i)
                {
                    RANDOM_BAG.Add(random.NextDouble());
                }

            }
        }*/

        private static readonly int PROCESS_ID = Process.GetCurrentProcess().Id;
        private const int ROUNDING_DECIMAL_PLACES = 6;
        private static ZigguratGaussianDistribution ZIGGURAT { get; set; }
        private static Random DROPOUT_GEN = new Random(321837821);

        public static double Round(double num)
        {
            return Math.Round(num, ROUNDING_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
        }

        public static double Bent(double x, bool deriv = false)
        {
            return Round(
                !deriv ?
                    ((Math.Sqrt((x * x) + 1.0) - 1.0) / 2.0) + x :
                    (x / (2.0 * Math.Sqrt((x * x) + 1.0))) + 1.0
            );
        }

        public static double Sigmoid(double x, bool deriv = false)
        {
            double sig = 1.0 / (1.0 + Math.Exp(-x));
            return Round(
                !deriv ? sig : sig * (1 - sig)
            );
            /*var y = 1.0 / (1.0 + Math.Exp(-x));
            if (deriv)
                y *= (1.0 - y);
            return Round(y);*/
        }

        public static double Linear(double x, bool deriv = false)
        {
            return Round(!deriv ? x : 1.0);
        }

        public static double Gaussian(double x, bool deriv = false)
        {
            var y = Math.Exp(-(x * x));
            if (deriv)
                y *= x * -2;
            return Round(y);
        }

        public static double TanH(double x, bool deriv = false)
        {
            var y = Math.Tanh(x);
            if (deriv)
                y = 1.0 - (y * y);
            return Round(y);
        }

        public static double GetRandom(bool isWeight = true)
        {
            return isWeight ?
                    Round(ZIGGURAT.Sample()) :
                    DROPOUT_GEN.NextDouble(); //Math.Abs(ZIGGURAT.Sample() * 2);
        }

        public static double DerivativeCost(double predictedOutput, double expectedOutput)
        {
            return Round((expectedOutput - predictedOutput) * Bent(predictedOutput, true));
        }

        public static Pen CreatePenFromWeight(double weight, float zoomFactor)
        {
            int red = 127;
            int green = 128;
            var colorFactor = Convert.ToInt32(127.5 + (127.5 * weight));
            if (weight > 0)
            {
                green = colorFactor;
                red = 255 - colorFactor;
            }
            else if (weight < 0)
            {
                red = colorFactor;
                green = 255 - colorFactor;
            }

            return new Pen(
                Color.FromArgb(red, green, 0),
                Convert.ToSingle(1.5 + (3 * Math.Sqrt(Math.Abs(weight))))
            );
        }
    }
}
