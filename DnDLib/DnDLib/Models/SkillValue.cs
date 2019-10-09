using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class SkillValue
    {
        public Skill Skill { get; set; }
        public byte BaseValue { get; set; }
        public sbyte Bonus { get; set; }
    }
}
