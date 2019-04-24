using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    public class Perception : PersonalityTraits
    {
        internal const float INTEREST_MINIMUM = 0.05f;
        public SortedDictionary<ushort, ObservedVerb> ObservedActions { get; private set; }
        public SortedDictionary<ushort, PersonalityTraits> CircumferentialValues { get; private set; }
        public int TotalPerceptions { get; private set; }

        //private bool _personalFirstImpression;
        //private float _firstImpressionIntensity;
        //private TraitsVector _traits;
        
        public Perception() : base(0f, 0f, 0f, Trait.All)
        {
            TotalPerceptions = 0;
            ObservedActions = new SortedDictionary<ushort, ObservedVerb>();
            CircumferentialValues = new SortedDictionary<ushort, PersonalityTraits>();
            //_traits = this;
        }

        public Perception(PersonalityTraits other, float magnitudeOfFirstImpression = 0.25f)
        {
            TotalPerceptions = 0;
            ObservedActions = new SortedDictionary<ushort, ObservedVerb>();
            CircumferentialValues = new SortedDictionary<ushort, PersonalityTraits>();
            Timid_Powerful = other.Timid_Powerful * magnitudeOfFirstImpression;
            False_Honest = other.False_Honest.Blend(other.Timid_Powerful, 0.5f) * magnitudeOfFirstImpression;
            Bad_Good = (-Timid_Powerful).Blend(False_Honest, 0.25f) * magnitudeOfFirstImpression;
            Traits = Trait.All;
            //_traits = this;
        }

        internal static (Perception, Reaction) FromFirstImpression(PersonalityTraits other, Perception globalPerceptions, ushort verbID, ushort targetID, bool notTarget, float verbIntensity, TraitsVector reactorTraits)
        {
            // personaliy vs circumstance
            var perceptions = new Perception(other, 1 - verbIntensity);
            perceptions.Blend(globalPerceptions, 0.75f);
            perceptions.CircumferentialValues = globalPerceptions.CircumferentialValues;
            ++perceptions.TotalPerceptions;
            ++globalPerceptions.TotalPerceptions;
            //perceptions._personalFirstImpression = !notTarget;
            //perceptions._firstImpressionIntensity = verbIntensity;

            ObservedVerb verb = new ObservedVerb(verbID);
            var actionTraits = verb.GetTraits().ApplyMagnitude(verbIntensity);
            perceptions.AddVector(actionTraits);

            return (
                perceptions,
                new Reaction(
                    (reactorTraits - actionTraits).SumToBounded(),
                    (perceptions - actionTraits).SumToBounded(),
                    actionTraits,
                    1f,
                    verbIntensity
                )
            );
        }

        internal Reaction ObserveVerb(ushort verbID, VerbTargetInfo target, TraitsVector reactorTraits, float verbIntensity)
        {
            IVerb verbTraits = null;

            if (ObservedActions.TryGetValue(verbID, out ObservedVerb verb))
            {
                verbTraits = verb.GetTraits();
                ObservedActions[verbID] = verb;
            }
            else
            {
                verb = new ObservedVerb(verbID);
                verbTraits = verb.GetTraits();
                ObservedActions.Add(verbID, verb);
            }

            if (!verb.HitLimit)
            {
                ++TotalPerceptions;
                // could also be used to represent desensitization to violence
                float magnitude = (1.0f / verb.NumberOfTimes) * (1 - verbIntensity);
                //TraitsVector thisTraits = this;
                AddVector(verbTraits.ApplyMagnitude(magnitude));


                if (verbTraits.VerbType == VerbType.None)
                {
                    AddVector(verbTraits.ApplyMagnitude(magnitude));
                    
                }
                else if (verb)
                {

                }

                return new Reaction(
                    (reactorTraits - verbTraits).SumToBounded().Suppress(1 - magnitude),
                    ((TraitsVector)this - verbTraits).SumToBounded().Suppress(1 - magnitude),
                    verbTraits,
                    magnitude,
                    verbIntensity
                );
            }
            else
                return null;
        }
    }
}
