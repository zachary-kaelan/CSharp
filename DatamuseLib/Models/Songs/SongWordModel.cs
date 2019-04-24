using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatamuseLib;
using Jil;

namespace DatamuseLib.Models.Songs
{
    public class SongWordModel
    {
        [JilDirective("defs")]
        public string[] Definitions { get; private set; }
        [JilDirective("defHeadword")]
        public string BaseForm { get; private set; }
        [JilDirective("speech_parts")]
        public PartOfSpeech PartsOfSpeech { get; private set; }
        [JilDirective("numSyllables")]
        public string[] SyllableCount { get; private set; }
        [JilDirective("pron")]
        public string[] Pronunciation { get; private set; }

        [JilDirective("numOccurences")]
        public int SongCount { get; private set; }
        [JilDirective("songFrequency")]
        public double SongFrequency { get; private set; }
        [JilDirective("frequency")]
        public double Frequency { get; private set; }
        [JilDirective("relative_frequency")]
        public double RelativeFrequency { get; private set; }
        [JilDirective("id")]
        public int ID { get; private set; }

        private static int IDCOUNTER = 0;
        public SongWordModel(FullLexicalModel model, int count, double frequency, double relativeFrequency)
        {
            SongCount = count;
            SongFrequency = frequency;
            RelativeFrequency = relativeFrequency;
            ID = IDCOUNTER;
            ++IDCOUNTER;

            Definitions = model.Definitions;
            BaseForm = model.BaseForm;
            PartsOfSpeech = model.PartsOfSpeech;
            Pronunciation = model.Pronunciation;
            Frequency = model.WordFrequency;
        }
    }
}
