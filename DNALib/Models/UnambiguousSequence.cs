using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.Models
{
    public enum UnambiguousNucleotide : byte
    {
        Adenine = 0,
        Cytosine,
        Guanine,
        Thymine
    }

    public class UnambiguousSequence : IEnumerator<UnambiguousNucleotide>
    {
        private byte[] _bytes;
        private int ByteIndex = 0;
        private UnambiguousNucleotide[] _buffer;
        private int BufferIndex = 0;

        public UnambiguousSequence(byte[] bytes)
        {
            _bytes = bytes;
            _buffer = new UnambiguousNucleotide[4];
        }

        public UnambiguousNucleotide Current => _buffer[BufferIndex];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            // the buffer is in reverse, to match the right-to-left binary encoding of bytes
            if (--BufferIndex < 0)
            {
                if (++ByteIndex >= _bytes.Length)
                    return false;
                byte current = _bytes[ByteIndex];
                var enumerator = Constants.POWERS_OF_2.GetEnumerator();
                for (int i = 0; i < 4; ++i)
                {
                    enumerator.MoveNext();
                    if (current > enumerator.Current)   // first bit is set
                    {
                        current -= enumerator.Current;
                        enumerator.MoveNext();
                        if (current > enumerator.Current)   // second bit is set
                        {
                            _buffer[i] = UnambiguousNucleotide.Thymine;
                            current -= enumerator.Current;
                        }
                        else    // second bit is not set
                        {
                            _buffer[i] = UnambiguousNucleotide.Guanine;
                            // if all the other bits are 0, they are known and can be skipped
                            // could make these skippable Nucleotides the most common in a sequence to increase skipping likelihood
                            if (current == enumerator.Current)  // rest of bits are not set
                            {
                                for (int k = i + 1; k < 4; ++k)
                                {
                                    _buffer[k] = UnambiguousNucleotide.Adenine;
                                }
                                return true;
                            }
                        }
                    }
                    else if (current < enumerator.Current)  // first bit is not set
                    {
                        enumerator.MoveNext();
                        if (current > enumerator.Current)   // second bit is set
                        {
                            _buffer[i] = UnambiguousNucleotide.Cytosine;
                            current -= enumerator.Current;
                        }
                        else    // second bit is not set
                        {
                            _buffer[i] = UnambiguousNucleotide.Adenine;
                            if (current == enumerator.Current)  // the rest of the bits are not set
                            {
                                for (int k = i + 1; k < 4; ++k)
                                {
                                    _buffer[k] = UnambiguousNucleotide.Adenine;
                                }
                                return true;
                            }
                        }
                    }
                    else    // first bit is last set bit in sequence
                    {
                        // means this bit-pair is 10
                        _buffer[i] = UnambiguousNucleotide.Guanine;
                        for (int k = i + 1; k < 4; ++k)
                        {
                            _buffer[k] = UnambiguousNucleotide.Adenine;
                        }
                        return true;
                    }
                }

                // could also check if the remaining bits are equal to the current power of 2 minus 1
                // in that case, all the remaining bits are set to 1

                BufferIndex = 4;
            }
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
