using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Scheduling;

namespace CampSchedulesLib_v2.Models
{
    public class DormActivityOption : Thing, /*IComparable<DormActivityOption>, */ICloneable
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
        //internal SortedDictionary<int, int> _prevSorts;
        internal static int ID_COUNTER = 0;

        public DormActivityOption(int dorm, int activity, int dormActivityPriority, bool hasExcess = false, int duration = 1) : base(ID_COUNTER, dorm + "_" + activity)
        {
            ++ID_COUNTER;
            Dorm = dorm;
            Activity = activity;
            ActivityPriority = Schedule.Activities[activity].Priority;
            OtherDorm = -1;
            TotalPriority = ActivityPriority + dormActivityPriority;/* == 0 ? 0 :
                (2f * ActivityPriority * dormActivityPriority) / (ActivityPriority + dormActivityPriority);*/

            HasExcess = hasExcess;
            Duration = duration;
            if (activity != 15)
                HasExcess = false;

            SortIndex = -1;
            //_prevSorts = new SortedDictionary<int, int>();
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
            activityPriority += ActivityPriority;
            TotalPriority = (2f * activityPriority * DormPriority) / (activityPriority + DormPriority);
            //TotalPriority = Convert.ToInt32(Math.Round((ActivityPriority + OtherActivityPriority) / 2.0));

            HasExcess = false;
            Duration = duration;

            SortIndex = -1;

            //_prevSorts = new SortedDictionary<int, int>();
        }

        
        /*public int CompareTo(DormActivityOption other)
        {
            // result is x relative to y
            if (ID == other.ID)
                return 0;

            int temp = 0;
            if (TryGetPrevSort(other, out temp))
                return temp;

            if (IsRepeatedActivity && !other.IsRepeatedActivity)
                return 1;
            else if (!IsRepeatedActivity && other.IsRepeatedActivity)
                return -1;
            
            if (SortIndex == other.SortIndex)
            {
                if (HasOther && other.HasOther)
                    temp = OtherDormFunc().Options - other.OtherDormFunc().Options;
            }
            else
                temp = SortIndex - other.SortIndex;

            if (temp == 0 && Schedule.RANDOMNESS_ENABLED)
            {
                temp = Schedule.GEN.Next(-2, 2);
                _prevSorts.Add(other.ID, temp);
                return temp;
            }
            else
                return temp;
        }

        public bool TryGetPrevSort(DormActivityOption other, out int temp)
        {
            if (_prevSorts.TryGetValue(other.ID, out temp))
                return true;
            else if (other._prevSorts.TryGetValue(ID, out temp))
                return true;
            return false;
        }*/

        public object Clone()
        {
            var option = new DormActivityOption(Dorm, Activity, 0, OtherDormFunc, 0, Duration);
            option.DormPriority = this.DormPriority;
            option.ActivityPriority = this.ActivityPriority;
            option.TotalPriority = this.TotalPriority;
            option.HasExcess = this.HasExcess;

            option.IsRepeatedActivity = this.IsRepeatedActivity;
            option.IsRepeatedDorm = this.IsRepeatedDorm;
            option.IsRepeatedDormToday = this.IsRepeatedDormToday;
            option.SortIndex = this.SortIndex;
            option.SecondaryScore = this.SecondaryScore;

            //option._prevSorts = this._prevSorts;

            return option;
        }
    }
}
