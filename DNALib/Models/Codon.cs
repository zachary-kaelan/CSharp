using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNALib.Exceptions;

namespace DNALib
{
    public struct Codon : IEquatable<Codon>
    {
        private Nucleotide _firstBase;
        private Nucleotide _secondBase;
        private Nucleotide _thirdBase;
        private (byte, byte) _encoded;

        public Nucleotide FirstBase {
            get => _firstBase;
            set
            {
                _firstBase = value;
                _encodeFirst();
            }
        }

        public Nucleotide SecondBase {
            get => _secondBase;
            set
            {
                _secondBase = value;
                _encodeFirst();
            }
        }

        public Nucleotide ThirdBase {
            get => _thirdBase;
            set
            {
                _thirdBase = value;
                _encodeSecond();
            }
        }

        public AminoProperties Props { get; private set; }

        public Codon(Nucleotide firstBase, Nucleotide secondBase, Nucleotide thirdBase, AminoProperties props) : this()
        {
            _firstBase = firstBase;
            _secondBase = secondBase;
            _thirdBase = thirdBase;
            Props = props;
            _encodeBoth();
        }

        public Codon(Nucleotide firstBase, Nucleotide secondBase, Nucleotide thirdBase) : this()
        {
            _firstBase = firstBase;
            _secondBase = secondBase;
            _thirdBase = thirdBase;

            // figure out the Props value
            switch(_firstBase)
            {
                case Nucleotide.Thymine:
                    Props = AminoProperties.Nonpolar;
                    break;

                case Nucleotide.Cytosine:
                    Props = Nucleotide.Purine.HasFlag(_secondBase) ?
                        AminoProperties.Polar :
                        AminoProperties.Nonpolar;
                    break;

                case Nucleotide.Adenine:
                    switch (_secondBase)
                    {
                        case Nucleotide.Thymine:
                            Props = Nucleotide.Pyrmidine.HasFlag(_thirdBase) ?
                                AminoProperties.Polar :
                                AminoProperties.Stop;
                            break;

                        case Nucleotide.Cytosine:
                            Props = Nucleotide.Pyrmidine.HasFlag(_thirdBase) ?
                                AminoProperties.Basic :
                                AminoProperties.Polar;
                            break;

                        case Nucleotide.Adenine:
                            Props = Nucleotide.Pyrmidine.HasFlag(_thirdBase) ?
                                AminoProperties.Polar :
                                AminoProperties.Basic;
                            break;

                        case Nucleotide.Guanine:
                            Props = AminoProperties.Acidic;
                            break;

                        default:
                            throw new UnexpectedAmbiguityException(_secondBase);
                    }
                    break;

                case Nucleotide.Guanine:
                    switch (_secondBase)
                    {
                        case Nucleotide.Thymine:
                            if (Nucleotide.Pyrmidine.HasFlag(_thirdBase))
                                Props = AminoProperties.Polar;
                            else if (_thirdBase == Nucleotide.Adenine)
                                Props = AminoProperties.Stop;
                            else
                                Props = AminoProperties.Nonpolar;
                            break;

                        case Nucleotide.Cytosine:
                            Props = AminoProperties.Basic;
                            break;

                        case Nucleotide.Adenine:
                            Props = Nucleotide.Pyrmidine.HasFlag(_thirdBase) ?
                                AminoProperties.Polar :
                                AminoProperties.Basic;
                            break;

                        case Nucleotide.Guanine:
                            Props = AminoProperties.Nonpolar;
                            break;

                        default:
                            throw new UnexpectedAmbiguityException(_secondBase);
                    }
                    break;

                default:
                    throw new UnexpectedAmbiguityException(_firstBase);
            }
            _encodeBoth();
        }

        // an Encode call can be saved by determining what was changed
        private void _encodeFirst() => _encoded.Item1 = EncodingHelper.Encode(_firstBase, _secondBase);
        private void _encodeSecond() => _encoded.Item2 = EncodingHelper.Encode((byte)_thirdBase, (byte)Props);
        private void _encodeBoth() => _encoded = EncodingHelper.Encode(_firstBase, _secondBase, _thirdBase, (byte)Props);

        public bool Equals(Codon other) => _encoded == other._encoded;
    }
}
