using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class SpellCasting
    {
        public byte SpellcasterLevel { get; set; }
        public Abilities SpellcastingAbility { get; set; }
        public byte SaveDifficulty { get; set; }
        public sbyte HitModifier { get; set; }
    }
}
