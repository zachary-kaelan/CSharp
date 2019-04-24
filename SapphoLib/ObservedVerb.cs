using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    public struct ObservedVerb
    {
        // List of all actions and their effects on traits
        private static SortedDictionary<ushort, IVerb> VerbTraits { get; set; }

        public bool HitLimit { get; private set; }
        public byte NumberOfTimes { get; private set; }
        public ushort VerbID { get; private set; }

        internal IVerb GetTraits()
        {
            if (!HitLimit)
            {
                if (NumberOfTimes == 255)
                    HitLimit = true;
                else
                    ++NumberOfTimes;
            }
            return VerbTraits[VerbID];
        }

        public ObservedVerb(ushort verbID)
        {
            NumberOfTimes = 0x00;
            VerbID = verbID;
            HitLimit = false;
        }
    }
}
