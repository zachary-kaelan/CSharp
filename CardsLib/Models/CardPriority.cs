using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class CardPriority
    {
        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }
        public int Priority { get; set; }

        public CardPriority(Suit suit, Rank rank, int priority = 1)
        {
            Suit = suit;
            Rank = rank;
            Priority = priority;
        }
    }
}
