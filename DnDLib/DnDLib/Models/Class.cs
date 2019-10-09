using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class Class
    {
        public string Name { get; set; }
        public Die HitDie { get; set; }
        public Abilities PrimaryAbilities { get; set; }
        public Abilities SavingThrowProficiencies { get; set; }

        public Skill SkillsToChooseFrom { get; set; }
        public byte NumSkillsToChoose { get; set; }
        public DieMultiple StartingWealth { get; set; }
    }
}
