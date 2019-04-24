using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DNALib
{
    public class CodonSequence : IReadOnlyList<DNACodon>
    {
        private DNACodon[] Codons;

        public CodonSequence(params DNACodon[] codons)
        {
            Codons = codons;
        }

        #region IReadOnlyList<DNACodon> Implementation
        public DNACodon this[int index] => Codons[index];

        public int Count => Codons.Length;

        public IEnumerator<DNACodon> GetEnumerator() => ((IReadOnlyList<DNACodon>)Codons).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Codons.GetEnumerator();
        #endregion

        public DNACodon[] GetArray() => Codons;
    }
}
