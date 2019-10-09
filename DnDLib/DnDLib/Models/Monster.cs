using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{

    public class Monster : Race
    {
        public string SwarmMonsterType { get; set; }

        public string Reactions { get; set; }
        public SortedDictionary<string, string> LegendaryActions { get; set; }
        public string Characteristics { get; set; }
        //public SortedDictionary<string, string> LandAndLair { get; set; }
        public string LairAndLairActions { get; set; }

        public byte ArmorClass { get; set; }
        public byte PassivePerception { get; set; }
        public bool HasLair { get; set; }
        public bool IsLegendary { get; set; }
        public float ChallengeRating { get; set; }

        public HPStats HPStats { get; set; }
        public Abilities SavingThrowProficiencies { get; set; }
        public DamageAdjustments[] DamageAdjustments { get; set; }
        public Environment Environments { get; set; }

        public string LanguageNoteOverride { get; set; }
        public MonsterTags[] Tags { get; set; }
        public SkillValue Skills { get; set; }
    }
}
