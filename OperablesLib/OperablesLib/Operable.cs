using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperablesLib
{
    public abstract class Operable : IOperable
    {
        public abstract OValueType Type { get; protected set; }
        public abstract IOperable Operate(UnaryOperator op);
        public abstract IOperable Operate(BinaryOperator op, IOperable other);
    }
}
