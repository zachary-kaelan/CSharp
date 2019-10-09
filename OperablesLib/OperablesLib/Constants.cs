using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperablesLib
{
    public enum OValueType : byte
    {
        Nil = 0,
        Signed = 1,
        Bit = 3,
        Byte = 4,
        SByte = Byte | Signed,
        UInt16 = 6,
        Int16 = UInt16 | Signed,
        UInt32 = 8,
        Int32 = UInt32 | Signed,
        UInt64 = 10,
        Int64 = UInt64 | Signed,
        Float = 13,
        Double = 15,
        Custom = 32
    }

    public enum UnaryOperator : byte
    {
        Positive,
        Negative,
        NOT,
        Complement,
        Increment,
        Decrement,
        TRUE,
        FALSE
    }

    public enum BinaryOperator : byte
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulus,
        AND,
        OR,
        XOR,
        LeftShift,
        RightShift
    }

    class Constants
    {
    }
}
