using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace DBD_Text_Adventure
{
    

    public static class Scenarios
    {
        public static IEnumerable<string> StartingChoices = new string[]
        {
            "Patrol generators.",
            "Head for my totem(s).",
            "DC.",
            "Turn on my bot script."
        };

        public static string[] SequentialNumbers = new string[]
        {
            "",
            "first",
            "second",
            "third",
            "fourth",
            "fifth",
            "sixth",
            "seventh"
        };
    }

    public class Game
    {
        public Killer Killer { get; set; }
        public Survivor[] Survivors { get; set; }
        public int GensRemaining { get; set; }
        public bool HatchOpen { get; private set; }
        public bool GameOver { get; set; }

        public Game(int startingChoice, Killer killer)
        {
            Survivors = Enumerable.Range(0, 4).Select(s => new Survivor(killer.Rank)).Where(s => s.Status != Status.Nonexistent).ToArray();
            GensRemaining = 5 - (4 - Survivors.Length);
            GameOver = !Survivors.Any() || Survivors.All(s => s.Status == Status.DCed);
            Killer = killer;

            int saltModifier = Utils.RANDOM.Next(5, 10) * (4 - Survivors.Count(s => s.Status != Status.DCed));
            switch(killer.Name)
            {
                case KillerName.Doctor:
                    saltModifier += Utils.RANDOM.Next(10, 20);
                    break;

                case KillerName.Nightmare:
                    saltModifier += Utils.RANDOM.Next(5, 30);
                    break;

                case KillerName.Nurse:
                    saltModifier += Utils.RANDOM.Next(0, 15);
                    break;
            }

            for(int i = 0; i < Survivors.Length; ++i)
            {
                Survivors[i].Saltiness += saltModifier;
            }
        }

        public Game(int startingChoice, int killerRank) : this(startingChoice, new Killer(killerRank))
        {
        }

        public void Patrol()
        {

        }
    }
}
