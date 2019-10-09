using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class Player
    {
        public int NumBuys { get; private set; }
        private Hand Hand { get; set; }
        public bool IsBot { get; protected set; }

        public Player(Hand hand)
        {
            Hand = hand;
            NumBuys = 3;
        }
    }
}
