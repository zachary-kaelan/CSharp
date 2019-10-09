using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib.Verbs
{
    public class ReactiveVerb : IVerb
    {
        public TraitsVector Traits { get; private set; }

        public VerbType VerbType => VerbType.Reactive;

        public Reaction ApplyToPerception(Perception perception, VerbTargetInfo targetInfo, TraitsVector reactorTraits, float verbIntensity)
        {
            throw new NotImplementedException();
        }
    }
}
