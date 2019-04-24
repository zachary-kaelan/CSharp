using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib
{
    [Flags]
    public enum Base
    {
        Zero = 0,
        One = 1,
        Two,
        Three = 4,
        Four = 8
    };

    public class Sequence : IReadOnlyList<BasePair>
    {
        private BasePair[] Helix;
        private byte NumberOfBases;

        public Sequence(params BasePair[] helix)
        {
            Helix = helix;
        }

        #region IReadOnlyList Implementation
        public BasePair this[int index] => Helix[index];

        public int Count => Helix.Length;

        public IEnumerator<BasePair> GetEnumerator() => ((IEnumerable<BasePair>)Helix).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Helix.GetEnumerator();
        #endregion

        public float SimilarityWith(Sequence other)
        {
            if (other.Count != Count)
                throw new IndexOutOfRangeException("Sequences being compared are not the same length.");
            int inCommon = 0;
            var enumerator = GetEnumerator();
            var otherEnumerator = other.GetEnumerator();
            while (enumerator.MoveNext())
            {
                otherEnumerator.MoveNext();
                if (enumerator.Current.Equals(otherEnumerator.Current))
                    ++inCommon;
            }
            return ((float)inCommon) / Count;
        }

        public IEnumerable<Codon> Translate(int readingFrame, int start, int stop, bool complementary)
            => complementary ?
                ReverseTranslate(readingFrame, start, stop) :
                NormalTranslate(readingFrame, start, stop);

        private IEnumerable<Codon> NormalTranslate(int readingFrame, int start, int stop)
        {
            // should start and stop be skipped as "stop" and "start" codons?
            // it should be safe to assume that the corresponding parameters are instead referring to the reading frame between said codons
            // another problem is whether "stop" refers to the maximum last start of a codon, or the end of the reading frame
            for (int i = start + readingFrame; i <= stop - 2; i += 3)
            {
                yield return new Codon(Helix[i].FirstBase, Helix[i + 1].FirstBase, Helix[i + 2].FirstBase);
            }
            yield break;
        }

        private IEnumerable<Codon> ReverseTranslate(int readingFrame, int start, int stop)
        {
            for (int i = start + readingFrame; i <= stop - 2; i += 3)
            {
                yield return new Codon(Helix[i].SecondBase, Helix[i + 1].SecondBase, Helix[i + 2].SecondBase);
            }
            yield break;
        }
    }
}
