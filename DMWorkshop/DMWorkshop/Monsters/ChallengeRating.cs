using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMWorkshop.Monsters
{
    public static class ChallengeRatingStats
    {
        public static int ProficiencyBonus(int CR)
        {
            if (CR < 5)
                return 2;
            else if (CR < 9)
                return 3;
            else if (CR < 13)
                return 4;
            else if (CR < 17)
                return 5;
            else if (CR < 21)
                return 6;
            else if (CR < 25)
                return 7;
            else if (CR < 29)
                return 8;
            else
                return 9;
        }

        private static readonly Dictionary<double, int> EXPDict = new Dictionary<double, int>()
        {
            {0, 0},
            {0.125, 25},
            {0.25, 50},
            {0.5, 100},
            {1, 200},
            {2, 450},
            {3, 700},
            {4, 1100 },
            {5,1800 },
            {6,2300 },
            {7,2900 },
            {8,3900 },
            {9, 5000 },
            {10, 5900 },
            {11, 7200},
            {12, 8400},
            {13, 10000},
            {14, 11500},
            {15, 13000},
            {16, 15000},
            {17, 18000},
            {18, 20000},
            {19, 22000},
            {20, 25000},
            {21, 33000},
            {22, 41000},
            {23, 50000},
            {24, 62000},
            {25, 75000},
            {26, 90000},
            {27, 105000},
            {28, 120000},
            {29, 135000},
            {30, 155000}
        };
        public static int ExperiencePoints(double CR)
        {

        }
    }
}
