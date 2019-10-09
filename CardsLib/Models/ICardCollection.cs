using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib.Models
{
    public interface ICardCollection
    {
        int Jokers { get; }
        int Unknown { get; }
        int Count { get; }
    }
}
