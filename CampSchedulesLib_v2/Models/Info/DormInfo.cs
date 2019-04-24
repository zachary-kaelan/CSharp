using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.Info
{
    public class DormInfo : Thing
    {
        static DormInfo()
        {
            BoyIDs = new List<int>();
            GirlIDs = new List<int>();
        }

        public SortedSet<int> AllowedExclusiveActivities { get; private set; }
        public bool IsGirl { get; private set; }
        public int AgeGroup { get; private set; }
        internal int AgeIndex { get; set; }

        private static int ID_COUNTER = 0;
        public static List<int> BoyIDs { get; private set; }
        public static List<int> GirlIDs { get; private set; }

        public DormInfo(int ageGroup, bool isGirl = true, IEnumerable<int> allowedExclusive = null) : base(ID_COUNTER, ageGroup.ToString() + (isGirl ? "G" : "B"))
        {
            IsGirl = isGirl;
            AgeGroup = ageGroup;
            AllowedExclusiveActivities = new SortedSet<int>(allowedExclusive);
            ++ID_COUNTER;

            if (isGirl)
                GirlIDs.Add(ID);
            else
                BoyIDs.Add(ID);
        }

        public DormInfo(string abbrv, params int[] allowedExclusive) : base(ID_COUNTER, abbrv)
        {
            IsGirl = abbrv[1] == 'G';
            AgeGroup = Convert.ToInt32(char.GetNumericValue(abbrv[0]));
            AllowedExclusiveActivities = new SortedSet<int>(allowedExclusive);
            ++ID_COUNTER;

            if (IsGirl)
                GirlIDs.Add(ID);
            else
                BoyIDs.Add(ID);
        }

        public int GetAgeIndex(int boysMaxAgeGroup, int girlsMaxAgeGroup)
        {
            if (IsGirl)
            {
                --boysMaxAgeGroup;
                if (AgeGroup > boysMaxAgeGroup + 1)
                    return (boysMaxAgeGroup * 2) + (AgeGroup - boysMaxAgeGroup);
                else
                    return (AgeGroup * 2) - 1;
            }
            else
            {
                --girlsMaxAgeGroup;
                if (AgeGroup > girlsMaxAgeGroup + 1)
                    return (girlsMaxAgeGroup * 2) + (AgeGroup - (boysMaxAgeGroup + 1));
                else
                    return (AgeGroup * 2) - 2;
            }
        }
    }
}
