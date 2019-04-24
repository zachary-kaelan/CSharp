using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace DBD_Text_Adventure
{
    public enum KillerName
    {
        Trapper,
        Wraith,
        Hillbilly,
        Nurse,
        Shape,
        Hag,
        Doctor,
        Huntress,
        Cannibal,
        Nightmare,
        Pig
    };

    public enum KillerAction
    {
        Carrying,
        Stunned,
        Blinded,
        Patroling,
        Camping,
        Hunting,
        Chasing
    };

    [Flags]
    public enum KillerPower
    {
        None,
        Insidious,
        BBQ,
        Ruin,
        NursesCalling,
        BrutalStrength,
        Enduring,
        NOED,
        Agitation,
        Whispers,
        ThrillOfTheHunt,
        FranklinsDemise,
        Overcharge,
        Bloodhound
    }

    public struct Killer
    {
        public KillerName Name { get; set; }
        public KillerAction CurrentAction { get; set; }
        public KillerPower Powers { get; set; }
        public int Rank { get; set; }
        public int Toxicity { get; set; }
        public int Saltiness { get; set; }

        public Killer(int killerRank, int saltiness, KillerName name, KillerPower powers) : this()
        {
            Rank = killerRank;
            Saltiness = saltiness;
            Name = name;
            Powers = powers;

            bool extraPerk = Utils.RANDOM.NextDouble() >= 0.25;
            if (powers == KillerPower.None)
            {
                switch(name)
                {
                    case KillerName.Trapper:
                        Powers &= KillerPower.BrutalStrength;
                        if (extraPerk)
                            Powers &= KillerPower.Agitation;
                        break;

                    case KillerName.Cannibal:
                        Powers &= KillerPower.BBQ;
                        if (extraPerk)
                            Powers &= KillerPower.FranklinsDemise;
                        break;

                    case KillerName.Doctor:
                        Powers &= KillerPower.Overcharge;
                        break;

                    case KillerName.Hag:
                        Powers &= KillerPower.Ruin;
                        break;

                    case KillerName.Hillbilly:
                        Powers &= KillerPower.Enduring;
                        break;

                    case KillerName.Nurse:
                        Powers &= KillerPower.NursesCalling;
                        break;
                }
            }
        }

        public Killer(int killerRank, int saltiness, KillerPower powers) : this(killerRank, saltiness, GetRandomName(), powers)
        {

        }

        private static KillerName GetRandomName()
        {
            int num = Utils.RANDOM.Next(1, 101);

            if (num <= 16)
                return KillerName.Shape;
            else if (num <= 29)
                return KillerName.Hillbilly;
            else if (num <= 42)
                return KillerName.Huntress;
            else if (num <= 53)
                return KillerName.Pig;
            else if (num <= 63)
                return KillerName.Nurse;
            else if (num <= 73)
                return KillerName.Doctor;
            else if (num <= 81)
                return KillerName.Wraith;
            else if (num <= 87)
                return KillerName.Trapper;
            else if (num <= 93)
                return KillerName.Cannibal;
            else if (num <= 97)
                return KillerName.Nightmare;
            else
                return KillerName.Hag;
        }
    }
}
