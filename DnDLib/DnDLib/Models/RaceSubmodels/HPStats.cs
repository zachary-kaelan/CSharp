using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class HPStats
    {
        public DieMultiple HPDice { get; set; }
        public sbyte HPModifier { get; set; }
        public ushort HPAverage => (ushort)Math.Round(HPDice.Average + HPModifier);
    }
}
