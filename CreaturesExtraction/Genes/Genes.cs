using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreaturesExtraction.Genes
{
    public enum GeneType
    {
        BrainLobe = 0,
        BrainOrgan = 1,

        Receptor = 16,
        Emitter = 17,
        ChemicalReaction = 18,
        HalfLives = 19,
        InitialConcentration = 20,

        Stimulus = 32,
        Genus = 33,
        Appearance = 34,
        Pose = 35,
        Gait = 36,
        Instinct = 37,
        Pigment = 38,
        PigmentBleed = 39,

        Organ = 48
    }

    public enum LifeStage
    {
        Embryo = 0,
        Child,
        Adolescent,
        Youth,
        Adult,
        Old,
        Senile
    }

    [Flags]
    public enum GeneCharacteristics
    {
        Mutable = 1,
        Duplicatable = 2,
        Deletable = 4,
        Male = 8,
        Female = 16,
        Dormant = 32,
        Unisex = 64
    }

    public class Gene
    {
        protected const int HEADER_LENGTH = 11;

        public GeneType Type { get; protected set; }
        public byte ID { get; protected set; }
        public byte MutationDuplicate { get; protected set; }
        public LifeStage ActivationStage { get; protected set; }
        public GeneCharacteristics Characteristics { get; protected set; }
        public byte MutationChance { get; protected set; }

        public Gene(FileStream file)
        {
            file.Seek(4, SeekOrigin.Current);
            var geneBytes = new byte[7];
            file.Read(geneBytes, 0, 7);

            Type = (GeneType)((16 * geneBytes[0]) + geneBytes[1]);
            ID = geneBytes[2];
            MutationDuplicate = geneBytes[3];
            ActivationStage = (LifeStage)geneBytes[4];
            Characteristics = (GeneCharacteristics)geneBytes[5];
            if (!Characteristics.HasFlag(GeneCharacteristics.Male) && !Characteristics.HasFlag(GeneCharacteristics.Female))
                Characteristics |= GeneCharacteristics.Unisex;
            MutationChance = geneBytes[6];
        }
    }
}
