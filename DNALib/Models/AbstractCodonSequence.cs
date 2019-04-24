using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.Models
{
    public class AbstractCodonSequence
    {
        public int Count { get; protected set; }
        protected List<DNACodon> _codons = new List<DNACodon>();
        // start and end position in the parent codon sequence
        protected int _startPosition;
        protected int _endPosition;

        public AbstractCodonSequence(int position)
        {
            _startPosition = position;
            // skips the start codon
            Count = 1;
            _codons.Add(DNACodon.Met);
        }
    }
}
