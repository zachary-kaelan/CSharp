using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib.Verbs
{
    public class VerbInfo
    {
        public VerbType Type { get; private set; }
        public Func<PersonalityTraits, float> PersonalityInclination { get; private set; }
        public Emotion[] EmotionalInclinations { get; private set; }
    }
}
