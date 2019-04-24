using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedules_v3.SimpleGeneticAlgorithm;

namespace CampSchedules_v3
{
    internal class UnboundedByteGenerator
    {
        private const byte NUM_INTS = 4;
        private const byte BUFFER_SIZE = NUM_INTS * 4;

        private byte[] _buffer;
        private sbyte _bufferIndex;

        private int Index = -1;

        public UnboundedByteGenerator()
        {
            RefreshBuffer();
        }

        public void Dispose()
        {
            _buffer = null;
        }

        internal byte GetNext(byte maxValue)
        {
            if (_bufferIndex == BUFFER_SIZE)
                RefreshBuffer();
            ++Index;

            byte output = _buffer[_bufferIndex];
            while (output >= maxValue)
                output -= maxValue;
            ++_bufferIndex;
            return output;
        }

        private void RefreshBuffer()
        {
            _buffer = new byte[BUFFER_SIZE];
            byte index = 0;
            for (byte i = 0; i < NUM_INTS; ++i)
            {
                var bytes = BitConverter.GetBytes(SimpleChromosome.GEN.Next());
                Array.Copy(bytes, 0, _buffer, index, 4);
                index += 4;
            }
            _bufferIndex = 0;
        }

        public void Reset()
        {
            Index = -1;
            RefreshBuffer();
        }
    }
}
