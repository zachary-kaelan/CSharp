using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib.Statistics;

namespace NeuralNetworksLib
{
    public class DatasetNormalizationHandler
    {
        private INormalizationHelper<double>[] HelpersSingle { get; set; }
        private INormalizationHelper<double[]>[] HelpersMultiple { get; set; }
        private KeyValuePair<bool, int>[] ColumnHelperIndices { get; set; }
        private int NumRows { get; set; }
        private int NumColumns { get; set; }
        private int NumSlots { get; set; }

        public DatasetNormalizationHandler(IEnumerable<INormalizationHelper<double>> singles, IEnumerable<INormalizationHelper<double[]>> multiples, IEnumerable<KeyValuePair<bool, int>> helperIndicies, int rowCount)
        {
            HelpersSingle = singles.ToArray();
            HelpersMultiple = multiples.ToArray();
            NumRows = rowCount;
            NumColumns = HelpersSingle.Length + HelpersMultiple.Length;
            NumSlots = singles.Sum(s => s.NumSlots) + multiples.Sum(m => m.NumSlots);
            ColumnHelperIndices = helperIndicies.ToArray();
        }

        public double[] Normalize(object[] row)
        {
            ArrayList list = new ArrayList(NumSlots);

            int slot = 0;
            for (int j = 0; j < NumColumns; ++j)
            {
                var index = ColumnHelperIndices[j];
                if (index.Key)
                {
                    var helper = HelpersMultiple[index.Value];
                    var objs = helper.Normalize(row[j]);
                    list.InsertRange(slot, objs);
                    slot += helper.NumSlots;
                }
                else
                {
                    list[slot] = HelpersSingle[index.Value].Normalize(row[j]);
                    ++slot;
                }
            }

            return list.Cast<double>().ToArray();
        }

        public object[] DeNormalize(IEnumerable<double> row)
        {
            object[] list = new object[NumColumns];
            var rowTemp = row.ToList();

            int slot = 0;
            for (int j = 0; j < NumColumns; ++j)
            {
                var index = ColumnHelperIndices[j];
                if (index.Key)
                {
                    var helper = HelpersMultiple[index.Value];
                    list[j] = helper.DeNormalize(rowTemp.GetRange(slot, helper.NumSlots).ToArray());
                    slot += helper.NumSlots;
                }
                else
                {
                    list[j] = HelpersSingle[index.Value].DeNormalize(rowTemp[slot]);
                    ++slot;
                }
            }

            rowTemp = null;
            return list;
        }

        public double[][] Normalize(object[][] data)
        {
            double[][] output = new double[NumRows][];
            for (int i = 0; i < NumRows; ++i)
            {
                output[i] = new double[NumSlots];
            }

            int slot = 0;
            for (int j = 0; j < NumColumns; ++j)
            {
                var index = ColumnHelperIndices[j];
                if (index.Key)
                {
                    var helper = HelpersMultiple[index.Value];
                    for (int i = 0; i < NumRows; ++i)
                    {
                        var itor = ((IEnumerable<double>)helper.Normalize(data[i][j])).GetEnumerator();
                        for (int s = slot; itor.MoveNext(); ++s)
                        {
                            output[i][s] = itor.Current;
                        }
                    }
                    slot += helper.NumSlots;
                }
                else
                {
                    var helper = HelpersSingle[index.Value];
                    for(int i = 0; i < NumRows; ++i)
                    {
                        output[i][slot] = helper.Normalize(data[i][j]);
                    }
                    ++slot;
                }
            }

            return output;
        }

        public object[][] DeNormalize(double[][] data)
        {
            object[][] output = new object[NumRows][];
            for(int j = 0; j < NumColumns; ++j)
            {
                output[j] = new object[NumRows];
            }

            int slot = 0;
            for (int j = 0; j < NumColumns; ++j)
            {                 
                var index = ColumnHelperIndices[j];
                if(index.Key)
                {
                    var helper = HelpersMultiple[index.Value];
                    for(int i = 0; i < NumRows; ++i)
                    {
                        output[i][j] = helper.DeNormalize(data[i].ToList().GetRange(slot, helper.NumSlots).ToArray());
                    }
                    slot += helper.NumSlots;
                }
                else
                {
                    var helper = HelpersSingle[index.Value];
                    for(int i = 0; i < NumRows; ++i)
                    {
                        output[i][j] = helper.DeNormalize(data[i][slot]);
                    }
                    ++slot;
                }
            }
            return output;
        }
    }

    public struct EnumNormalizer : INormalizationHelper<double[]>
    {
        private SortedDictionary<object, double[]> NamesDict { get; set; }
        private SortedDictionary<double[], object> ValuesDict { get; set; }
        public int NumSlots { get; private set; }
        public bool IsEnum
        {
            get
            {
                return true;
            }
        }

        public EnumNormalizer(string[] names, bool isOutput = false)
        {
            NamesDict = new SortedDictionary<object, double[]>();
            ValuesDict = new SortedDictionary<double[], object>();
            NumSlots = names.Length;

            double[] last = new double[names.Length];
            for (int i = names.Length - 1; i >= 0; --i)
            {
                double[] encoded = new double[names.Length];
                encoded[i] = 1.0;
                NamesDict.Add(names[i], encoded);
                ValuesDict.Add(encoded, names[i]);
                if (!isOutput)
                    last[i] = -1.0;
            }
            last[0] = isOutput ? 1.0 : -1.0;
            string lastName = names.Last();
            NamesDict.Add(lastName, last);
            ValuesDict.Add(last, lastName);
        }

        public double[] Normalize(object obj)
        {

            return NamesDict[obj];
        }

        public object DeNormalize(double[] value)
        {
            return ValuesDict[value];
        }
    }

    public struct BoolNormalizer : INormalizationHelper<double>
    {
        public int NumSlots
        {
            get
            {
                return 1;
            }
        }
        public bool IsEnum
        {
            get
            {
                return false;
            }
        }

        public double Normalize(object obj)
        {
            return ((bool)obj) ? 1.0 : -1.0;
        }

        public object DeNormalize(double value)
        {
            return value == 1.0;
        }
    }
}
