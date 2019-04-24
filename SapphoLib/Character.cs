using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    internal class Character : PersonalityTraits, IEntity<Perception>, IEntity
    {
        // for logging and debugging purposes
        private string Name { get; set; }

        public SortedDictionary<ushort, Perception> Perceptions { get; private set; }
        public ushort EntityID { get; private set; }

        //SortedDictionary<ushort, Character> ITraitableObject<Character>.Perceptions => throw new NotImplementedException();

        private static ushort ID_COUNTER = 1;

        public Character(string name)
        {
            Name = name;
            Perceptions = new SortedDictionary<ushort, Perception>();
            Traits = Trait.Bad_Good | Trait.False_Honest | Trait.Timid_Powerful;
            EntityID = ID_COUNTER;
            ++ID_COUNTER;
        }

        public Character(string name, BoundedNumber badGood, BoundedNumber falseHonest, BoundedNumber timidPowerful) : this(name)
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

        public void ObserveVerb(ushort entityID, ushort targetID, ushort verbID, float verbIntensity, SortedList<ushort, Reaction> reactions)
        {
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
                        verbIntensityTemp *= verbIntensity;
                        targetInfo.GetTraits = () => new PersonalityTraits[] { this, Perceptions[targetID], Perceptions[entityID], Perceptions[0] };
                    }
                    else
                    {
                        targetInfo.IsYou = true;
                        targetInfo.GetTraits = () => new PersonalityTraits[] { this, Perceptions[targetID], Perceptions[0] };
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

                reactions.Add(EntityID, reaction);
            }

            // TODO: Affects self-perceptions
        }
    }
}
