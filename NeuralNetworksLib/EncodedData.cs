using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;
using ZachLib.Statistics;

namespace NeuralNetworksLib
{
    public enum DataType
    {
        NumericX,
        NumericY,
        BinaryX,
        BinaryY,
        CategoricalX,
        CategoricalY
    };

    public class DataEncoder
    {
        public DataType[] DataTypes { get; private set; }
        public Matrix Encoded { get; private set; }

        public DataEncoder(string[][] data, DataType[] types)
        {
            DataTypes = types;
            int count = types.Length;

            for (int i = 0; i < count; ++i)
            {

            }
        }
    }
}
