using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace CampSchedulesLib_v1.Models
{
    // TO-DO: Better integrate into DormInfo
    public class DormActivities
    {
        public Dorm Dorm { get; private set; }
        public Dictionary<string, int> Priorities { get; private set; }
        public string[] AvailableActivities { get; private set; }
        public Dictionary<Dorm, int> PartnerPriorities { get; private set; }
        public int PriorityPointsRemaining { get; set; }
        public static string[] ActivityNames = Enum.GetNames(typeof(Activity));

        public DormActivities(DormInfo dorm)
        {
            AvailableActivities = ActivityNames.Concat(
                dorm.AllowedSpecial.ToString().Split(
                    new string[] { " & " },
                    StringSplitOptions.None
                )
            ).ToArray();
            PartnerPriorities = new Dictionary<Dorm, int>();
            Dorm = dorm.Dorm;
            // The start of binary is 2^0, so everything is offset incorrectly by default
            int dormNum = Convert.ToInt32(Char.GetNumericValue(Dorm.ToString()[1])) - 1;
            int dormNumStart = Math.Max(0, dormNum - 1);

            int min = ZachMath.IntPow(2, dormNumStart);
            int boysMax = ZachMath.IntPow(2, Math.Min(5, dormNum + 1));
            int girlsMax = ZachMath.IntPow(2, Math.Min(6, dormNum + 1)) * 64;
            int boysMin = min;
            int girlsMin = min * 64;

            int boysOffset = 0;
            int boysConstant = 0;
            int girlsOffset = 0;
            int girlsConstant = 0;
            if (dorm.IsGirl)
            {
                girlsOffset = 1;
                girlsConstant = 4;
                boysOffset = 2;
                boysConstant = 5;
            }
            else
            {
                girlsOffset = 2;
                girlsConstant = 5;
                boysOffset = 1;
                boysConstant = 4;
            }

            int countingDormNum = dormNumStart;
            for (int i = boysMin; i <= boysMax; i *= 2)
            {
                PartnerPriorities.Add((Dorm)i, boysConstant - Math.Abs(dormNum - countingDormNum) * boysOffset);
                ++countingDormNum;
            }

            countingDormNum = dormNumStart;
            for (int i = girlsMin; i <= girlsMax; i *= 2)
            {
                PartnerPriorities.Add((Dorm)i, girlsConstant - Math.Abs(dormNum - countingDormNum) * girlsOffset);
                ++countingDormNum;
            }

            if (dormNum >= 4)
            {
                PartnerPriorities.Add(Dorm._1B, 1);
                PartnerPriorities.Add(Dorm._1G, 1);
            }
            else if (dormNum == 0)
            {
                PartnerPriorities.Add(Dorm._5B, 1);
                PartnerPriorities.Add(Dorm._5G, 1);
                PartnerPriorities.Add(Dorm._6B, 1);
                PartnerPriorities.Add(Dorm._6G, 1);
                PartnerPriorities.Add(Dorm._7G, 1);
            }

            PartnerPriorities.Remove(this.Dorm);
        }
    }
}
