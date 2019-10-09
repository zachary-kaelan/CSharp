using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    internal class Reaction
    {
        public UBoundedNumber Agreement { get; set; }
        public UBoundedNumber Surprise { get; set; }
        public UBoundedNumber Significance { get; set; }

        public PersonalityTraits ExpressiveDissonance { get; set; }
        public PersonalityTraits CharacterDissonance { get; set; }
        public PersonalityTraits TotalDissonance { get; set; }

        public Reaction(UBoundedNumber agreement, UBoundedNumber surprise, UBoundedNumber significance, TraitsVector totalDissonance)
        {
            Agreement = agreement;
            Surprise = surprise;
            Significance = significance;
            TotalDissonance = totalDissonance;
            ExpressiveDissonance = null;
            CharacterDissonance = null;
        }

        public Reaction(UBoundedNumber agreement, UBoundedNumber surprise, UBoundedNumber significance, TraitsVector totalDissonance, TraitsVector characterDissonance) : 
            this(agreement, surprise, significance, totalDissonance)
        {
            CharacterDissonance = characterDissonance;
        }

        public Reaction(UBoundedNumber agreement, UBoundedNumber surprise, UBoundedNumber significance, TraitsVector totalDissonance, TraitsVector characterDissonance, TraitsVector expressiveDissonance) :
            this(agreement, surprise, significance, totalDissonance, characterDissonance)
        {
            ExpressiveDissonance = expressiveDissonance;
        }
    }
}
