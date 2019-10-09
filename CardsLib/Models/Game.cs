using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class Game
    {
        private Deck DrawPile { get; set; }
        private Deck DiscardPile { get; set; }

        public Game(int numPlayers)
        {
            if (numPlayers <= 4)
                DrawPile = new Deck(2, 4 + (numPlayers == 3 ? 0 : 2));
            else
                DrawPile = new Deck(3, 6 + (numPlayers == 5 ? 2 : 4));
            DiscardPile = new Deck();
        }
    }
}
