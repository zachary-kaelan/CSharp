using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class Bot : Player
    {
        private BotPerceptions[] Perceptions { get; set; }
        private List<CardPriority> Priorities { get; set; }

        public Bot(Hand hand) : base(hand)
        {
            IsBot = true;
        }

        public void Initialize(int numPlayers, Card topCard)
        {

        }
    }
}
