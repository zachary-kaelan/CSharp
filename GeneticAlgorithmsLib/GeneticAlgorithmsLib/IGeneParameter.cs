using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmsLib
{
    public interface IGeneParameter<TValue>
    {
        byte Index { get; }
        bool Mutable { get; }
        TValue GetRandomValue();
    }
}
