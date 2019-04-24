using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    internal class Reaction
    {
        public UBoundedNumber Agreement { get; private set; }
        public UBoundedNumber Surprise { get; private set; }
        public UBoundedNumber Significance { get; private set; }
        public TraitsVector Traits { get; private set; }

        public Reaction(UBoundedNumber agreement, UBoundedNumber surprise, TraitsVector reactionTraits, UBoundedNumber significance)
        {
            Agreement = agreement;
            Surprise = surprise;
            Significance = significance;
            Traits = reactionTraits;
        }
    }
}
