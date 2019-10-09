using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib.Logging;

namespace GeneticAlgorithmsLib
{
    public abstract class BaseChromosome : IComparable<BaseChromosome>
    {
        public ulong ChromosomeID { get; private set; }
        public BaseGene[] Genes { get; private set; }
        public KeyValuePair<ulong, ulong> ParentIDs { get; private set; }

        public byte Age = 0;

        // 0 is reserved as parents for initial chromosomes
        private static ulong CHROMOSOME_ID_COUNTER = 1;
        private bool _logBufferFlushed = false;
        private List<LogUpdate> _logBuffer = new List<LogUpdate>();

        public BaseChromosome(BaseGene[] genes)
        {
            Genes = genes;
            ParentIDs = new KeyValuePair<ulong, ulong>(0, 0);
            ChromosomeID = CHROMOSOME_ID_COUNTER++;
            _logBufferFlushed = true;
        }

        public BaseChromosome(BaseGene[] genes, KeyValuePair<ulong, ulong> parents)
        {
            Genes = genes;
            ParentIDs = parents;
            ChromosomeID = CHROMOSOME_ID_COUNTER++;
            _logBufferFlushed = true;
        }
    }
}
