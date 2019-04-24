using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace DatamuseLib.Models
{
    public class FullLexicalModel
    {
        private bool _isInitialized = false;

        [JilDirective("defs")]
        public string[] Definitions { get; private set; }
        [JilDirective("defHeadword")]
        public string BaseForm { get; private set; }
        [JilDirective("word")]
        public string Word { get; private set; }
        [JilDirective("numSyllables")]
        public byte SyllableCount { get; private set; }

        [JilDirective("tags")]
        private string[] tags;

        private int _freq;
        [JilDirective(true)]
        public int WordFrequency
        {
            get
            {
                Initialize();
                return _freq;
            }
        }
        private PartOfSpeech _partsOfSpeech;
        [JilDirective(true)]
        public PartOfSpeech PartsOfSpeech
        {
            get
            {
                Initialize();
                return _partsOfSpeech;
            }
        }
        private string[] _pron;
        [JilDirective(true)]
        public string[] Pronunciation
        {
            get
            {
                Initialize();
                return _pron;
            }
        }

        private void Initialize()
        {
            if (!_isInitialized)
            {
                if (tags != null && tags.Any())
                {
                    _partsOfSpeech = PartOfSpeech.Unknown;
                    foreach (var tag in tags)
                    {
                        if (tag.StartsWith("pron:"))
                            _pron = tag.Substring(5).Split(' ');
                        else if (tag.StartsWith("f:"))
                            _freq = Convert.ToInt32(tag.Substring(2));
                        else
                            _partsOfSpeech |= API.PARTS_OF_SPEECH_DICT[tag];
                    }
                }
                _isInitialized = true;
            }
        }
    }
}
