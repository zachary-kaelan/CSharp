using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.Models
{
    internal abstract class AbstractReadingFrame : AbstractCodonSequence, ICodonSequence
    {
        // members only in use throughout extended initialization
        protected List<ICodonSequence> _orfs = new List<ICodonSequence>();
        protected ICodonSequence _currentorf;
        protected int _orfStartIndex = -1;
        protected bool _readingSubFrame = false;

        public AbstractReadingFrame(int position) : base(position)
        {

        }

        private static readonly SortedSet<DNACodon> START_STOP_CODONS = new SortedSet<DNACodon>() { DNACodon.Met, DNACodon.Och, DNACodon.Amb, DNACodon.Opl };
        public virtual void AddCodon(DNACodon codon)
        {
            _codons.Add(codon);
            if (START_STOP_CODONS.Contains(codon))
            {
                if (codon == DNACodon.Met)  // if start codon
                {
                    if (!_readingSubFrame)  // if not reading from a previous subORF
                    {
                        _orfStartIndex = Count;
                        _readingSubFrame = true;
                        _currentorf = _currentorf.GetNew(Count);
                    }
                }
                else if (_readingSubFrame)
                {
                    _readingSubFrame = false;
                    _currentorf.EndSequence(Count);
                    _orfs.Add(_currentorf);
                }
            }
            else if (_readingSubFrame)
                _currentorf.AddCodon(codon);
            ++Count;
        }

        public virtual void EndSequence(int position)
        {

        }

        public abstract ICodonSequence GetNew(int position);
    }
}
