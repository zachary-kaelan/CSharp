using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WargamingAPI.Models;

namespace WargamingAPI
{
    /*public sealed class TankAverages : TankObject
    {
        public int battle_life_time { get; private set; }
        public long total_battle_life_time { get; private set; }
        public int number_of_players { get; private set; }
        public AllBattles all { get; private set; }
        public SpecialStats special { get; private set; }

        public struct SpecialStats
        {
            public double winrate { get; private set; }
            public double damageRatio { get; private set; }
            public double kdr { get; private set; }
            public double damagePerBattle { get; private set; }
            public double killsPerBattle { get; private set; }
            public double hitsPerBattle { get; private set; }
            public double spotsPerBattle { get; private set; }
            public double wpm { get; private set; }
            public double dpm { get; private set; }
            public double kpm { get; private set; }
            public double hitRate { get; private set; }
            public double survivalRate { get; private set; }
        }
    }*/

    public class TankAveragesTemp
    {
        public string Tank { get; set; }
        public int Tier { get; set; }
        public int Battles { get; set; }
        public double Winrate { get; set; }
        public double DR { get; set; }
        public double KDR { get; set; }
        public double DPB { get; set; }
        public double KPB { get; set; }
        public double HPB { get; set; }
        public double Spots { get; set; }
        public double WPM { get; set; }
        public int DPM { get; set; }
        public double KPM { get; set; }
        public double HitRate { get; set; }
        public double Survival { get; set; }
        public string Mastery { get; set; }
        public int Players { get; set; }
    }

    public class TankAverages
    {
        public int TankID { get; set; }
        public int TankTier { get; set; }
        public string TankType { get; set; }
        public string TankNation { get; set; }

        public int Battles { get; set; }
        public int Players { get; set; }
        public double DamageRatio { get; set; }
        public double KillDeathRatio { get; set; }

        public double WinRate { get; set; }
        public double DamagePerBattle { get; set; }
        public double KillsPerBattle { get; set; }
        public double HitsPerBattle { get; set; }
        public double SpotsPerBattle { get; set; }

        public double WinsPerMinute { get; set; }
        public double BattlesPerMinute { get; set; }
        public int DamagePerMinute { get; set; }
        public double KillsPerMinute { get; set; }

        public double Accuracy { get; set; }
        public double SurvivalRate { get; set; }
        public double WinSurvivalRate { get; set; }

        public TankAverages()
        {
            TankID = 0;
            TankTier = 0;
            TankType = null;
            TankNation = null;
            Battles = 0;
            Players = 0;
            DamageRatio = 0;
            KillDeathRatio = 0;
            WinRate = 0;
            DamagePerBattle = 0;
            KillsPerBattle = 0;
            HitsPerBattle = 0;
            SpotsPerBattle = 0;
            WinsPerMinute = 0;
            BattlesPerMinute = 0;
            DamagePerMinute = 0;
            KillsPerMinute = 0;
            Accuracy = 0;
            SurvivalRate = 0;
            WinSurvivalRate = 0;
        }

        public TankAverages(TankAveragesTemp averages, int tank_id, int tier, string type, string nation)
        {
            TankID = tank_id;
            TankTier = tier;
            TankType = type;
            TankNation = nation;

            Battles = averages.Battles;
            Players = averages.Players;
            DamageRatio = averages.DR;
            KillDeathRatio = averages.KDR;

            WinRate = averages.Winrate;
            DamagePerBattle = averages.DPB;
            KillsPerBattle = averages.KPB;
            HitsPerBattle = averages.HPB;
            SpotsPerBattle = averages.Spots;

            WinsPerMinute = averages.WPM;
            BattlesPerMinute = DamagePerMinute / DamagePerBattle;
            DamagePerMinute = averages.DPM;
            KillsPerMinute = averages.KPM;

            Accuracy = averages.HitRate;
            SurvivalRate = averages.Survival;
            WinSurvivalRate = SurvivalRate * WinRate;
        }

        public static PopularityAverages operator *(TankAverages avgs, double factor)
        {
            PopularityAverages newAvg = new PopularityAverages();

            newAvg.TankID = avgs.TankID;
            newAvg.TankTier = avgs.TankTier;
            newAvg.TankType = avgs.TankType;
            newAvg.TankNation = avgs.TankNation;

            newAvg.MinutesPerBattle = 1.0 / avgs.BattlesPerMinute;
            newAvg.Popularity = factor;

            newAvg.DamagePerBattle = avgs.DamagePerBattle * factor;
            newAvg.DamageTakenPerBattle = (newAvg.DamagePerBattle / avgs.DamageRatio) * factor;
            newAvg.KillsPerBattle = avgs.KillsPerBattle * factor;
            newAvg.DeathsPerBattle = (avgs.KillsPerBattle / avgs.KillDeathRatio) * factor;
            newAvg.HitsPerBattle = avgs.HitsPerBattle * factor;
            newAvg.SpotsPerBattle = avgs.SpotsPerBattle * factor;

            newAvg.WinRate = avgs.WinRate;
            newAvg.SurvivalRate = avgs.SurvivalRate;
            newAvg.WinSurvivalRate = avgs.WinSurvivalRate;

            return newAvg;
        }
    }

    public struct PopularityAverages
    {
        public int TankID { get; set; }
        public int TankTier { get; set; }
        public string TankType { get; set; }
        public string TankNation { get; set; }
        public string TankName { get; set; }

