using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Statistics
{
    public struct CollectionStatistics : INormalizationHelper<double>
    {
        public double Maximum { get; set; }
        public double Minimum { get; set; }
        public double Mean { get; set; }
        public double[] Modes { get; set; }
        public double Median { get; set; }
        public double StandardDeviation { get; set; }
        public int Count { get; set; }
        public bool IsEnum
        {
            get
            {
                return false;
            }
        }
        public int NumSlots
        {
            get
            {
                return 1;
            }
        }

        /*public CollectionStatistics()
        {
            IsEnum = false;
            Maximum = 0;
            Minimum = 0;
            Mean = 0;
            Modes = null;
            Median = 0;
            StandardDeviation = 0;
            Count = 0;
        }*/

        public double Normalize(object value)
        {
            return (((double)value) - Mean) / StandardDeviation;
        }

        public object DeNormalize(double value)
        {
            return ((double)value * StandardDeviation) + Mean;
        }
    }

    public struct CorrelationHelper
    {
        public double Sum { get; private set; }
        public double SquaredSum { get; private set; }
        private double[] DataSet { get; set; }

        public CorrelationHelper(double[] values)
        {
            double sum = 0;
            double squaredSum = 0;
            int count = values.Length;

            for (int i = 0; i < count; ++i)
            {
                double val = values[i];
                sum += val;
                squaredSum += Math.Pow(val, 2.0);
            }

            Sum = sum;
            SquaredSum = squaredSum;
            DataSet = values;
        }

        public CorrelationHelper(double[] values, double sum, double squaredSum)
        {
            Sum = sum;
            SquaredSum = squaredSum;
            DataSet = values;
        }
    }

    public interface INormalizationHelper<D>
    {
        D Normalize(object obj);
        object DeNormalize(D value);
        bool IsEnum { get; }
        int NumSlots { get; }
    }

    public struct BoxWhiskerPlot
    {
        public double LowerQuartile { get; private set; }
        public double UpperQuartile { get; private set; }
        public double IQRange { get; private set; }
        public double LowerInnerFence { get; private set; }
        public double UpperInnerFence { get; private set; }
        public double Median { get; private set; }
        public double LowerOuterFence { get; private set; }
        public double UpperOuterFence { get; private set; }

        public double[] MildOutliers { get; private set; }
        public double[] ExtremeOutliers { get; private set; }

        public BoxWhiskerPlot(double[] dataset) : this()
        {
            Array.Sort(dataset);
            int count = dataset.Length;
            Median = dataset.Median();

            double dblTemp = 0.25 * (count + 1);
            double dblTemp2 = dblTemp % 1;
            int intTemp = Convert.ToInt32(dblTemp - dblTemp2) - 1;
            dblTemp = dblTemp2;

            LowerQuartile = dataset[intTemp];
            if (dblTemp > 0)
                LowerQuartile += dblTemp * (dataset[intTemp + 1] - LowerQuartile);

            dblTemp = 0.75 * (count + 1);
            dblTemp2 = dblTemp % 1;
            intTemp = Convert.ToInt32(dblTemp - dblTemp2) - 1;
            dblTemp = dblTemp2;

            UpperQuartile = dataset[intTemp];
            if (dblTemp > 0)
                UpperQuartile += dblTemp * (dataset[intTemp + 1] - UpperQuartile);

            IQRange = UpperQuartile - LowerQuartile;

            double change = 1.5 * IQRange;
            LowerInnerFence = LowerQuartile - change;
            UpperInnerFence = UpperQuartile + change;
            change += change;
            LowerOuterFence = LowerQuartile - change;
            UpperOuterFence = UpperQuartile + change;

            var copy = this;
            MildOutliers = dataset.Where(n => n < copy.LowerInnerFence || n > copy.UpperInnerFence).ToArray();
            ExtremeOutliers = MildOutliers.Where(n => n < copy.LowerOuterFence || n > copy.UpperOuterFence).ToArray();
        }
    }
}
