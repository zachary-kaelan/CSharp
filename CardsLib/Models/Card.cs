using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class Card
    {
        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }
        public int Points { get; internal set; }
        public bool IsJoker { get; private set; }
        public bool IsUnknown { get; private set; }
        private string FullName { get; set; }

        internal Card()
        {
            Suit = Suit.UNKNOWN;
            Rank = Rank.UNKNOWN;
            Points = 0;
            IsJoker = false;
            IsUnknown = true;
        }

        internal Card(Suit suit, Rank rank, int points = 0)
        {
            Initialize(suit, rank, points);
        }

        internal void Initialize(Suit suit, Rank rank, int points = 0)
        {
            Suit = suit;
            Rank = rank;
            Points = points;
            if (Suit == Suit.JOKER)
            {
                FullName = "JOKER";
                IsJoker = true;
                IsUnknown = false;
            }
            else if (Suit == Suit.UNKNOWN)
                FullName = "UNKNOWN";
            else
                FullName = Rank.ToString().TrimStart('_') + " OF " + Suit.ToString();
        }

        public override string ToString() => FullName;
    }
}
