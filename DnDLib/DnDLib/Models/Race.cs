using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class Race
    {
        public string Name { get; set; }

        public MonsterType Type { get; set; }
        public MonsterSubtype Subtype { get; set; }
        public Size Size { get; set; }
        public Alignment Alignment { get; set; }
        
        public SortedDictionary<string, string> SpecialTraits { get; set; }
        public SortedDictionary<string, string> Actions { get; set; }

        public AbilityScores AbilityScores { get; set; }
        public Condition ConditionImmunities { get; set; }
        public Language Languages { get; set; }
        public MonsterSense[] Senses { get; set; }
        public Movement[] MovementTypes { get; set; }
    }
}
