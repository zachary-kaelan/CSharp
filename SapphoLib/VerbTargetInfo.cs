using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    internal struct VerbTargetInfo : IEntity
    {
        public bool AffectsTarget { get; set; }
        public ushort EntityID { get; set; }
        public bool IsYou { get; set; }
        public float Relationship { get; set; }
        public bool UsingGlobalPerceptions { get; set; }
        public BoundedNumber KnowThemWell { get; set; }
        public Func<Perception[]> GetTraits { get; set; }

        public VerbTargetInfo(ushort id)
        {
            KnowThemWell = new BoundedNumber();
            EntityID = id;
            IsYou = false;
            Relationship = 0;
            GetTraits = null;
            AffectsTarget = true;
        }

        public VerbTargetInfo(ushort id, bool isYou)
        {
            EntityID = id;
            IsYou = isYou;
            Relationship = 0;
            GetTraits = null;
            AffectsTarget = true;
            KnowThemWell = new BoundedNumber();
        }

        public VerbTargetInfo(ushort id, bool isYou, float relationship)
        {
            EntityID = id;
            IsYou = isYou;
            Relationship = relationship;
            GetTraits = null;
            AffectsTarget = true;
            KnowThemWell = new BoundedNumber();
        }
    }
}
