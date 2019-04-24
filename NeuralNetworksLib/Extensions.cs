using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZachLib;
using ZachLib.Statistics;

namespace NeuralNetworksLib
{
    /*public static class NNExtensions
    {
        public static double Encode(this bool binary)
        {
            return binary ? 1.0 : -1.0;
        }

        public static double[] Encode(this double[] data, ref CollectionStatistics stats)
        {
            if (stats.Maximum == stats.Minimum)
                stats = data.GetStatistics();

            return data.Select(
                v => stats.Normalize(v)
            ).ToArray();
        }

        public static IEnumerable<double> Encode(this IEnumerable<double> data, ref CollectionStatistics stats)
        {
            if (stats.Maximum == stats.Minimum)
                stats = data.GetStatistics();

            var temp = stats;
            return data.Select(
                v => temp.Normalize(v)
            );
        }

        public static double[][] Encode(this double[][] data, ref CollectionStatistics[] stats)
        {
            var cols = data.GetLength(1);
            if (stats == null)
            {
                var rotated = data.T();
                stats = new CollectionStatistics[cols];
                for(int i = 0; i < cols; ++i)
                {
                    stats[i] = rotated[i].GetStatistics();
                }
            }

            var rows = data.GetLength(0);
            double[][] encoded = new double[rows][];
            for(int i = 0; i < rows; ++i)
            {
                var row = new double[cols];
                for(int j = 0; j < cols; ++j)
                {
                    row[j] = stats[j].Normalize(data[i][j]);
                }
                encoded[i] = row;
            }
            return encoded;
        }

        public static double[][] EncodeObjects<T>(this IEnumerable<T> data, out DatasetNormalizationHandler normalizer)
        {
            int rows = data.Count();
            Type t = typeof(T);
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToArray();
            object[][] unencoded = data.Select(o => props.Select(p => p.GetValue(o)).ToArray()).ToArray().T();
            var propGroups = props.GroupBy(p => p.PropertyType.IsEnum || p.PropertyType == typeof(string));
            props = propGroups.First(g => !g.Key).ToArray();
            var enumProps = propGroups.Last(g => g.Key).ToArray();
            propGroups = null;
            int cols = props.Length;
            int enumsCount = enumProps.Length;

            double[][] encoded = new double[rows][];
            List<INormalizationHelper<double>> singles = new List<INormalizationHelper<double>>(cols);
            List<KeyValuePair<bool, int>> indicies = new List<KeyValuePair<bool, int>>(cols + enumsCount);

            int boolIndex = -1;
            int index = 0;
            for (int i = 0; i < cols; ++i)
            {
                Type pType = props[i].PropertyType;
                if (pType == typeof(bool))
                {
                    if (boolIndex == -1)
                    {
                        singles.Add(new BoolNormalizer());
                        indicies.Add(new KeyValuePair<bool, int>(false, index));
                        boolIndex = index;
                        ++index;
                    }
                    else
                        indicies.Add(new KeyValuePair<bool, int>(false, boolIndex));
                }
                else
                {
                    singles.Add(unencoded[i].Cast<double>().GetStatistics());
                    indicies.Add(new KeyValuePair<bool, int>(false, index));
                    ++index;
                }
            }

            
            Dictionary<string, double[]>[] enums = new Dictionary<string, double[]>[enumsCount];
            for (int i = 0; i < enumsCount; ++i)
            {
                var prop = enumProps[i];
                var names = Enum.GetNames(prop.PropertyType);

            }
        }
    }*/
}
