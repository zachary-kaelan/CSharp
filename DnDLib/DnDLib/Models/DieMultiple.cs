using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class DieMultiple
    {
        public byte Count { get; set; }
        public Die Die { get; set; }
        public float Average => ((((byte)Die) / 2) + 0.5f) * Count;
    }
}
