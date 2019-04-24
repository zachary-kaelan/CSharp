using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib.Verbs
{
    public struct BasicVerb : IVerb
    {
        public VerbType VerbType => VerbType.None;
        public TraitsVector Traits { get; private set; }

        internal Reaction ApplyToPerception(Perception perception, VerbTargetInfo info, float magnitude, float verbIntensity)
        {
            var thisTraits = ApplyMagnitude(magnitude);
            if (info.IsYou)
            {

            }
            return new Reaction(
                (reactorTraits - verbTraits).SumToBounded().Suppress(1 - magnitude),
                (thisTraits - verbTraits).SumToBounded().Suppress(1 - magnitude),
                verbTraits,
                magnitude,
                verbIntensity
            );
        }
            
    }
}
