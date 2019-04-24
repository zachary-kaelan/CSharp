using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib
{
    public enum Suit
    {
        UNKNOWN = -1,
        JOKER = 0,
        HEARTS = 1,
        DIAMONDS = 2,
        CLUBS = 3,
        SPADES = 4
    }

    public enum Rank
    {
        UNKNOWN = -1,
        JOKER = 0,
        ACE = 1,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
        _10,
        JACK,
        QUEEN,
        KING
    }
}
