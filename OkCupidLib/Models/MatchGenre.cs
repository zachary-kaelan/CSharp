using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class MatchGenre
    {
        public byte enemy { get; protected set; }
        public bool insufficient { get; protected set; }
        public string label { get; protected set; }
        public byte match { get; protected set; }
        public int mutual_answered { get; protected set; }
        public string rel { get; protected set; }
        public int they_answered { get; protected set; }
        public int you_answered { get; protected set; }
    }
}
