using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreaturesExtraction.Genes
{
    public sealed class BrainOrgan : Gene
    {
        public byte ClockRate { get; private set; }
        public byte LifeForceRepairRate { get; private set; }
        public byte LifeForceStart { get; private set; }
        public byte BiotickStart { get; private set; }
        public byte ATPDamageCoeff { get; private set; }

        public BrainOrgan(byte[] geneBytes) : base(geneBytes)
        {
            var bytes = geneBytes.Skip(HEADER_LENGTH).ToArray();
            ClockRate = bytes[0];
            LifeForceRepairRate = bytes[1];
            LifeForceStart = bytes[2];
            BiotickStart = bytes[3];
            ATPDamageCoeff = bytes[4];
        }
    }
}
