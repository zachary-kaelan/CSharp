using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    public class ByteGenerator
    {
        private static readonly Random _gen = new Random();

        private const byte NUM_INTS = 4;
        private const byte BUFFER_SIZE = NUM_INTS * 4;

        private byte[] _buffer;
        private sbyte _bufferIndex;

        private int Index = -1;

        public byte MaxValue { get; private set; }

        public ByteGenerator(byte maxValue)
        {
            MaxValue = maxValue;
            RefreshBuffer();
        }

        public void Dispose()
        {
            _buffer = null;
        }

        public byte Next()
        {
            if (_bufferIndex == BUFFER_SIZE)
                RefreshBuffer();
            ++Index;

            byte output = _buffer[_bufferIndex];
            ++_bufferIndex;
            return output;
        }

        private void RefreshBuffer()
        {
            _buffer = new byte[BUFFER_SIZE];
            byte index = 0;
            for (byte i = 0; i < NUM_INTS; ++i)
            {
                var bytes = BitConverter.GetBytes(_gen.Next());
                for (byte b = 0; b < 4; ++b)
                {
                    // Byte doesn't have support for modulus operator, so we have to improvise
                    byte temp = bytes[b];
                    while (temp >= MaxValue)
                        temp -= MaxValue;
                    _buffer[index] = temp;
                    ++index;
                }
            }
            _bufferIndex = 0;
        }

        private void Reset()
        {
            Index = -1;
            RefreshBuffer();
        }
    }
}
