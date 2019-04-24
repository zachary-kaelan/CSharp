using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    [Flags]
    public enum Trait
    {
        None = 0,
        Bad_Good = 1,
        False_Honest = 2,
        Timid_Powerful = 4,
        All = Bad_Good | False_Honest | Timid_Powerful
    }

    public class PersonalityTraits
    {
        public BoundedNumber Bad_Good { get; protected set; }
        public BoundedNumber False_Honest { get; protected set; }
        public BoundedNumber Timid_Powerful { get; protected set; }
        protected Trait Traits { get; set; }

        public PersonalityTraits()
        {
            Bad_Good = new BoundedNumber();
            False_Honest = new BoundedNumber();
            Timid_Powerful = new BoundedNumber();
            Traits = Trait.None;
        }

        public PersonalityTraits(BoundedNumber badGood, BoundedNumber falseHonest, BoundedNumber timidPowerful)
        {
            Bad_Good = badGood;
            False_Honest = falseHonest;
            Timid_Powerful = timidPowerful;

            Traits = Trait.None;
            if (Bad_Good.Number != 0)
                Traits |= Trait.Bad_Good;
            if (False_Honest.Number != 0)
                Traits |= Trait.False_Honest;
            if (Timid_Powerful.Number != 0)
                Traits |= Trait.Timid_Powerful;
        }

        internal PersonalityTraits(BoundedNumber badGood, BoundedNumber falseHonest, BoundedNumber timidPowerful, Trait traits)
        {
            Bad_Good = badGood;
            False_Honest = falseHonest;
            Timid_Powerful = timidPowerful;
            Traits = traits;
        }

        internal void AddVector(ITraits vec)
        {
            Bad_Good += vec.Bad_Good;
            False_Honest += vec.False_Honest;
            Timid_Powerful += vec.Timid_Powerful;

            /*if (Traits == Trait.All)
            {
                Trait newTraits = Trait.None;
                if (Bad_Good.Number != 0)
                    newTraits |= Trait.Bad_Good;
                if (False_Honest.Number != 0)
                    newTraits |= Trait.False_Honest;
                if (Timid_Powerful.Number != 0)
                    newTraits |= Trait.Timid_Powerful;
            }
            else if (Traits == Trait.None)
            {
                if (vec.Bad_Good != 0)
                    Traits |= Trait.Bad_Good;
                if (vec.False_Honest != 0)
                    Traits |= Trait.False_Honest;
                if (vec.Timid_Powerful != 0)
                    Traits |= Trait.Timid_Powerful;
            }
            else
            {
                if ()
            }*/
        }

        internal void Blend(PersonalityTraits other, PersonalityTraits weights, bool assertSecondaryHigher)
        {
            //if (Traits.HasFlag(Trait.All))

            if (assertSecondaryHigher)
            {
                Bad_Good = Bad_Good.BlendToBounded(
                    other.Bad_Good,
                    other.Bad_Good > Bad_Good ?
                        weights.Bad_Good :
                        -weights.Bad_Good
                );
                False_Honest = False_Honest.BlendToBounded(
                    other.False_Honest, 
                    other.False_Honest > False_Honest ?
                        weights.False_Honest :
                        -weights.False_Honest
                );
                Timid_Powerful = Timid_Powerful.BlendToBounded(
                    other.Timid_Powerful, 
                    other.Timid_Powerful > Timid_Powerful ?
                        weights.Timid_Powerful :
                        -weights.Timid_Powerful
                );
            }
            else
            {
                Bad_Good = Bad_Good.BlendToBounded(other.Bad_Good, weights.Bad_Good);
                False_Honest = False_Honest.BlendToBounded(other.False_Honest, weights.False_Honest);
                Timid_Powerful = Timid_Powerful.BlendToBounded(other.Timid_Powerful, weights.Timid_Powerful);
            }
        }

        internal void Blend(PersonalityTraits other, BoundedNumber weight, bool assertSecondaryHigher = false)
        {
            //if (Traits.HasFlag(Trait.All))

            if (assertSecondaryHigher)
            {
                Bad_Good = Bad_Good.BlendToBounded(
                    other.Bad_Good,
                    other.Bad_Good > Bad_Good ?
                        weight : -weight
                );
                False_Honest = False_Honest.BlendToBounded(
                    other.False_Honest, 
                    other.False_Honest > False_Honest ?
                        weight : -weight
                );
                Timid_Powerful = Timid_Powerful.BlendToBounded(
                    other.Timid_Powerful, 
                    other.Timid_Powerful > Timid_Powerful ?
                        weight : -weight
                );
            }
            else
            {
                Bad_Good = Bad_Good.BlendToBounded(other.Bad_Good, weight);
                False_Honest = False_Honest.BlendToBounded(other.False_Honest, weight);
                Timid_Powerful = Timid_Powerful.BlendToBounded(other.Timid_Powerful, weight);
            }
        }

        internal void Blend(PersonalityTraits other, float weight)
        {
            //if (Traits.HasFlag(Trait.All))

            Bad_Good = Bad_Good.BlendToBounded(other.Bad_Good, weight);
            False_Honest = False_Honest.BlendToBounded(other.False_Honest, weight);
            Timid_Powerful = Timid_Powerful.BlendToBounded(other.Timid_Powerful, weight);
        }

        internal PersonalityTraits BlendToBounded(PersonalityTraits other, BoundedNumber weight)
        {
            //if (Traits.HasFlag(Trait.All))
            return new PersonalityTraits(
                Bad_Good.BlendToBounded(other.Bad_Good, weight),
                False_Honest.BlendToBounded(other.False_Honest, weight),
                Timid_Powerful.BlendToBounded(other.Timid_Powerful, weight)
            );
        }

        /*public static implicit operator TraitsVector(PersonalityTraits traits) =>
            new TraitsVector() {
                Bad_Good = traits.Bad_Good.UnboundedNumber,
                False_Honest = traits.False_Honest.UnboundedNumber,
                Timid_Powerful = traits.Timid_Powerful.UnboundedNumber
            };*/

        /*public static PersonalityTraits operator /(PersonalityTraits traits, int integer) =>
            new PersonalityTraits(
                traits.Traits.HasFlag(Trait.Bad_Good) ? traits.Bad_Good / integer : 0,
                traits.Traits.HasFlag(Trait.False_Honest) ? traits.False_Honest / integer : 0,
                traits.Traits.HasFlag(Trait.Timid_Powerful) ? traits.Timid_Powerful / integer : 0,
                traits.Traits
            );

        public static PersonalityTraits operator -(PersonalityTraits traits, PersonalityTraits actionTraits)
        {
            float agreement = 0;
            float surprise = 0;
            if (actionTraits.Traits.HasFlag(Trait.Bad_Good))

        }*/

        public static PersonalityTraits operator *(PersonalityTraits traits, BoundedNumber multiplier) =>
            new PersonalityTraits(
                traits.Bad_Good.Number * multiplier.Number,
                traits.False_Honest.Number * multiplier.Number,
                traits.Timid_Powerful.Number * multiplier.Number
            );

        public static PersonalityTraits operator +(PersonalityTraits traits, PersonalityTraits other) =>
            new PersonalityTraits(
                traits.Bad_Good + other.Bad_Good,
                traits.False_Honest + other.False_Honest,
                traits.Timid_Powerful + other.Timid_Powerful
            );
    }
}
