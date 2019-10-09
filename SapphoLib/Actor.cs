using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    internal class Actor : PersonalityTraits, IEntity<Perception>, IEntity
    {
        // for logging and debugging purposes
        private string Name { get; set; }
        public EmotionalState Emotions { get; set; }
        private EmotionalState DefaultEmotionalState { get; set; }
        public SortedDictionary<ushort, Perception> Perceptions { get; private set; }
        public ushort EntityID { get; private set; }
        public UBoundedNumber ED { get; private set; }

        //SortedDictionary<ushort, Character> ITraitableObject<Character>.Perceptions => throw new NotImplementedException();

        private static ushort ID_COUNTER = 1;

        public Actor(string name)
        {
            Name = name;
            Perceptions = new SortedDictionary<ushort, Perception>();
            Traits = Trait.Bad_Good | Trait.False_Honest | Trait.Timid_Powerful;
            EntityID = ID_COUNTER;
            ++ID_COUNTER;
        }

        public Actor(string name, BoundedNumber badGood, BoundedNumber falseHonest, BoundedNumber timidPowerful) : this(name)
        {
            Bad_Good = badGood;
            False_Honest = falseHonest;
            Timid_Powerful = timidPowerful;
        }

        /*private void FirstImpression(Personality other, float magnitude)
        {
            Perceptions perceptionsOfOther = new Perceptions();
            perceptionsOfOther.Timid_Powerful = other.Timid_Powerful;
            perceptionsOfOther.False_Honest = other.False_Honest.Blend(other.Timid_Powerful, 0.5f) * magnitude;
            perceptionsOfOther.Bad_Good = (-Timid_Powerful).Blend(perceptionsOfOther.False_Honest, 0.25f) * magnitude;
        }*/

        public void ResetEmotionalState()
        {
            if (DefaultEmotionalState == null)
            {
                var globalPerceptions = Perceptions[0];
                var selfEsteem = Perceptions[EntityID];
                DefaultEmotionalState = new EmotionalState();

                DefaultEmotionalState.Dysphoria_Euphoria = selfEsteem.Bad_Good.BlendToBounded(globalPerceptions.Bad_Good);

                // if you see the world as helpless sheep, you'll expect less to get done
                DefaultEmotionalState.Apathy_Passion = globalPerceptions.Timid_Powerful.BlendToBounded(
                    Bad_Good.BlendToBounded(globalPerceptions.Bad_Good), -Timid_Powerful
                ).Suppress();

                DefaultEmotionalState.Inward_Outward = Timid_Powerful.BlendToBounded(
                    False_Honest.BlendToBounded(globalPerceptions.False_Honest), 0.33f
                );

                DefaultEmotionalState.Primitive_Controlled = (-DefaultEmotionalState.Inward_Outward).BlendToBounded(
                    -DefaultEmotionalState.Dysphoria_Euphoria.Significance(), 0.66f
                );
            }
            else
            {
                DefaultEmotionalState.Dysphoria_Euphoria = DefaultEmotionalState.Dysphoria_Euphoria.BlendToBounded(Emotions.Dysphoria_Euphoria, Constants.EMOTIONAL_INSTABILITY);
                DefaultEmotionalState.Apathy_Passion = DefaultEmotionalState.Apathy_Passion.BlendToBounded(Emotions.Apathy_Passion, Constants.EMOTIONAL_INSTABILITY);
                DefaultEmotionalState.Inward_Outward = DefaultEmotionalState.Inward_Outward.BlendToBounded(Emotions.Inward_Outward, Constants.EMOTIONAL_INSTABILITY);
                DefaultEmotionalState.Primitive_Controlled = DefaultEmotionalState.Primitive_Controlled.BlendToBounded(Emotions.Primitive_Controlled, Constants.EMOTIONAL_INSTABILITY);
            }

            Emotions = DefaultEmotionalState.Clone();
        }

        public void ObserveVerb(ushort entityID, ushort targetID, ushort verbID, float verbIntensity, SortedList<ushort, Reaction> reactions)
        {
            verbIntensity *= ED.Number * 2;
            if (entityID != EntityID)
            {
                Perception perception = null;

                Reaction reaction = null;
                if (Perceptions.TryGetValue(entityID, out perception))
                {
                    VerbTargetInfo targetInfo = new VerbTargetInfo(targetID);

                    var globalPerceptions = Perceptions[0].TotalPerceptions / (Perceptions.Count - 1);
                    var entityPerceptions = Perceptions[EntityID].TotalPerceptions;
                    targetInfo.KnowThemWell = new BoundedNumber(entityPerceptions - globalPerceptions);
                    if (entityPerceptions <= globalPerceptions)
                        targetInfo.UsingGlobalPerceptions = true;

                    float verbIntensityTemp = verbIntensity;
                    if (targetID != EntityID) // you are not the target
                    {
                        //verbIntensityTemp *= verbIntensity;
                        targetInfo.GetTraits = () => new Perception[] { Perceptions[EntityID], Perceptions[targetID], Perceptions[entityID], Perceptions[0] };
                    }
                    else
                    {
                        targetInfo.IsYou = true;
                        targetInfo.GetTraits = () => new Perception[] { Perceptions[EntityID], Perceptions[targetID], Perceptions[0] };
                    }
                    reaction = perception.ObserveVerb(verbID, targetInfo, this, verbIntensity);
                }
                else
                {
                    //if (notTarget && !Perceptions.ContainsKey(targetID))
                    // first impressions are made automatically upon being made observable
                    (perception, reaction) = Perception.FromFirstImpression(Constants.CHARACTERS[targetID], Perceptions[0], verbID, targetID, targetID != EntityID, verbIntensity, this);
                    Perceptions.Add(entityID, perception);
                }

                if (reaction == null)
                    Emotions.Apathy_Passion -= 0.5f * ED.Number;
                else
                    reactions.Add(EntityID, reaction);
            }

            // TODO: Affects self-perceptions
        }
    }
}
