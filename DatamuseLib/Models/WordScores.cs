using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace DatamuseLib.Models
{
    public interface IWordScore
    {
        string Word { get; }
        int Score { get; }
    }

    public struct WordScore : IWordScore
    {
        [JilDirective("word")]
        public string Word { get; private set; }
        [JilDirective("score")]
        public int Score { get; private set; }
    }
}
