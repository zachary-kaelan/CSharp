using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumLib.States
{
    public interface IQNumber<T>
    {
        T ExpectedValue { get; }

        float GetProbabilityOf(T value);
    }
}
