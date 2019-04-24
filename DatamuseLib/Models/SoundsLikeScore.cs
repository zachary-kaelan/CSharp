using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace DatamuseLib.Models
{
    public struct SoundsLikeScore : IWordScore
    {
        [JilDirective("word")]
        public string Word { get; private set; }
        [JilDirective("score")]
        public int Score { get; private set; }
        [JilDirective("numSyllables")]
        public int SyllableCount { get; private set; }
    }
}
