using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib.Verbs
{
    public class PhysicalVerb : IVerb
    {
        public VerbType VerbType => VerbType.Physical | VerbType.Targeted;
        internal TraitsVector Traits { get; private set; } // what it says about them
        private TraitsVector PhysicalExpression { get; set; } // what they are saying about the target

        /// <summary>Change perceptions upon observing someone take an expressive action.</summary>
        /// <param name="perception">   Perception of the person executing the verb.
        /// </param>
        /// <returns>     MY reaction to EXECUTER doing VERB to TARGET. 
        /// </returns>
        public Reaction ApplyToPerception(Perception perception, VerbTargetInfo targetInfo, float magnitude, byte verbNumTimes)
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
                return null;
            }

            byte targetIndex = (byte)(globalPerceptionsIndex - 1);

            // assumes that one has complete self-awareness of Timid_Powerful
            var selfEsteem = traits[0];

            // for expressive assertions
            PersonalityTraits expressiveAgreement = traits[targetIndex] - PhysicalExpression;
            var selfAgreement = selfEsteem - Traits;
            TraitsVector reactionTraits = expressiveAgreement.BlendToBounded(expressiveAgreement + selfAgreement, -targetInfo.KnowThemWell);

            var agreement = (reactionTraits).SumToUBounded();
            var boundedAgreement = (BoundedNumber)agreement;
            var agreementSignificance = boundedAgreement.Significance();

            if (agreementSignificance.Number * magnitude < Perception.INTEREST_MINIMUM)
                return null;

            //var agreementInverted = boundedAgreement.UInvert();

            if (verbProbability <= 0.0042) // found by graphing multiplier formula on Desmos
                surpriseMultiplier = 1;
            else
            {
                float bitsOfInformation = (float)Math.Log(verbProbability, 2);
                surpriseMultiplier = ((float)Math.Sqrt(bitsOfInformation) - 1) * 0.55f;
            }

            // how much attention you pay
            var surpriseTraits = (surprisePerceptions - Traits);
            var surprise = surpriseTraits.SumToUBounded();

            // if you are a good person, and perceive the person to be dishonest, it will change little
            // trust in yourself vs trust in the other person
            // your values are less assertive if you are more timid
            var trust = new BoundedNumber(new UBoundedNumber(selfEsteem.Bad_Good.WeightingFactor - traits[executorIndex].Bad_Good.WeightingFactor));

            // if you trust them more, disagreements are less of a shock
            surprise = agreement.Blend(surprise, trust);
            var scaledAssertions = PhysicalExpression * magnitude;

            if (perception.CircumferentialValues.TryGetValue(targetInfo.EntityID, out PersonalityTraits cValue))
            {
                if (targetInfo.AffectsTarget)
                    cValue.AddVector(scaledAssertions);
                else
                    cValue = cValue + traits[targetIndex];
            }
            else
            {
                if (targetInfo.AffectsTarget)
                    perception.CircumferentialValues.Add(targetInfo.EntityID, scaledAssertions);
                else
                    perception.CircumferentialValues.Add(targetInfo.EntityID, ((TraitsVector)traits[targetIndex] + scaledAssertions));
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
            var significance = surprise.Blend(agreementSignificance, selfEsteem.Timid_Powerful.WeightingFactor) * magnitude;

            if (targetInfo.IsYou)
            {
                if (targetInfo.AffectsTarget)
                    significance = new UBoundedNumber(significance.Amplify(0.333f));
                traits[targetIndex].Blend(oldTargetPerception + scaledAssertions, trust, true);
                if (significance.Number < 1 - selfEsteem.Timid_Powerful.WeightingFactor)
                    return null;
            }
            else
            {
                traits[targetIndex].Blend(
                    oldTargetPerception + scaledAssertions,
                    traits[globalPerceptionsIndex] * trust,
                    true
                );
                if (!targetInfo.AffectsTarget)
                    significance = significance * 0.667f;
                if (significance.Number < 1 - selfEsteem.Timid_Powerful.WeightingFactor)
                    return null;
            }

            return new Reaction(
                agreement.Suppress(1 - magnitude),
                surprise,
                ((PersonalityTraits)reactionTraits).BlendToBounded(surpriseTraits, selfEsteem.Timid_Powerful.WeightingFactor) * magnitude,
                significance
            );
        }
    }
}
