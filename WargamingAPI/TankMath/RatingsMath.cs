using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingAPI.TankMath
{
    public static class RatingsMath
    {
        public static double EfficiencyRating(double tier, double avgDamage, double avgKills, double avgSpots, double avgCapPoints, double avgDefensePoints)
        {
            return (avgDamage * (10.0 / (tier + 2)) * (0.21 + ((3 * tier) / 100.0))) +
                   (avgKills * 250) +
                   (avgSpots * 150) +
                   ((Math.Log(avgCapPoints + 1.0) / Math.Log(1.732)) * 150) +
                   (avgDefensePoints * 150);
        }

        private static double TierCoefficient(int tierMax, int tierMin, int tierCurr)
        {
            return ((tierMax + tierMin) / 2.0) - tierCurr;
        }

        private static double TierCoefficient(double tierCurr)
        {
            return 5.5 - tierCurr;
        }

        private static double TierExperienceCoefficient(int numBattles)
        {
            if (numBattles <= 50)
                return 0;
            else if (numBattles <= 500)
                return (numBattles - 50) / 1000.0;
            else if (numBattles <= 1000)
                return 0.45 + ((numBattles - 500) / 2000.0);
            else if (numBattles <= 2000)
                return 0.7 + ((numBattles - 1000) / 4000.0);
            else
                return 0.95 + ((numBattles - 2000) / 8000.0);
        }

        private static double TotalExperienceCoefficient(int totalNumBattles)
        {
            if (totalNumBattles <= 500)
                return 0;
            else if (totalNumBattles <= 5000)
                return (totalNumBattles - 500) / 10000.0;
            else if (totalNumBattles <= 10000)
                return 0.45 + ((totalNumBattles - 5000) / 20000.0);
            else if (totalNumBattles <= 20000)
                return 0.7 + ((totalNumBattles - 10000) / 40000.0);
            else
                return 0.95 + ((totalNumBattles - 20000) / 80000.0);
        }

        private static double TankProficiencyCoefficient(double playerTankWinRate, double tankWinRate)
        {
            return (100.0 + (playerTankWinRate - tankWinRate)) / 100.0;
        }

        private static double ProficiencyCoefficient(double playerWinRate)
        {
            return TankProficiencyCoefficient(playerWinRate, 48.5);
        }

        private static double EfficiencyCoefficient(double accountEfficiency, double avgTier, int currTier)
        {
            return accountEfficiency + (accountEfficiency * ((avgTier - currTier) * 0.05));
        }

        public static double BattleEffectiveness(double tankEff, double tierCoeff, double tierXPCoeff, double totalXPCoeff, double tankProfCoeff, double profCoeff, double playerEffCoeff)
        {
            return ((((3.0 / 5.0) * tankEff * tankProfCoeff) * (tankProfCoeff + tierXPCoeff)) + (((2.0 / 5.0) * playerEffCoeff * profCoeff) * (profCoeff + totalXPCoeff))) * (profCoeff + (0.25 * tierCoeff));
        }

        public static double FirstTimeEffectiveness(double tierCoeff, double totalXPCoeff, double profCoeff, double playerEffCoeff)
        {
            return ((playerEffCoeff * profCoeff) * (profCoeff + totalXPCoeff)) * (profCoeff + (0.25 * tierCoeff));
        }

        public static double NormalizedBattleEffectiveness(double battleEffectiveness)
        {
            if (battleEffectiveness <= 0)
                return 0;
            else if (battleEffectiveness <= 200)
                return battleEffectiveness;
            else
                return 200.0;
        }

        public static double TeamBattleEffectiveness(double[] team)
        {
            return team.Sum(p => NormalizedBattleEffectiveness(p));
        }

        public static double TeamWinProbability(double alliedBattleEffectiveness, double enemyBattleEffectiveness)
        {
            double winChance = (0.5 + (((alliedBattleEffectiveness / (alliedBattleEffectiveness + enemyBattleEffectiveness)) - 0.5) * 1.5)) * 100.0;
            if (winChance > 95)
                return 95;
            else if (winChance < 5)
                return 5;
            return winChance;
        }
    }
}
