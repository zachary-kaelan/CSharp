using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Statistics
{
    public static class StatisticsExtensions
    {
        internal static double PearsonCorrelationFormula(double rowCount, double xSum, double ySum, double xSquaredSum, double ySquaredSum, double xySum)
        {
            return Math.Round(
                (
                    (rowCount* xySum) -
                        (xSum * ySum)
                ) / (
                    Math.Sqrt(
                        (rowCount * xSquaredSum) -
                            Math.Pow(xSum, 2.0)
                    ) * Math.Sqrt(
                        (rowCount * ySquaredSum) -
                            Math.Pow(ySum, 2.0)
                    )
                ), 4, MidpointRounding.AwayFromZero
            );
        }

        #region GetPearsonCorrelation
        public static double GetPearsonCorrelation(this double[] xValues, double[] yValues)
        {
            double xSum = 0;
            double ySum = 0;
            double xSquaredSum = 0;
            double ySquaredSum = 0;
            double xySum = 0;
            int count = xValues.Length;
            double xyCount = Convert.ToDouble(count);

            for (int i = 0; i < count; ++i)
            {
                double x = xValues[i];
                double y = yValues[i];
                xSum += x;
                ySum += y;
                xSquaredSum += Math.Pow(x, 2.0);
                ySquaredSum += Math.Pow(y, 2.0);
                xySum += x * y;
            }

            return PearsonCorrelationFormula(count, xSum, ySum, xSquaredSum, ySquaredSum, xySum);
        }

        public static double GetPearsonCorrelation(this IEnumerable<double> xValues, IEnumerable<double> yValues) => xValues.ToArray().GetPearsonCorrelation(yValues.ToArray());

        public static double GetPearsonCorrelation(this IEnumerable<KeyValuePair<double, double>> xyValues)
        {
            var arr = xyValues.ToArray();
            double xSum = 0;
            double ySum = 0;
            double xSquaredSum = 0;
            double ySquaredSum = 0;
            double xySum = 0;
            int count = arr.Length;
            double xyCount = Convert.ToDouble(count);

            for (int i = 0; i < count; ++i)
            {
                var xy = arr[i];
                xSum += xy.Key;
                ySum += xy.Value;
                xSquaredSum += Math.Pow(xy.Key, 2.0);
                ySquaredSum += Math.Pow(xy.Value, 2.0);
                xySum += xy.Key * xy.Value;
            }

            return Math.Round(
                (
                    (xyCount * xySum) -
                        (xSum * ySum)
                ) / (
                    Math.Pow(
                        (xyCount * xSquaredSum) -
                            Math.Pow(xSum, 2.0),
                        0.5
                    ) * Math.Pow(
                        (xyCount * ySquaredSum) -
                            Math.Pow(ySum, 2.0),
                        0.5
                    )
                ), 4, MidpointRounding.AwayFromZero
            );
        }
        #endregion

        public static Dictionary<string, double> GetPearsonCorrelation(this IEnumerable<KeyValuePair<string, double>> xyvalues)
        {
            var categories = xyvalues.ToLookup();
            var keys = categories.GetKeys();
            Dictionary<string, double> output = new Dictionary<string, double>();

            foreach(var name in keys)
            {
                output.Add(
                    name,
                    categories.Where(c => c.Key != name).SelectMany(
                        c => c.Select(o => new KeyValuePair<double, double>(0.0, o))
                    ).Concat(
                        categories[name].Select(o => new KeyValuePair<double, double>(1.0, o))
                    ).GetPearsonCorrelation()
                );
            }
            categories = null;
            keys = null;
            return output;
        }

        #region EncodeEnum
        public static Matrix EncodeEnum(this string[] xValues) => xValues.EncodeEnum(out _);

        public static Matrix EncodeEnum(this string[] xValues, out string[] namesOrder)
        {
            int count = xValues.Length;
            var names = xValues.Distinct().ToArray();
            Array.Sort(names);
            namesOrder = names;
            int distinct = names.Length;

            Matrix mat = new Matrix(count, distinct);
            for (int i = 0; i < count; ++i)
            {
                mat[i, Array.BinarySearch(names, xValues[i])] = 1.0;
            }
            return mat;
        }

        public static Matrix EncodeEnum(this string[] xValues, string leastInterestName) => xValues.EncodeEnum(leastInterestName, out _);

        public static Matrix EncodeEnum(this string[] xValues, string leastInterestName, out string[] namesOrder)
        {
            int count = xValues.Length;
            var names = xValues.Distinct().ToArray();
            Array.Sort(names);
            int distinct = names.Length;

            --distinct;
            Matrix mat = new Matrix(count, distinct);
            names = names.Where(n => n != leastInterestName).ToArray();
            Array.Sort(names);
            namesOrder = names.Append(leastInterestName).ToArray();
            double[] leastInterestEncoded = Enumerable.Repeat<double>(-1.0, distinct).ToArray();
            for (int i = 0; i < count; ++i)
            {
                string name = xValues[i];
                if (name == leastInterestName)
                    mat[i] = leastInterestEncoded;
                else
                    mat[i, Array.BinarySearch(names, name)] = 1.0;
            }
            return mat;
        }
        #endregion

        #region GetEnumPearsonCorrelation
        /*public static Dictionary<string, double> GetPearsonCorrelation(this string[] xValues, double[] yValues)
        {
            var names = xValues.Distinct().ToArray();
            int count = xValues.Length;
            Dictionary<string, double> output = new Dictionary<string, double>(names.Length);

            foreach(var name in names)
            {
                double[] xNumValues = new double[count];
                for (int i = 0; i < count; ++i)
                {
                    xNumValues[i] = xValues[i] == name ? 1.0 : 0.0;
                }
                
            }
        }*/

        public static Dictionary<string, double[]> GetPearsonCorrelations(this IEnumerable<KeyValuePair<string, double[]>> xyvalues)
        {
            var categories = xyvalues.GroupBy(
                kv => kv.Key,
                kv => kv.Value,
                (k, g) => new KeyValuePair<string, double[][]>(k, g.ToArray())
            ).ToDictionary();
            Dictionary<string, double[]> output = new Dictionary<string, double[]>();

            foreach(var name in categories.Keys)
            {
                output.Add(
                    name,
                    categories.Where(c => c.Key != name).SelectMany(
                        c => c.Value.Select(o => o.Select(n => new KeyValuePair<double, double>(0.0, n)).ToArray())
                    ).Concat(
                        categories[name].Select(o => o.Select(n => new KeyValuePair<double, double>(1.0, n)).ToArray())
                    ).ToArray().T().Select(f => f.AsEnumerable().GetPearsonCorrelation()).ToArray()
                );
            }

            return output;
        }
        #endregion

        public static CollectionStatistics GetStatistics(this IEnumerable<double> set, bool isSample = true)
        {
            set = set.OrderBy();
            CollectionStatistics stats = new CollectionStatistics()
            {
                Count = set.Count(),
                Minimum = set.First(),
                Maximum = set.Last()
            };
            var midpoint = stats.Count / 2;
            if (stats.Count % 2 == 0)
                stats.Median = (set.ElementAt(midpoint - 1) + set.ElementAt(midpoint));
            else
                stats.Median = set.ElementAt(midpoint);

            stats.Mean = set.Average();
            stats.StandardDeviation = Math.Pow(set.Sum(x => Math.Pow(x - stats.Mean, 2)) / (isSample ? stats.Count - 1 : stats.Count), 0.5);
            var counts = set.GroupBy(x => x, x => true, (k, g) => new KeyValuePair<double, int>(k, g.Count())).OrderBy(kv => kv.Value);
            int max = counts.Last().Value;
            stats.Modes = counts.Reverse().TakeWhile(c => c.Value == max).Select(kv => kv.Key).ToArray();

            return stats;
        }

        public static double GetMultipleCorrelation(this double[][] inputs, double[] outputs)
        {
            Matrix inputsT = new Matrix(inputs).T();
            int numCols = inputs[0].Length;
            int numRows = outputs.Length;
            double[] correlations = new double[numCols];
            Matrix correlationsMatrix = new Matrix(inputsT.NumRows);
            for (int i = 0; i < numCols; ++i)
            {
                correlations[i] = inputsT[i].Zip(outputs, (input, output) => new KeyValuePair<double, double>()).GetPearsonCorrelation();
                for (int j = 0; j < numCols; ++j)
                {
                    correlationsMatrix[i, j] = i == j ? 1.0 :
                        inputsT[i].Zip(
                            inputsT[j], (input1, input2) => new KeyValuePair<double, double>(input1, input2)
                        ).GetPearsonCorrelation();
                }
            }
            return 0;
        }

        public static T[][] T<T>(this T[][] array)
        {
            var cols = array[0].Length;
            var rows = array.Length;
            T[][] rotated = new T[cols][];
            for (int j = 0; j < cols; ++j)
            {
                rotated[j] = new T[rows];
            }

            for (int i = 0; i < rows; ++i)
            {
                var row = array[i];
                for (int j = 0; j < cols; ++j)
                {
                    rotated[j][i] = row[j];
                }
            }
            return rotated;
        }

        public static T[] GetCol<T>(this T[][] matrix, int colIndex)
        {
            int rows = matrix.GetLength(0);
            T[] column = new T[rows];
            for (int i = 0; i < rows; ++i)
            {
                column[i] = matrix[i][colIndex];
            }
            return column;
        }

        public static double Median(this IEnumerable<double> dataset, bool isSorted = false)
        {
            if (!isSorted)
                dataset = dataset.OrderBy();
            int count = dataset.Count();
            int midpoint = count / 2;
            if (count % 2 == 0)
                return (dataset.ElementAt(midpoint - 1) + dataset.ElementAt(midpoint)) / 2.0;
            else
                return dataset.ElementAt(midpoint);
        }

        #region StdDev
        public static double StdDev(this IEnumerable<double> dataset) => StdDev(dataset, true, out _);

        public static double StdDev(this IEnumerable<double> dataset, bool sample) => StdDev(dataset, sample, out _);

        public static double StdDev(this IEnumerable<double> dataset, out double mean) => StdDev(dataset, true, out mean);

        public static double StdDev(this IEnumerable<double> dataset, bool sample, out double mean)
        {
            double avg = dataset.Average();
            mean = avg;
            return Math.Sqrt(
                dataset.Sum(
                    v => Math.Pow(v - avg, 2.0)
                ) / (dataset.Count() - (sample ? 1 : 0))
            );
        }
        #endregion
    }
}
