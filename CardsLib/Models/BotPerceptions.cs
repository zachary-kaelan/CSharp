using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public class BotPerceptions
    {
        public Hand Hand { get; private set; }
        public List<CardPriority> Priorities { get; private set; }
    }
}
