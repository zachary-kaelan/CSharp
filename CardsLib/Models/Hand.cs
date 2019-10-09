using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class Hand : ICardCollection
    {
        private SortedSet<Card> Cards { get; set; }
        public int Unknown { get; private set; }
        public int Jokers { get; private set; }
        public int Count => Cards.Count;

        public Hand()
        {
            Cards = new SortedSet<Card>();
            Unknown = 0;
            Jokers = 0;
        }
    }
}
