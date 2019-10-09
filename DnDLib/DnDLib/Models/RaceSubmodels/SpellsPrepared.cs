using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class SpellsPrepared
    {
        public byte Level { get; set; }
        public byte NumSlots { get; set; }
        public string[] Spells { get; set; }
    }
}
