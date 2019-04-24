using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.Exceptions
{
    class UnexpectedAmbiguityException : Exception
    {
        public UnexpectedAmbiguityException(Nucleotide problemInput) : base("Nucleotide was an unexpected ambiguity character.")
        {
            Data.Add("Problem Input", problemInput);
        }
    }
}
