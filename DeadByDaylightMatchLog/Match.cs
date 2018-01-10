using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadByDaylightMatchLog
{
    public interface IMatch
    {
        int BloodPoints { get; set; }
        int Pips { get; set; }
        Dictionary<string, int> Offerings { get; set; }
        bool IsSurvivor { get; }

        DateTime EndedAt { get; set; }
        string ToListString();
    }

    public interface ISurvivorMatch : IMatch
    {
        string Survivor { get; set; }
        string Killer { get; set; }
        bool Died { get; set; }
        ItemOrPower Item { get; set; }
    }

    public interface IKillerMatch : IMatch
    {
        ItemOrPower Power { get; set; }
        string Killer { get; set; }
        Dictionary<string, int> Survivors { get; set; }
    }

    public struct ItemOrPower
    {
        public string Name { get; set; }
        public string Addon1 { get; set; }
        public string Addon2 { get; set; }
    }

    public struct SurvivorMatch : ISurvivorMatch
    {
        public int BloodPoints { get; set; }
        public int Pips { get; set; }
        public Dictionary<string, int> Offerings { get; set;  }
        public bool IsSurvivor => true;
        public string Killer { get; set; }

        public bool Died { get; set; }
        public DateTime EndedAt { get; set; }
        public string Survivor { get; set; }
        public ItemOrPower Item { get; set; }

        public string ToListString()
        {
            return Survivor + " vs. " + Killer + " - " + BloodPoints.ToString() + " (" + Pips.ToString() + ")";
        }
    }
    
    public struct KillerMatch : IKillerMatch
    {
        public int BloodPoints { get; set; }
        public int Pips { get; set; }
        public Dictionary<string, int> Offerings { get; set; }
        public bool IsSurvivor => false;
        public string Killer { get; set; }

        // Dictionary of Survivors and how many Killer Goals you got off of each.
        public DateTime EndedAt { get; set; }
        public Dictionary<string, int> Survivors { get; set; }
        public ItemOrPower Power { get; set; }

        public string ToListString()
        {
            return Killer + " vs. " + String.Join(", ", Survivors.Keys) + " - " + BloodPoints.ToString() + " (" + Pips.ToString() + ")";
        }
    }
}
