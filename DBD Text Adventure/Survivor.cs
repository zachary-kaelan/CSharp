using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace DBD_Text_Adventure
{
    public enum Status
    {
        Healthy,
        Injured,
        Mitherless,
        Carried,
        Dying,
        Hooked,
        Dead,
        DCed,
        Nonexistent
    };

    public enum SurvivorName
    {
        Dwight,
        Meg,
        Claudette,
        Jake,
        Bill,
        Nea,
        Laurie,
        Ace,
        Feng,
        David,
        Quentin,
        Tapp
    };

    public enum SurvivorAction
    {
        Repairing,
        Cleansing,
        SelfHealing,
        BeingHealed,
        HealingAnother,
        Sneaking,
        Locker,
        Looping
    };

    [Flags]
    public enum SurvivorPower
    {
        None,
        SelfCare,
        DecisiveStrike,
        BorrowedTime,
        SprintBurst,
        Toolbox,
        Saboteur,
        IronWill,
        Flashlight,
        MedKit
    };

    public struct Survivor
    {
        public SurvivorName Name { get; set; }
        public bool IsObsession { get; set; }
        public Status Status { get; set; }
        public SurvivorAction CurrentAction { get; set; }
        public SurvivorPower Powers { get; set; }
        public int Rank { get; set; }
        public int Toxicity { get; set; }
        public int Saltiness { get; set; }

        public Survivor(int killerRank) : this()
        {
            Rank = Utils.RANDOM.Next(Math.Max(killerRank - 2, 1), 0);
            Toxicity = Utils.RANDOM.Next(5, 40) / Rank;
            Saltiness = Utils.RANDOM.Next(0, 25);

            double rankModifier = (Convert.ToDouble(Rank) / 50.0);
            double num = Math.Min(Utils.RANDOM.NextDouble() + rankModifier, 1.0);
            double num2 = Math.Min(Utils.RANDOM.NextDouble() + (rankModifier / 1.5), 1.0);
            if (num <= 0.20)
            {
                Name = SurvivorName.Claudette;
                Powers = SurvivorPower.SelfCare;
                Toxicity += Utils.RANDOM.Next(15, 40);
                Saltiness += 5;
            }
            else if (num <= 0.4)
            {
                Name = SurvivorName.Meg;
                Powers = SurvivorPower.SprintBurst;
                Toxicity += Utils.RANDOM.Next(10, 35);
                Saltiness += 5;
            }
            else if (num <= 0.6 && ((Saltiness * Toxicity) / num) >= 750)
            {
                Status = Status.Nonexistent;
                Powers = SurvivorPower.None;
            }
            else if (num <= 0.7 && ((Saltiness * Toxicity) / num) >= 1000)
            {
                Status = Status.DCed;
                Powers = SurvivorPower.None;
            }
            else if (num <= 0.8)
            {
                Name = SurvivorName.Dwight;
                Toxicity += Utils.RANDOM.Next(5, 20);
                if (num2 >= 0.5)
                    Powers = SurvivorPower.Toolbox;
                else
                    Powers = SurvivorPower.None;
            }
            else if (num <= 0.85)
            {
                Name = SurvivorName.Jake;
                Toxicity += Utils.RANDOM.Next(8, 32);
                Powers = SurvivorPower.Saboteur;
                if (num2 >= 0.6)
                    Powers &= SurvivorPower.IronWill;
                if (num2 >= 0.75)
                    Powers &= SurvivorPower.Toolbox;
            }
            else if (num <= 0.9)
            {
                Name = SurvivorName.Bill;
                Toxicity += Utils.RANDOM.Next(8, 32);
                Powers = SurvivorPower.BorrowedTime;
            }
            else
            {
                Name = (SurvivorName)Utils.RANDOM.Next(5, 12);
                if (Name == SurvivorName.Laurie)
                {
                    Powers = SurvivorPower.DecisiveStrike;
                    IsObsession = true;
                    Toxicity += Utils.RANDOM.Next(0, 15);
                }
                else
                    Powers = SurvivorPower.None;
                Toxicity += Utils.RANDOM.Next(5, 20);
            }

            double toxDouble = Convert.ToDouble(Toxicity);
            // Add perks
            MaybeAddPower(SurvivorPower.SelfCare, 0.5 / toxDouble);
            MaybeAddPower(SurvivorPower.SprintBurst, 0.75 / toxDouble);
            MaybeAddPower(SurvivorPower.BorrowedTime, 0.8 / toxDouble);
            MaybeAddPower(SurvivorPower.DecisiveStrike, 0.8 / toxDouble);
            MaybeAddPower(SurvivorPower.IronWill, 0.85 / toxDouble);

            // Add items
            if (!Powers.HasFlag(SurvivorPower.Toolbox))
            {
                double temp = 0.6 / toxDouble;
                if (Utils.RANDOM.NextDouble() >= temp)
                    Powers &= SurvivorPower.Flashlight;
                else if (Utils.RANDOM.NextDouble() >= temp)
                    Powers &= SurvivorPower.Toolbox;
                else if (Utils.RANDOM.NextDouble() >= temp)
                    Powers &= SurvivorPower.MedKit;
            }
        }

        public void MaybeAddPower(SurvivorPower power, double chance)
        {
            if (!Powers.HasFlag(power) && Utils.RANDOM.NextDouble() >= chance)
                Powers &= power;
        }
    }
}
