using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace DatamuseLib.Models
{
    public struct MeansLikeScore : IWordScore
    {
        [JilDirective("word")]
        public string Word { get; private set; }
        [JilDirective("score")]
        public int Score { get; private set; }
        [JilDirective("tags")]
        public string[] Tags { get; private set; }
    }
}
