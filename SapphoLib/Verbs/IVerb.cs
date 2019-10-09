using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    [Flags]
    public enum VerbType : byte
    {
        None = 0,
        Targeted = 1,
        Expressive = 2,
        Circumferential = 4,
        Physical = 8,
        Reactive = 16,
        Emotional = 32
    }

    internal interface IVerb
    {
        TraitsVector Traits { get; }
        VerbType VerbType { get; }
        bool ApplyToPerception(Reaction reaction, Perception perception, VerbTargetInfo targetInfo, TraitsVector reactorTraits, float verbIntensity);
        bool ApplyToEmotions(Reaction reaction);
    }
}
