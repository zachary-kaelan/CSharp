using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib.Verbs
{
    public static class VerbSelection
    {
        public const float MINIMUM_VERB_SIGNIFICANCE = 0.05f;
        private const float REACTIVE_VERB_SIGNIFICANCE = 0.2f;

        public int SelectVerb(int entityID, Reaction reaction)
        {
            if (reaction.Significance < REACTIVE_VERB_SIGNIFICANCE)
            {

            }
        }
    }
}
