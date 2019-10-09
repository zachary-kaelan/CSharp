using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib.Verbs
{
    public class ExpressiveVerb : IVerb
    {
        public VerbType VerbType => VerbType.Expressive | VerbType.Targeted;
        internal TraitsVector Traits { get; private set; } // what it says about them
        private TraitsVector ExpressiveAssertions { get; set; } // what they are saying about the target
        // also has a catharsis factor for the character expressing it

        /// <summary>Change perceptions upon observing someone take an expressive action.</summary>
        /// <param name="perception">   Perception of the person executing the verb.
        /// </param>
        /// <returns>     MY reaction to EXECUTER saying VERB about TARGET. 
        /// </returns>
        public bool ApplyToPerception(Reaction reaction, Perception perception, VerbTargetInfo targetInfo, float magnitude, byte verbNumTimes)
        {
            // pSelf
            // pTarget
            // pExecutor
            // pGlobal
            Perception[] traits = targetInfo.GetTraits();
            byte globalPerceptionsIndex = (byte)(targetInfo.IsYou ? 2 : 3);
            byte executorIndex = (byte)(globalPerceptionsIndex - 1);

            var surprisePerceptions = targetInfo.UsingGlobalPerceptions ?
                    traits[globalPerceptionsIndex] :
                    traits[executorIndex];
            float verbProbability = surprisePerceptions.TotalPerceptions / (float)verbNumTimes;
            float surpriseMultiplier = 0;
            if (verbProbability >= 0.5)
            {
                // surpriseMultiplier is 0
                return false;
            }

            byte targetIndex = (byte)(globalPerceptionsIndex - 1);

            // assumes that one has complete self-awareness of Timid_Powerful
            var selfEsteem = traits[0];

            // for expressive assertions
            reaction.ExpressiveDissonance = traits[targetIndex] - ExpressiveAssertions;
            var selfAgreement = selfEsteem - Traits;
            reaction.TotalDissonance = reaction.ExpressiveDissonance.BlendToBounded(reaction.ExpressiveDissonance + selfAgreement, -targetInfo.KnowThemWell);

            reaction.Agreement = ((TraitsVector)reaction.TotalDissonance).SumToUBounded();
            var boundedAgreement = (BoundedNumber)reaction.Agreement;
            var agreementSignificance = boundedAgreement.Significance();

            if (agreementSignificance.Number * magnitude < VerbSelection.MINIMUM_VERB_SIGNIFICANCE)
                return false;

            //var agreementInverted = boundedAgreement.UInvert();

            if (verbProbability <= 0.0042) // found by graphing multiplier formula on Desmos
                surpriseMultiplier = 1;
            else
            {
                float bitsOfInformation = (float)Math.Log(verbProbability, 2);
                surpriseMultiplier = ((float)Math.Sqrt(bitsOfInformation) - 1) * 0.55f;
            }

            // how much attention you pay
            reaction.CharacterDissonance = (surprisePerceptions - Traits);
            reaction.Surprise = ((TraitsVector)reaction.CharacterDissonance).SumToUBounded();
            
            // if you are a good person, and perceive the person to be dishonest, it will change little
            // trust in yourself vs trust in the other person
            // your values are less assertive if you are more timid
            var trust = (-selfEsteem.Bad_Good).BlendToBounded(
                traits[executorIndex].False_Honest,
                selfEsteem.Timid_Powerful
            );

            // if you trust them more, disagreements are less of a shock
            reaction.Surprise = reaction.Agreement.Blend(reaction.Surprise, trust);
            reaction.TotalDissonance = reaction.TotalDissonance.BlendToBounded(reaction.CharacterDissonance, trust);
            var scaledAssertions = ExpressiveAssertions * magnitude;

            if (perception.CircumferentialValues.TryGetValue(targetInfo.EntityID, out PersonalityTraits cValue))
            {
                if (targetInfo.AffectsTarget)
                    cValue.AddVector(scaledAssertions);
                else
                    cValue = (cValue + traits[targetIndex]).BlendToBounded(scaledAssertions, trust);
            }
            else
            {
                if (targetInfo.AffectsTarget)
                    perception.CircumferentialValues.Add(targetInfo.EntityID, scaledAssertions);
                else
                    perception.CircumferentialValues.Add(targetInfo.EntityID, traits[targetIndex].BlendToBounded(scaledAssertions, trust));
            }

            var scaledTraits = Traits * magnitude;
            TraitsVector oldExecutorPerception = traits[executorIndex];
            traits[executorIndex].Blend(oldExecutorPerception + scaledTraits, traits[globalPerceptionsIndex], true);
            TraitsVector oldTargetPerception = traits[targetIndex];

            var oldTrust = trust;

            // trusting their judgement on this issue
            trust = trust.Blend(
                boundedAgreement, 
                selfEsteem.Timid_Powerful
            );

            //agreement = agreement.Suppress(1 - magnitude);
            // Do you react?
            reaction.Significance = reaction.Surprise.Blend(agreementSignificance, selfEsteem.Timid_Powerful.WeightingFactor) * magnitude;

            if (targetInfo.IsYou)
            {
                if (targetInfo.AffectsTarget)
                    reaction.Significance = new UBoundedNumber(reaction.Significance.Amplify(0.25f));
                // side effects
                traits[targetIndex].Blend(oldTargetPerception + scaledAssertions, trust, true);
                if (reaction.Significance.Number < 1 - selfEsteem.Timid_Powerful.WeightingFactor)
                    return false;
            }
            else
            {
                traits[targetIndex].Blend(
                    oldTargetPerception + scaledAssertions, 
                    traits[globalPerceptionsIndex] * trust, 
                    true
                );
                if (!targetInfo.AffectsTarget)
                    reaction.Significance = reaction.Significance.Suppress(0.25f);
                if (reaction.Significance.Number < 1 - selfEsteem.Timid_Powerful.WeightingFactor)
                    return false;
            }

            reaction.Agreement = reaction.Agreement.Suppress(1 - magnitude);
            return true;
        }
    }
}