        public double MinutesPerBattle { get; set; }
        public double Popularity { get; set; }

        public double DamagePerBattle { get; set; }
        public double DamageTakenPerBattle { get; set; }
        public double KillsPerBattle { get; set; }
        public double DeathsPerBattle { get; set; }
        public double HitsPerBattle { get; set; }
        public double SpotsPerBattle { get; set; }

        public double WinRate { get; set; }
        public double SurvivalRate { get; set; }
        public double WinSurvivalRate { get; set; }

        public static PopularityAverages operator *(PopularityAverages avgs, double factor)
        {
            PopularityAverages newAvg = new PopularityAverages();

            newAvg.TankID = avgs.TankID;
            newAvg.TankTier = avgs.TankTier;
            newAvg.TankType = avgs.TankType;
            newAvg.TankNation = avgs.TankNation;

            newAvg.DamagePerBattle = avgs.DamagePerBattle * factor;
            newAvg.DamageTakenPerBattle = avgs.DamageTakenPerBattle * factor;
            newAvg.KillsPerBattle = avgs.KillsPerBattle * factor;
            newAvg.DeathsPerBattle = avgs.DeathsPerBattle * factor;
            newAvg.HitsPerBattle = avgs.HitsPerBattle * factor;
            newAvg.SpotsPerBattle = avgs.SpotsPerBattle * factor;

            return newAvg;
        }

        public static PopularityAverages operator +(PopularityAverages avgs, PopularityAverages other)
        {
            avgs.MinutesPerBattle += other.MinutesPerBattle * other.Popularity;
            avgs.Popularity += other.Popularity;

            avgs.DamagePerBattle += other.DamagePerBattle;
            avgs.DamageTakenPerBattle += other.DamageTakenPerBattle;
            avgs.KillsPerBattle += other.KillsPerBattle;
            avgs.DeathsPerBattle += other.DeathsPerBattle;
            avgs.HitsPerBattle += other.HitsPerBattle;
            avgs.SpotsPerBattle += other.SpotsPerBattle;

            avgs.WinRate += other.WinRate * other.Popularity;
            avgs.SurvivalRate += other.SurvivalRate * other.Popularity;
            avgs.WinSurvivalRate += other.WinSurvivalRate * other.Popularity;

            return avgs;
        }

        public override string ToString()
        {
            return TankName + " - " + TankNation + " - " + TankTier.ToString();
        }
    }

    public class PopularityProfile : IReadOnlyDictionary<int, PopularityAverages>
    {
        private SortedDictionary<int, PopularityAverages> Averages { get; set; }
        public PopularityAverages Totals { get; private set; }
        private double BattleTimeSlope { get; set; }
        private double BattleTimeIntercept { get; set; }
        public SortedDictionary<int, double> StallFactors { get; private set; }
        public double ProfileStallFactor { get; private set; }
        public int MaxTier { get; private set; }
        public int MinTier { get; private set; }

        #region IReadOnlyDictionary Implementation
        public IEnumerable<int> Keys => Averages.Keys;

        public IEnumerable<PopularityAverages> Values => Averages.Values;

        public int Count => Averages.Count;

        public PopularityAverages this[int key] => Averages[key];

        public bool ContainsKey(int key)
        {
            return Averages.ContainsKey(key);
        }

        public bool TryGetValue(int key, out PopularityAverages value)
        {
            return Averages.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<int, PopularityAverages>> GetEnumerator()
        {
            return Averages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Averages.GetEnumerator();
        }
        #endregion

        public PopularityProfile(IDictionary<int, PopularityAverages> profile, PopularityAverages totals, KeyValuePair<double, double> battleTime, KeyValuePair<int, int> tierRange)
        {
            Averages = new SortedDictionary<int, PopularityAverages>(profile);
            Totals = totals;

            BattleTimeSlope = battleTime.Key;
            BattleTimeIntercept = battleTime.Value;
            MaxTier = tierRange.Value;
            MinTier = tierRange.Key;

            ProfileStallFactor = ((BattleTimeSlope * Totals.SurvivalRate) + BattleTimeIntercept) / Totals.MinutesPerBattle;
            StallFactors = new SortedDictionary<int, double>(
                profile.Values.ToDictionary(
                    v => v.TankID,
                    v => ((BattleTimeSlope * v.SurvivalRate) + BattleTimeIntercept) / v.MinutesPerBattle
                )
            );
        }

        private const string TOSTRING_FORMAT = "{0} to {1}; {2}m{3}s to {4}m{5}s";
        public override string ToString()
        {
            int minMinutes = Convert.ToInt32(Math.Floor(BattleTimeIntercept));
            int minSeconds = Convert.ToInt32(Math.Round((BattleTimeIntercept % 1) * 60));
            if (minSeconds == 60)
            {
                ++minMinutes;
                minSeconds = 0;
            }
            double max = BattleTimeSlope + BattleTimeIntercept;
            int maxMinutes = Convert.ToInt32(Math.Floor(max));
            int maxSeconds = Convert.ToInt32(Math.Round((max % 1) * 60));
            if (maxSeconds == 60)
            {
                ++maxMinutes;
                maxSeconds = 0;
            }

            return String.Format(
                TOSTRING_FORMAT,
                MinTier,
                MaxTier,
                minMinutes,
                minSeconds,
                maxMinutes,
                maxSeconds
            );
        }
    }
}
