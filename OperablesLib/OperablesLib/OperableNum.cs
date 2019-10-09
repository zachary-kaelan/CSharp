using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperablesLib.OperableImplementations
{
    public class OperableNum : Operable
    {
        public override OValueType Type { get; protected set; }
        public object _num;

        public OperableNum(object num, OValueType type)
        {
            _num = num;
            Type = type;
        }

        public override IOperable Operate(UnaryOperator op)
        {
            switch(Type)
            {
			    case OValueType.Nil:
                    return new OperableNum(null, OValueType.Nil);

                case OValueType.Custom:
                    throw new ArgumentOutOfRangeException();
                
				case OValueType.Bit:
					switch (op)
					{
						case UnaryOperator.NOT:
							return new OperableNum(!((bool)_num), OValueType.Bit);
							
						case UnaryOperator.TRUE:
							return new OperableNum(((bool)_num) ? true : false, OValueType.Bit);
							
						case UnaryOperator.FALSE:
							return new OperableNum(((bool)_num) ? true : false, OValueType.Bit);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.Byte:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((byte)_num), OValueType.Byte);
							
						case UnaryOperator.Negative:
							return new OperableNum(-((byte)_num), OValueType.Byte);
							
						case UnaryOperator.Complement:
							return new OperableNum(~((byte)_num), OValueType.Byte);
							
						case UnaryOperator.Increment:
							return new OperableNum(((byte)_num) + 1, OValueType.Byte);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((byte)_num) - 1, OValueType.Byte);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.SByte:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((sbyte)_num), OValueType.SByte);
							
						case UnaryOperator.Negative:
							return new OperableNum(-((sbyte)_num), OValueType.SByte);
							
						case UnaryOperator.Complement:
							return new OperableNum(~((sbyte)_num), OValueType.SByte);
							
						case UnaryOperator.Increment:
							return new OperableNum(((sbyte)_num) + 1, OValueType.SByte);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((sbyte)_num) - 1, OValueType.SByte);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.UInt16:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((ushort)_num), OValueType.UInt16);
							
						case UnaryOperator.Negative:
							return new OperableNum(-((ushort)_num), OValueType.UInt16);
							
						case UnaryOperator.Complement:
							return new OperableNum(~((ushort)_num), OValueType.UInt16);
							
						case UnaryOperator.Increment:
							return new OperableNum(((ushort)_num) + 1, OValueType.UInt16);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((ushort)_num) - 1, OValueType.UInt16);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.Int16:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((short)_num), OValueType.Int16);
							
						case UnaryOperator.Negative:
							return new OperableNum(-((short)_num), OValueType.Int16);
							
						case UnaryOperator.Complement:
							return new OperableNum(~((short)_num), OValueType.Int16);
							
						case UnaryOperator.Increment:
							return new OperableNum(((short)_num) + 1, OValueType.Int16);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((short)_num) - 1, OValueType.Int16);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.UInt32:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((uint)_num), OValueType.UInt32);
							
						case UnaryOperator.Negative:
							return new OperableNum(-((uint)_num), OValueType.UInt32);
							
						case UnaryOperator.Complement:
							return new OperableNum(~((uint)_num), OValueType.UInt32);
							
						case UnaryOperator.Increment:
							return new OperableNum(((uint)_num) + 1, OValueType.UInt32);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((uint)_num) - 1, OValueType.UInt32);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.Int32:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((int)_num), OValueType.Int32);
							
						case UnaryOperator.Negative:
							return new OperableNum(-((int)_num), OValueType.Int32);
							
						case UnaryOperator.Complement:
							return new OperableNum(~((int)_num), OValueType.Int32);
							
						case UnaryOperator.Increment:
							return new OperableNum(((int)_num) + 1, OValueType.Int32);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((int)_num) - 1, OValueType.Int32);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.UInt64:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((ulong)_num), OValueType.UInt64);
							
						case UnaryOperator.Complement:
							return new OperableNum(~((ulong)_num), OValueType.UInt64);
							
						case UnaryOperator.Increment:
							return new OperableNum(((ulong)_num) + 1, OValueType.UInt64);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((ulong)_num) - 1, OValueType.UInt64);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.Int64:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((long)_num), OValueType.Int64);
							
						case UnaryOperator.Complement:
							return new OperableNum(~((long)_num), OValueType.Int64);
							
						case UnaryOperator.Increment:
							return new OperableNum(((long)_num) + 1, OValueType.Int64);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((long)_num) - 1, OValueType.Int64);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.Float:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((float)_num), OValueType.Float);
							
						case UnaryOperator.Negative:
							return new OperableNum(-((float)_num), OValueType.Float);
							
						case UnaryOperator.Increment:
							return new OperableNum(((float)_num) + 1, OValueType.Float);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((float)_num) - 1, OValueType.Float);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
				case OValueType.Double:
					switch (op)
					{
						case UnaryOperator.Positive:
							return new OperableNum(+((double)_num), OValueType.Double);
							
						case UnaryOperator.Negative:
							return new OperableNum(-((double)_num), OValueType.Double);
							
						case UnaryOperator.Increment:
							return new OperableNum(((double)_num) + 1, OValueType.Double);
							
						case UnaryOperator.Decrement:
							return new OperableNum(((double)_num) - 1, OValueType.Double);
							
						default:
							throw new ArgumentOutOfRangeException();
					}

					
            }

			throw new ArgumentOutOfRangeException();
        }

        public override IOperable Operate(BinaryOperator op, IOperable other)
        {
            throw new NotImplementedException();
        }
    }
}