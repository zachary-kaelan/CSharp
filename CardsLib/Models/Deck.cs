using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class Deck : ICardCollection
    {
        private Stack<Card> Cards { get; set; }
        public int Unknown { get; private set; }
        public int Jokers { get; private set; }
        public int Count { get; private set; }

        public Deck()
        {
            Cards = new Stack<Card>();
        }

        public Deck(IEnumerable<Card> cards)
        {
            Cards = new Stack<Card>(cards);
        }

        public Deck(int numDecks, int numJokers)
        {
            Cards = new Stack<Card>();
            Unknown = 52 * numDecks;
            Jokers = numJokers;
            Count = Unknown + Jokers;
            List<Card> cards = new List<Card>(Count);

            for (int i = 1; i <= 4; ++i)
            {
                Suit suit = (Suit)i;
                for (int j = 1; j <= 13; ++j)
                {
                    cards.AddRange(Enumerable.Repeat(new Card(suit, (Rank)j), numDecks));
                }
            }
        }

        private void Initialize()
        {
            Count = Cards.Count;
            Unknown = 0;
            Jokers = 0;
            foreach(var card in Cards)
            {
                if (card.IsUnknown)
                    ++Unknown;
                else if (card.IsJoker)
                    ++Jokers;
            }
        }
    }
}
