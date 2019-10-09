using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumLib.States
{
    public class DictionaryQNumber<T> : IQNumber<T>
    {
        public SortedDictionary<T, float> Values { get; private set; }

    }
}
