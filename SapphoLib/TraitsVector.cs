using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    // Allows for Personality Models to be put on the stack for a lot of calculations without bounding
    internal struct TraitsVector : ITraits
    {
        public float Bad_Good { get; set; }
        public float False_Honest { get; set; }
        public float Timid_Powerful { get; set; }

        private TraitsVector(float badGood, float falseHonest, float timidPowerful)
        {
            Bad_Good = badGood;
            False_Honest = falseHonest;
            Timid_Powerful = timidPowerful;
        }

        public float Sum() => Bad_Good + False_Honest + Timid_Powerful;
        public float USum() => Math.Abs(Bad_Good) + Math.Abs(False_Honest) + Math.Abs(Timid_Powerful);

        public BoundedNumber SumToBounded() => 
            BoundedNumber.FromUnboundedNumber(Bad_Good + False_Honest + Timid_Powerful);
        public UBoundedNumber SumToUBounded() =>
            UBoundedNumber.FromUnbounded(Math.Abs(Bad_Good) + Math.Abs(False_Honest) + Math.Abs(Timid_Powerful));

        public static TraitsVector operator +(TraitsVector traits, ITraits otherTraits) =>
            new TraitsVector(
                traits.Bad_Good + otherTraits.Bad_Good,
                traits.False_Honest + otherTraits.False_Honest,
                traits.Timid_Powerful + otherTraits.Timid_Powerful
            );

        public static TraitsVector operator -(TraitsVector traits, ITraits otherTraits) =>
            new TraitsVector(
                traits.Bad_Good - otherTraits.Bad_Good,
                traits.False_Honest - otherTraits.False_Honest,
                traits.Timid_Powerful - otherTraits.Timid_Powerful
            );

        // magnitude applies to unbounded numbers
        // means you can't kill someone "lightly" enough for people to not hate you
        public static TraitsVector operator *(TraitsVector traits, float weight) =>
            new TraitsVector(
                traits.Bad_Good * weight,
                traits.False_Honest * weight,
                traits.Timid_Powerful * weight
            );

        public static implicit operator TraitsVector(PersonalityTraits traits) =>
            new TraitsVector()
            {
                Bad_Good = traits.Bad_Good.UnboundedNumber,
                False_Honest = traits.False_Honest.UnboundedNumber,
                Timid_Powerful = traits.Timid_Powerful.UnboundedNumber
            };

        public static implicit operator PersonalityTraits(TraitsVector vec) =>
            new PersonalityTraits(
                BoundedNumber.FromUnboundedNumber(vec.Bad_Good),
                BoundedNumber.FromUnboundedNumber(vec.False_Honest),
                BoundedNumber.FromUnboundedNumber(vec.Timid_Powerful)
            );
    }
}
