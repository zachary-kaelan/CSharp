using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMWorkshop.Monsters
{
    public enum Order
    {
        Lawful,
        Neutral,
        Chaotic
    }

    public enum Morality
    {
        Good,
        Neutral,
        Evil
    }

    public interface IMonsterTextAttribute
    {
        string Name { get; }
        string[] PrimeExamples { get; }

        string ToString();
    }

    public struct Size : IMonsterTextAttribute
    {
        public string Name { get; private set; }
        public int SquareFt { get; private set; }
        public string[] PrimeExamples { get; private set; }
        public int HitDie { get; private set; }
        public double AvgHPPerDie { get; private set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    public struct MonsterType : IMonsterTextAttribute
    {
        public string Name { get; private set; }
        public string[] PrimeExamples { get; private set; }
        public string[] Tags { get; private set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public struct Alignment
    {
        public Order Conformity { get; set; }
        public Morality Morales { get; set; }

        public override string ToString()
        {
            return Conformity.ToString() + " " + Morales.ToString();
        }
    }
    
    public struct AbilityScore : IMonsterTextAttribute
    {
        public string Name { get; private set; }
        public string[] PrimeExamples { get; private set; }
        public string Shorthand { get; private set; }
        public int Score { get { return Score;  } set { this.Score = value; this.Modifier = (Score - 10) / 2; } }
        public int Modifier { get; private set; }

        public static AbilityScore Strength(int score)
        {
            return new AbilityScore()
            {
                Name = "Strength",
                PrimeExamples = new string[] { "Barbarian", "Fighter", "Paladin" },
                Shorthand = "Str"
            };
        }
    }

    public struct AbilityScores
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
    }
}
