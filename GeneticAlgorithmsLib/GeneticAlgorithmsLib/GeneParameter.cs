using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmsLib
{    
    // template for gene parameters
    public struct GeneParameter<TValue> : IGeneParameter<TValue>
        where TValue : IComparable, IComparable<TValue>, IConvertible, IEquatable<TValue>, IFormattable
    {
        public TValue MaxValue { get; private set; }  // number of possible values, minus 1
        public TValue MinValue { get; private set; }
        //public byte Bits { get; private set; }
        public byte Index { get; private set; }
        public bool Mutable { get; private set; }

        private Func<TValue> _getRandValue;
        private GeneParamType _type;
        private static readonly Type _typeFloat = typeof(float);
        private static readonly Type _typeSByte = typeof(sbyte);
        private static readonly Type _typeByte = typeof(byte);
        private static readonly Type _typeBool = typeof(bool);

        public GeneParameter(byte index, bool mutable, TValue minValue, TValue maxValue)
        {
            var type = typeof(TValue);
            if (type == _typeBool)
                _type = GeneParamType.Bool;
            else if (type.IsAssignableFrom(_typeFloat))
            {
                if (type.IsAssignableFrom(typeof(double)))
                {
                    if (type.IsAssignableFrom(typeof(decimal)))
                        _type = GeneParamType.Decimal;
                    else
                        _type = GeneParamType.Double;
                }
                else
                    _type = GeneParamType.Float;
            }
            else if (type.IsAssignableFrom(_typeSByte))
            {
                if (type.IsAssignableFrom(typeof(short)))
                {
                    if (type.IsAssignableFrom(typeof(int)))
                    {
                        if (type.IsAssignableFrom(typeof(long)))
                            _type = GeneParamType.Int64;
                        else
                            _type = GeneParamType.Int32;
                    }
                    else
                        _type = GeneParamType.Int16;
                }
                else
                    _type = GeneParamType.SByte;
            }
            else if (type.IsAssignableFrom(_typeByte))
            {
                if (type.IsAssignableFrom(typeof(ushort)))
                {
                    if (type.IsAssignableFrom(typeof(uint)))
                    {
                        if (type.IsAssignableFrom(typeof(ulong)))
                            _type = GeneParamType.UInt64;
                        else
                            _type = GeneParamType.UInt32;
                    }
                    else
                        _type = GeneParamType.UInt16;
                }
                else
                    _type = GeneParamType.Byte;
            }
            else
                throw new ArgumentOutOfRangeException("TValue", type, "Parameter is not a numeric or boolean type.");

            Index = index;
            Mutable = mutable;
            //MaxValue = (byte)(values.Length - 1);
            var range = Convert.ToDecimal(maxValue) - 1 + 1;
            MaxValue = maxValue; // upper exclusive bound
            MinValue = minValue;
            //Bits = (byte)(Constants.ALL_POWERS_OF_2_REVERSE.FindIndex(p => p >= max) + 1);
            BitConverter.
        }

        public byte GetRandomValue() =>
            (byte)Constants.GEN.Next(MaxValue + 1);

        private enum GeneParamType : byte
        {
            SByte = 0,
            Byte = 1,
            Int16 = 2,
            UInt16 = 3,
            Int32 = 4,
            UInt32 = 5,
            Int64 = 6,
            UInt64 = 7,
            Float = 8,
            Double = 9,
            Decimal = 10,
            Bool = 11
        }
    }
}
