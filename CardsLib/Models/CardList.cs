using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class CardList
    {
        private List<Card> Cards { get; set; }
        public int Unknown { get; private set; }
        public int Jokers { get; private set; }
        public int Count { get; private set; }

        public CardList()
        {
            Cards = new List<Card>();
        }

        public CardList(IEnumerable<Card> cards)
        {
            Cards = new List<Card>(cards);
        }

        public CardList(int capacity)
        {
            Cards = new List<Card>(capacity);
        }

        public CardList(IEnumerable<Card> cards, int capacity) : this(capacity)
        {
            Cards.AddRange(cards);
        }

        private void Initialize()
        {
            Count = Cards.Count;
            Unknown = 0;
            Jokers = 0;
            for (int i = 0; i < Count; ++i)
            {
                var card = Cards[i];
                if (card.IsUnknown)
                    ++Unknown;
                else if (card.IsJoker)
                    ++Jokers;
            }
        }
    }
}
