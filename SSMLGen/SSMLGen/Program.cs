using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Speech.Synthesis;
using System.Speech.Synthesis.TtsEngine;
using System.Speech.AudioFormat;

namespace SSMLGen
{
    class Program
    {
        static void Main(string[] args)
        {
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
            sre.SetInputToDefaultAudioDevice();
            var gb = new GrammarBuilder();
            gb.AppendDictation();
            sre.LoadGrammar(new Grammar(gb));
        }

        void sre_Recog(object sender, SpeechRecognizedEventArgs e)
        {
            var semantics = e.Result.ConstructSmlFromSemantics();
            var nav = semantics.CreateNavigator();

            var words = e.Result.Words;
            foreach (var word in words)
            {
                var audio = e.Result.GetAudioForWordRange(word, word);
                string phonetic = word.Pronunciation;
            }
        }
    }
}
