using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib.Verbs
{
    [Flags]
    internal enum VerbSummary
    {
        None = 0,
        Negative_Bad_Good = 1,
        Positive_Bad_Good = 2,
        Negative_False_Honest = 4,
        Positive_False_Honest = 8,
        Negative_Timid_Dominant = 16,
        Positive_Timid_Dominant = 32
    }

    public class Verblist
    {
        public const float MINIMUM_VERB_SIGNIFICANCE = 0.05f;
        private const float REACTIVE_VERB_SIGNIFICANCE = 0.2f;

        private SortedSet<ushort> AllVerbs { get; set; }
        private SortedSet<ushort> CharacterEstablishingVerbs { get; set; }
        private SortedSet<ushort> EmotionalVerbs { get; set; }
        private SortedSet<ushort> EmotionallyNeutralVerbs { get; set; }
        private SortedDictionary<Emotion, SortedSet<ushort>> VerbsByEmotion { get; set; }
        private SortedDictionary<VerbType, SortedSet<ushort>> VerbsByType { get; set; }

        public Verblist(SortedSet<ushort> allVerbs)
        {
            AllVerbs = allVerbs;
            EmotionalVerbs = new SortedSet<ushort>();
            EmotionallyNeutralVerbs = new SortedSet<ushort>();
            CharacterEstablishingVerbs = new SortedSet<ushort>();
            VerbsByType = new SortedDictionary<VerbType, SortedSet<ushort>>()
            {
                { VerbType.Targeted, new SortedSet<ushort>() },
                { VerbType.Expressive, new SortedSet<ushort>() },
                { VerbType.Circumferential, new SortedSet<ushort>() },
                { VerbType.Physical, new SortedSet<ushort>() },
                { VerbType.Reactive, new SortedSet<ushort>() },
                { VerbType.Emotional, new SortedSet<ushort>() }
            };

            foreach (var verb in AllVerbs)
            {
                var verbInfo = Constants.VERBS_INFO[verb];

                VerbsByType[verbInfo.Type].Add(verb);

                if (verbInfo.EmotionalInclinations != null && verbInfo.EmotionalInclinations.Any())
                { 
                    foreach (var emotion in verbInfo.EmotionalInclinations)
                    {
                        if (VerbsByEmotion.TryGetValue(emotion, out SortedSet<ushort> emotionVerbs))
                            emotionVerbs.Add(verb);
                        else
                            VerbsByEmotion.Add(emotion, new SortedSet<ushort>() { verb });
                    }
                }
                else
                    EmotionallyNeutralVerbs.Add(verb);

                if (verbInfo.PersonalityInclination != null)
                    CharacterEstablishingVerbs.Add(verb);
            }

            EmotionalVerbs.UnionWith(AllVerbs);
            EmotionalVerbs.ExceptWith(EmotionallyNeutralVerbs);
        }

        // if it fails, then you can add a new verb
        public SortedSet<ushort> SelectReaction(ushort reactorID, Reaction reaction)
        {
            var reactor = Constants.CHARACTERS[reactorID];

            // reaction decreases with each subsequent instance
        }

        public SortedSet<ushort> SelectVerb(ushort entityID, SortedSet<ushort> witnesses)
        {
            var actor = Constants.CHARACTERS[entityID];

            (Emotion bareLeanings, Emotion flags, Emotion actualEmotion) = actor.Emotions.Feelings();
            bool emotional = flags != Emotion.None;
            SortedDictionary<ushort, UBoundedNumber> verbPropensities = emotional ? 
                new SortedDictionary<ushort, UBoundedNumber>(EmotionalVerbs.ToDictionary(k => k, k => new UBoundedNumber())) :
                new SortedDictionary<ushort, UBoundedNumber>();

            if (emotional)
            {
                foreach (var emotion in VerbsByEmotion)
                {
                    if (flags.HasFlag(emotion.Key))
                    {
                        UBoundedNumber totalEmotions = new UBoundedNumber();
                        if (emotion.Key.HasFlag(Emotion.Euphoric))
                            totalEmotions += actor.Emotions.Dysphoria_Euphoria;
                        else if (emotion.Key.HasFlag(Emotion.Dysphoric))
                            totalEmotions += -actor.Emotions.Dysphoria_Euphoria;

                        if (emotion.Key.HasFlag(Emotion.Passionate))
                            totalEmotions += actor.Emotions.Apathy_Passion;
                        else if (emotion.Key.HasFlag(Emotion.Apathetic))
                            totalEmotions += -actor.Emotions.Apathy_Passion;

                        if (emotion.Key.HasFlag(Emotion.Outward))
                            totalEmotions += actor.Emotions.Inward_Outward;
                        else if (emotion.Key.HasFlag(Emotion.Inward))
                            totalEmotions += -actor.Emotions.Inward_Outward;

                        if (emotion.Key.HasFlag(Emotion.Controlled))
                            totalEmotions += actor.Emotions.Primitive_Controlled;
                        else if (emotion.Key.HasFlag(Emotion.Primitive))
                            totalEmotions += -actor.Emotions.Primitive_Controlled;

                        foreach(var verb in emotion.Value)
                        {
                            verbPropensities[verb] += totalEmotions;
                        }
                    }
                }
            }

            UBoundedNumber totalEmotionsInverted = UBoundedNumber.FromUnbounded(
                actor.Emotions.Dysphoria_Euphoria.UInvert().UnboundedNumber +
                actor.Emotions.Apathy_Passion.UInvert().UnboundedNumber +
                actor.Emotions.Inward_Outward.UInvert().UnboundedNumber +
                actor.Emotions.Primitive_Controlled.UInvert().UnboundedNumber
            );
            foreach (var verb in EmotionallyNeutralVerbs)
            {
                verbPropensities.Add(verb, totalEmotionsInverted);
            }

            foreach(var verb in CharacterEstablishingVerbs)
            {
                if (verbPropensities.TryGetValue(verb, out UBoundedNumber propensity))
                    verbPropensities[verb] += Constants.VERBS_INFO[verb].PersonalityInclination(actor);
                else
                    verbPropensities.Add(verb, new UBoundedNumber(Constants.VERBS_INFO[verb].PersonalityInclination(actor)));
            }


        }
    }
}
