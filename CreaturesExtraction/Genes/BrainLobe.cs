using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreaturesExtraction.Genes
{
    public sealed class BrainLobe : Gene
    {
        public enum LobeOpCode
        {
            END = 0,
            _0,
            _1,
            _64,
            _255,
            chem0,
            chem1,
            chem2,
            chem3,
            state,
            output,
            thres,
            type0,
            type1,
            anded0,
            anded1,
            input,
            conduct,
            suscept,
            STW,
            LTW,
            strength,
            _32,
            _128,
            rnd_const,
            chem4,
            chem5,
            leak_in,
            leak_out,
            curr_src_leak_in,
            TRUE,
            PLUS,
            MINUS,
            TIMES,
            INCR,
            DECR,
            FALSE,
            multiply,
            average,
            move_twrds,
            random,
            ERROR
        }

        public byte XPosition { get; private set; }
        public byte YPosition { get; private set; }
        public byte Width { get; private set; }
        public byte Height { get; private set; }
        public byte PerceptionLobe { get; private set; }
        public byte NominalThreshold { get; private set; }
        public byte LeakageRate { get; private set; }
        public byte RestState { get; private set; }
        public byte InputGainHighLow { get; private set; }
        public LobeOpCode[] LobeOpCodes { get; private set; }
        public bool WinnerTakesAll { get; private set; }
        public Dendrite DendriteType0Data { get; private set; }
        public Dendrite DendriteType1Data { get; private set; }

        public BrainLobe(FileStream file) : base(file)
        {
            byte[] bytes = new byte[9];
            file.Read(bytes, 0, 9);
            
            XPosition = bytes[0];
            YPosition = bytes[1];
            Width = bytes[2];
            Height = bytes[3];
            PerceptionLobe = bytes[4];
            NominalThreshold = bytes[5];
            LeakageRate = bytes[6];
            RestState = bytes[7];
            InputGainHighLow = bytes[8];

            bytes = bytes.Skip(9).ToArray();
            LobeOpCodes = bytes.Take(12).Cast<LobeOpCode>().ToArray();
            WinnerTakesAll = bytes[12] == 1;

            bytes = bytes.Skip(13).ToArray();
            DendriteType0Data = new Dendrite(bytes);
            DendriteType1Data = new Dendrite(bytes.Skip(21).ToArray());
        }
    }

    public struct Dendrite
    {
        public byte SourceLobe { get; private set; }
        public byte MinDendrites { get; private set; }
        public byte MaxDendrites { get; private set; }
        public byte Spread { get; private set; }
        public byte Fanout { get; private set; }
        public byte MinLongTermWeight { get; private set; }
        public byte MaxLongTermWeight { get; private set; }
        public byte MinInitialStrength { get; private set; }
        public byte MaxInitialStrength { get; private set; }
        public byte Migration { get; private set; }
        public byte RelaxationSuspectibility { get; private set; }
        public byte RelaxationShortTermWeight { get; private set; }
        public byte LongTermWeightGainRate { get; private set; }
        public byte StrengthGain { get; private set; }
        public byte StrengthGainSVRule { get; private set; }
        public byte StrengthLoss { get; private set; }
        public byte StrengthLossSVRule { get; private set; }
        public byte SuscepibilitySVRule { get; private set; }
        public byte RelaxationSVRule { get; private set; }
        public byte BackPropSVRule { get; private set; }
        public byte ForwardPropSVRule { get; private set; }


        public Dendrite(byte[] bytes)
        {
            SourceLobe = bytes[0];
            MinDendrites = bytes[1];
            MaxDendrites = bytes[2];
            Spread = bytes[3];
            Fanout = bytes[4];
            MinLongTermWeight = bytes[5];
            MaxLongTermWeight = bytes[6];
            MinInitialStrength = bytes[7];
            MaxInitialStrength = bytes[8];
            Migration = bytes[9];
            RelaxationSuspectibility = bytes[10];
            RelaxationShortTermWeight = bytes[11];
            LongTermWeightGainRate = bytes[12];
            StrengthGain = bytes[13];
            StrengthGainSVRule = bytes[14];
            StrengthLoss = bytes[15];
            StrengthLossSVRule = bytes[16];
            SuscepibilitySVRule = bytes[17];
            RelaxationSVRule = bytes[18];
            BackPropSVRule = bytes[19];
            ForwardPropSVRule = bytes[20];
        }
    }
}
