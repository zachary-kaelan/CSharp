using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib
{
    public struct BasePair : IEquatable<BasePair>
    {
        private byte _encoded;
        private Nucleotide _firstBase;
        private Nucleotide _secondBase;

        public Nucleotide FirstBase {
            get => _firstBase;
            set
            {
                _firstBase = value;
                _encode();
            }
        }

        public Nucleotide SecondBase
        {
            get => _secondBase;
            set
            {
                _secondBase = value;
                _encode();
            }
        }

        public BasePair(Nucleotide firstBase, Nucleotide secondBase) : this()
        {
            _firstBase = firstBase;
            _secondBase = secondBase;
            _encode();
        }

        private void _encode() => _encoded = EncodingHelper.Encode(_firstBase, _secondBase);

        public bool Equals(BasePair other) => _encoded == other._encoded;
    }
}
