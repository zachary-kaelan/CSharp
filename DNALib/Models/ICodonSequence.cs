using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.Models
{
    internal interface ICodonSequence
    {
        void AddCodon(DNACodon codon);
        void EndSequence(int position);
        ICodonSequence GetNew(int position);
    }
}
