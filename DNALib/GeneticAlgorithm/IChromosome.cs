using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.GeneticAlgorithm
{
    interface IChromosome
    {
        byte[] Bytes { get; }
        bool IsFemale { get; }
    }
}
