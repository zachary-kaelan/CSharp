using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Scheduling;

namespace CampSchedulesLib_v2.Models
{
    public class DormActivityOption : Thing, IComparable<DormActivityOption>
    {
        /*[Flags]
        public enum OptionFlags
        {
            None,
            HasExcess = 1,

        }*/

        public int Dorm { get; private set; }
        public int Activity { get; private set; }
        public int OtherDorm { get; private set; }
        public bool HasOther => OtherDorm != -1;
        public int Duration { get; private set; }

        public int DormPriority { get; private set; }
        //public int OtherDormPriority { get; private set; }
        public int ActivityPriority { get; private set; }
        public double TotalPriority { get; private set; }
        public bool HasExcess { get; private set; }

        public bool IsRepeatedActivity { get; set; }
        public bool IsRepeatedDorm { get; set; }
        public bool IsRepeatedDormToday { get; set; }
        public int SortIndex { get; set; }
        public int SecondaryScore { get; set; }
        public Func<InterDormTracking> OtherDormFunc { get; private set; }

        internal static int ID_COUNTER = 0;

        public DormActivityOption(int dorm, int activity, int dormActivityPriority, bool hasExcess = false, int duration = 1) : base(ID_COUNTER, dorm + "_" + activity)
        {
            ++ID_COUNTER;
            Dorm = dorm;
            Activity = activity;
            ActivityPriority = Schedule.Activities[activity].Priority;
            OtherDorm = -1;
            TotalPriority = ActivityPriority + dormActivityPriority == 0 ? 0 :
                (ActivityPriority * dormActivityPriority) / (ActivityPriority + dormActivityPriority);

            HasExcess = hasExcess;
            Duration = duration;
            if (activity != 15)
                HasExcess = false;

            SortIndex = -1;
        }

        public DormActivityOption(int dorm, int activity, int dormActivityPriority, Func<InterDormTracking> otherDormFunc, int otherDormActivityPriority/*, int otherDormPriority*/, int duration = 1) : base(ID_COUNTER, otherDormFunc().Abbreviation + "_" + activity)
        {
            ++ID_COUNTER;
            Dorm = dorm;
            Activity = activity;
            ActivityPriority = Schedule.Activities[activity].Priority;

            OtherDormFunc = otherDormFunc;
            var otherDorm = OtherDormFunc();
            OtherDorm = otherDorm.Dorm;

            DormPriority = otherDorm.Priority;
            //OtherDormPriority = otherDormPriority;
            var activityPriority = 
                (dormActivityPriority == 0 || otherDormActivityPriority == 0) ? 0f :
                    (2f * dormActivityPriority * otherDormActivityPriority) / (dormActivityPriority + otherDormActivityPriority);
            TotalPriority = (activityPriority + DormPriority + ActivityPriority) / 3;
            //TotalPriority = Convert.ToInt32(Math.Round((ActivityPriority + OtherActivityPriority) / 2.0));

            HasExcess = false;
            Duration = duration;

            SortIndex = -1;
        }

        public int CompareTo(DormActivityOption other)
        {
            // result is x relative to y
            if (ID == other.ID)
                return 0;
            int temp = 0;
            if (SortIndex == other.SortIndex)
            {
                if (HasOther && other.HasOther)
                    temp = OtherDormFunc().Options - other.OtherDormFunc().Options;
            }
            else
                temp = SortIndex - other.SortIndex;

            if (temp == 0 && Schedule.RANDOMNESS_ENABLED)
                return Schedule.GEN.Next(-1, 2);
            else
                return temp;
        }
    }
}
