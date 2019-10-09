using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperablesLib
{
    public interface IOperable
    {
        OValueType Type { get; }
        IOperable Operate(UnaryOperator op);
        IOperable Operate(BinaryOperator op, IOperable other);
    }
}
