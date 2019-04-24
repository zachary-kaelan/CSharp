using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreaturesExtraction.Genes
{
    public class Transmitter : Gene
    {
        public byte Organ { get; protected set; }
        public byte Tissue { get; protected set; }
        public byte Locus { get; protected set; }
        public byte Chemical { get; protected set; }
        public byte Threshold { get; protected set; }
        public byte Gain { get; protected set; }

        public Transmitter(byte[] geneBytes) : base(geneBytes)
        {
            var bytes = geneBytes.Skip(HEADER_LENGTH).ToArray();
            Organ = bytes[0];
            Tissue = bytes[1];
            Locus = bytes[2];
            Chemical = bytes[3];
            Threshold = bytes[4];
            Gain = bytes[6];
        }
    }

    public sealed class Receptor : Transmitter
    {
        [Flags]
        public enum ReceptorFlags
        {
            Inverted = 1,
            Digital = 2
        }

        public byte Nominal { get; private set; }
        public ReceptorFlags Flags { get; private set; }

        public Receptor(byte[] geneBytes) : base(geneBytes)
        {
            var bytes = geneBytes.Skip(HEADER_LENGTH + 5).ToArray();
            Nominal = bytes[0];
            //bytes[1] is Gain
            Flags = (ReceptorFlags)bytes[2];
        }
    }

    public sealed class Emitter : Transmitter
    {
        [Flags]
        public enum EmitterFlags
        {
            ClearSource = 1,
            Digital = 2,
            Inverted = 4
        }

        public byte SampleRate { get; private set; }
        public EmitterFlags Flags { get; private set; }

        public Emitter(byte[] geneBytes) : base(geneBytes)
        {
            var bytes = geneBytes.Skip(HEADER_LENGTH + 5).ToArray();
            SampleRate = bytes[0];
            Flags = (EmitterFlags)bytes[2];
        }
    }
}
