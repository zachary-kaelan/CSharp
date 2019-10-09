using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    [Flags]
    public enum Emotion
    {
        None = 0,        
        Dysphoric = 1,
        Euphoric = 16,
        Apathetic = 2,
        Passionate = 32,
        Inward = 4,
        Outward = 64,

        Primitive = 8,
        /* 
         * Catatonia
         * PassiveAggressiveness
         * Anxiety
         * Panic
         * ReligiousEctasy
         * InTune
         * Mania
         * SexualAttraction
         */

        Controlled = 128,
        /*
         * Depression
         * Nihilism
         * SelfLoathing
         * Anger
         * Meditation
         * PositiveVibes
         * Joy
         * Love
         */

        Depression =            Dysphoric | Apathetic,
        Catatonia =             Dysphoric | Apathetic | Inward | Primitive, // playing dead
        Unmotivated =           Dysphoric | Apathetic | Inward | Controlled, // lack of confidence
        PassiveAggressiveness = Dysphoric | Apathetic | Outward | Primitive,
        Nihilism =              Dysphoric | Apathetic | Outward | Controlled,

        Neuroticism =           Dysphoric | Passionate,
        Anxiety =               Dysphoric | Passionate | Inward | Primitive,
        SelfLoathing =          Dysphoric | Passionate | Inward | Controlled,
        Panic =                 Dysphoric | Passionate | Outward | Primitive,
        Paranoia =              Dysphoric | Passionate | Outward,
        Anger =                 Dysphoric | Passionate | Outward | Controlled, // animals don't experience anger

        Calmness =              Euphoric | Apathetic,
        ReligiousEctasy =       Euphoric | Apathetic | Inward | Primitive,
        Meditation =            Euphoric | Apathetic | Inward | Controlled,
        InTune =                Euphoric | Apathetic | Outward | Primitive, // feeling intuitively connected with your surroundings
        PositiveVibes =         Euphoric | Apathetic | Outward | Controlled,

        Mania =                 Euphoric | Passionate,
        Joy =                   Euphoric | Passionate | Inward | Primitive, // childlike; hyperactive
        Confident =             Euphoric | Passionate | Inward | Controlled,
        SexualAttraction =      Euphoric | Passionate | Outward | Primitive,
        Love =                  Euphoric | Passionate | Outward | Controlled
    }

    public class EmotionalState
    {
        public BoundedNumber Apathy_Passion { get; set; }
        public BoundedNumber Dysphoria_Euphoria { get; set; }
        public BoundedNumber Inward_Outward { get; set; }
        public BoundedNumber Primitive_Controlled { get; set; }

        private const float MIN_EMOTIONAL_SIGNIFICANCE = 0.15f;

        public EmotionalState()
        {
            Apathy_Passion = new BoundedNumber();
            Dysphoria_Euphoria = new BoundedNumber();
            Inward_Outward = new BoundedNumber();
            Primitive_Controlled = new BoundedNumber();
        }

        public (Emotion, Emotion, Emotion) Feelings()
        {
            Emotion bareLeanings = Emotion.None;
            if (Apathy_Passion.Number != 0)
                bareLeanings |= !Apathy_Passion ? Emotion.Apathetic : Emotion.Passionate;
            if (Dysphoria_Euphoria.Number != 0)
                bareLeanings |= !Dysphoria_Euphoria ? Emotion.Dysphoric : Emotion.Euphoric;
            if (Inward_Outward.Number != 0)
                bareLeanings |= !Inward_Outward ? Emotion.Inward : Emotion.Outward;
            if (Primitive_Controlled.Number != 0)
                bareLeanings |= !Primitive_Controlled ? Emotion.Primitive : Emotion.Controlled;

            Emotion flags = Emotion.None;
            if (Apathy_Passion.Significance() > MIN_EMOTIONAL_SIGNIFICANCE)
                flags |= !Apathy_Passion ? Emotion.Apathetic : Emotion.Passionate;
            if (Dysphoria_Euphoria.Significance() > MIN_EMOTIONAL_SIGNIFICANCE)
                flags |= !Dysphoria_Euphoria ? Emotion.Dysphoric : Emotion.Euphoric;
            if (Inward_Outward.Significance() > MIN_EMOTIONAL_SIGNIFICANCE)
                flags |= !Inward_Outward ? Emotion.Inward : Emotion.Outward;
            if (Primitive_Controlled.Significance() > MIN_EMOTIONAL_SIGNIFICANCE)
                flags |= !Primitive_Controlled ? Emotion.Primitive : Emotion.Controlled;

            var neuroticCalm = Neurotic_Calm();
            var depressiveManic = Depressive_Manic();
            Emotion actualEmotion = Emotion.None;
            UBoundedNumber totalSignificance = new UBoundedNumber();

            if (neuroticCalm.Significance() > depressiveManic.Significance())
            {
                totalSignificance = neuroticCalm.Significance();
                if (totalSignificance > MIN_EMOTIONAL_SIGNIFICANCE)
                {
                    if (!neuroticCalm)
                    {
                        actualEmotion |= Emotion.Passionate;
                        actualEmotion |= Emotion.Dysphoric;
                    }
                    else
                    {
                        actualEmotion |= Emotion.Apathetic;
                        actualEmotion |= Emotion.Euphoric;
                    }
                }
            }
            else
            {
                totalSignificance = depressiveManic.Significance();
                if (totalSignificance > MIN_EMOTIONAL_SIGNIFICANCE)
                {
                    if (!depressiveManic)
                    {
                        actualEmotion |= Emotion.Dysphoric;
                        actualEmotion |= Emotion.Apathetic;
                    }
                    else
                    {
                        actualEmotion |= Emotion.Euphoric;
                        actualEmotion |= Emotion.Passionate;
                    }
                }
            }

            return (bareLeanings, flags, actualEmotion);
        }

        // Inward_Outward determines "positive vibe emenance" or self-loathing
        // Primitive_Controlled determines religious stupor or passive-aggressiveness
        public BoundedNumber Neurotic_Calm() =>
            new BoundedNumber(
                (Apathy_Passion.Number +
                -Dysphoria_Euphoria.Number) / 2
            );

        // Inward_Outward determines how likely people are to be affected and/or notice
        // Primitive_Controlled determines how likely people are to commit you
        // High Inward_Outward indicates Hate_Love
        public BoundedNumber Depressive_Manic() =>
            new BoundedNumber(
                -(Apathy_Passion.Number +
                Dysphoria_Euphoria.Number) / 2
            );

        public EmotionalState Clone() =>
            new EmotionalState()
            {
                Dysphoria_Euphoria = Dysphoria_Euphoria,
                Apathy_Passion = Apathy_Passion,
                Inward_Outward = Inward_Outward,
                Primitive_Controlled = Primitive_Controlled
            };

    }
}
