using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Info;
using CampSchedulesLib_v2.Models.CSV;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    public class DormActivities : ICloneable
    {
        public SortedSet<int> OtherDorms { get; private set; }
        public SortedSet<int> AvailableActivities { get; private set; }
        public SortedSet<int> AvailableMultiDormActivities { get; private set; }
        public SortedSet<int> AvailableActivitiesToday { get; private set; }
        public SortedSet<int> AvailableMultiDormActivitiesToday { get; private set; }
        public SortedDictionary<int, int> ActivityPriorities { get; private set; }
        public SortedDictionary<int, InterDormTracking> DormPriorities { get; private set; }
        public SortedSet<string> ScheduleHistory { get; private set; }
        public int Dorm { get; private set; }
        public SortedSet<int> RepeatableHistory { get; private set; }
        public SortedSet<int> RepeatableTodayHistory { get; private set; }
        public SortedSet<int> RepeatableDoubleHistory { get; private set; }
        public SortedSet<int> OtherDormsDoneToday { get; private set; }
        public List<InterDormTracking> UsedUpOtherDorms { get; private set; }
        private int TotalManualDuration { get; set; }
        public int TotalActivityDuration { get; private set; }
        public int TotalScheduledDuration { get; set; }
        public float DurationScore {
            get
            {
                var multiplier = TotalActivityDuration / (Schedule.TotalBlocksDuration - TotalManualDuration);
                var initialScore = TotalScheduledDuration / (Schedule.TotalBlocksDuration - TotalManualDuration);
                var finalScore = (1 - initialScore) / multiplier;
                return finalScore;
            }
        }
        //public int RemainderPriority { get; private set; }

        public DormActivities(DormInfo dorm, IDictionary<int, int> priorities)
        {
            Dorm = dorm.ID;
            ScheduleHistory = new SortedSet<string>();
            AvailableActivities = new SortedSet<int>(ActivityInfo.ActivityIDs);
            if (dorm.AllowedExclusiveActivities != null && dorm.AllowedExclusiveActivities.Count > 0)
                AvailableActivities.UnionWith(dorm.AllowedExclusiveActivities);
            ActivityPriorities = new SortedDictionary<int, int>(priorities);
            foreach(var activity in AvailableActivities)
            {
                var info = Schedule.Activities[activity];
                if (!info.Flags.HasFlag(ActivityFlags.Manual) && (!info.Flags.HasFlag(ActivityFlags.Exclusive) || !info.Flags.HasFlag(ActivityFlags.Excess)))
                {
                    TotalActivityDuration += info.Duration;
                    if (info.Flags.HasFlag(ActivityFlags.Repeatable))
                        TotalActivityDuration += info.Duration;
                }
            }
            OtherDorms = new SortedSet<int>();
            DormPriorities = new SortedDictionary<int, InterDormTracking>();
            RepeatableHistory = new SortedSet<int>();
            RepeatableTodayHistory = new SortedSet<int>();
            RepeatableDoubleHistory = new SortedSet<int>();

            AvailableActivitiesToday = new SortedSet<int>();
            AvailableMultiDormActivities = new SortedSet<int>(AvailableActivities);
            AvailableMultiDormActivities.IntersectWith(ActivityInfo.MultiDormActivities);
            AvailableMultiDormActivitiesToday = new SortedSet<int>();

            OtherDormsDoneToday = new SortedSet<int>();
            UsedUpOtherDorms = new List<InterDormTracking>();
            //RemainderPriority = 0;
        }

        internal int ClearFromHistory(ScheduledActivity scheduled, bool ignoreHasOther = false)
        {
            ScheduleHistory.Remove(scheduled.Abbreviation);
            var activity = Schedule.Activities[scheduled.Activity];
            bool isRepeatable = activity.Flags.HasFlag(ActivityFlags.Repeatable);
            TotalScheduledDuration -= scheduled.Duration;

            int output = -1;

            if (!scheduled.HasOther)
            {
                var tracker = DormPriorities[Dorm];
                tracker.ScheduleHistory.Remove(scheduled.Abbreviation);
                tracker.PreviousRepeatableActivities.Remove(scheduled.Activity);
            }

            if (!isRepeatable || !RepeatableDoubleHistory.Remove(scheduled.Activity))
            {
                if (isRepeatable)
                {
                    if (!AvailableActivities.Add(activity.ID))
                        RepeatableHistory.Remove(activity.ID);
                    RepeatableTodayHistory.Remove(activity.ID);
                }
                else
                    AvailableActivities.Add(activity.ID);

                AvailableActivitiesToday.Add(activity.ID);
                if (activity.Flags.HasFlag(ActivityFlags.MultiDorm))
                {
                    output = 0;
                    if (AvailableMultiDormActivitiesToday.Add(activity.ID))
                        output = 1;
                    if (AvailableMultiDormActivities.Add(activity.ID))
                        output = 2;
                }
            }
            else
            {
                //RepeatableTodayHistory.Remove(activity.ID);
            }

            if (scheduled.HasOther && !ignoreHasOther)
            {
                OtherDormsDoneToday.Remove(scheduled.OtherDorm);
                if (!DormPriorities.ContainsKey(scheduled.OtherDorm))
                {
                    var index = UsedUpOtherDorms.FindIndex(d => d.Dorm == scheduled.OtherDorm);
                    DormPriorities.Add(scheduled.OtherDorm, UsedUpOtherDorms[index]);
                    UsedUpOtherDorms.RemoveAt(index);
                }
                var interDorm = DormPriorities[scheduled.OtherDorm];
                ++interDorm.Priority;
                interDorm.AvailableToday = true;
                interDorm.ScheduleHistory.Remove(scheduled.Abbreviation);
                interDorm.PreviousRepeatableActivities.Remove(activity.ID);
            }
            return output;
        }

        public void NewDay()
        {
            AvailableActivitiesToday.Clear();
            AvailableActivitiesToday.UnionWith(AvailableActivities);
            AvailableMultiDormActivitiesToday.Clear();
            AvailableMultiDormActivitiesToday.UnionWith(AvailableMultiDormActivities);
            OtherDormsDoneToday.Clear();
            RepeatableTodayHistory.Clear();
            foreach (var otherDorm in DormPriorities.Keys)
            {
                if (otherDorm != Dorm)
                    DormPriorities[otherDorm].AvailableToday = true;
            }
        }

        public bool CheckCompatibility(DormActivities otherActivities, out SortedSet<int> overlapping)
        {
            if (!otherActivities.AvailableMultiDormActivitiesToday.Overlaps(AvailableMultiDormActivitiesToday))
            {
                var interDorm = DormPriorities[otherActivities.Dorm];
                interDorm.Options = 0;
                if (!otherActivities.AvailableMultiDormActivities.Overlaps(AvailableMultiDormActivities))
                {
                    interDorm.AvailableToday = false;
                    UsedUpOtherDorms.Add(interDorm);
                    DormPriorities.Remove(otherActivities.Dorm);
                    overlapping = null;
                }
                else
                {
                    overlapping = new SortedSet<int>(AvailableMultiDormActivities);
                    overlapping.IntersectWith(otherActivities.AvailableMultiDormActivities);
                }
                return false;
            }
            else
            {
                overlapping = new SortedSet<int>(AvailableMultiDormActivitiesToday);
                overlapping.IntersectWith(otherActivities.AvailableMultiDormActivitiesToday);
                return true;
            }
        }

        internal bool RecheckCompatibility(DormActivities otherActivities, out bool overlaps)
        {
            overlaps = true;
            if (!otherActivities.AvailableMultiDormActivitiesToday.Overlaps(AvailableMultiDormActivitiesToday))
            {
                var interDorm = UsedUpOtherDorms.First(d => d.Dorm == otherActivities.Dorm);
                interDorm.Options = 0;
                if (!otherActivities.AvailableMultiDormActivities.Overlaps(AvailableMultiDormActivities))
                {
                    interDorm.AvailableToday = false;
                    UsedUpOtherDorms.Add(interDorm);
                    DormPriorities.Remove(otherActivities.Dorm);
                    overlaps = false;
                }
                else
                {
                    var overlapping = new SortedSet<int>(AvailableMultiDormActivities);
                    overlapping.IntersectWith(otherActivities.AvailableMultiDormActivities);
                    if (overlapping.Count == 0)
                        overlaps = false;
                }
                return false;
            }
            return true;
        }

        public void ScheduleActivity(ActivityInfo activity, string scheduled, bool clear = true)
        {
            if (!activity.Flags.HasFlag(ActivityFlags.Manual) && (!activity.Flags.HasFlag(ActivityFlags.Exclusive) || !activity.Flags.HasFlag(ActivityFlags.Excess)))
                TotalScheduledDuration += activity.Duration;
            else
                TotalManualDuration += activity.Duration;

            AvailableActivitiesToday.Remove(activity.ID);
            if (activity.Flags.HasFlag(ActivityFlags.MultiDorm))
                AvailableMultiDormActivitiesToday.Remove(activity.ID);

            bool repeatable = activity.Flags.HasFlag(ActivityFlags.Repeatable);
            if (clear)
            {
                AvailableActivities.Remove(activity.ID);
                if (repeatable)
                {
                    RepeatableHistory.Add(activity.ID);
                    RepeatableTodayHistory.Add(activity.ID);
                }
                if (activity.Flags.HasFlag(ActivityFlags.MultiDorm))
                    AvailableMultiDormActivities.Remove(activity.ID);
            }
            else if (repeatable)
            {
                RepeatableTodayHistory.Add(activity.ID);
                if (!RepeatableHistory.Add(activity.ID) && !AvailableActivities.Remove(activity.ID))
                    RepeatableDoubleHistory.Add(activity.ID);
                else if (activity.Flags.HasFlag(ActivityFlags.MultiDorm))
                    AvailableMultiDormActivities.Remove(activity.ID);
            }

            ScheduleHistory.Add(scheduled);
        }

        public object Clone()
        {
            var clone = new DormActivities(Schedule.Dorms[Dorm], ActivityPriorities)
            {
                OtherDorms = this.OtherDorms,
                TotalManualDuration = this.TotalManualDuration,
                TotalScheduledDuration = this.TotalScheduledDuration,
                TotalActivityDuration = this.TotalActivityDuration
            };
            clone.AvailableActivities.UnionWith(this.AvailableActivities);
            clone.AvailableActivitiesToday.UnionWith(this.AvailableActivitiesToday);
            clone.AvailableMultiDormActivities.UnionWith(this.AvailableMultiDormActivities);
            clone.AvailableMultiDormActivitiesToday.UnionWith(this.AvailableMultiDormActivitiesToday);
            clone.RepeatableHistory.UnionWith(this.RepeatableHistory);
            clone.RepeatableTodayHistory.UnionWith(this.RepeatableTodayHistory);
            clone.RepeatableDoubleHistory.UnionWith(this.RepeatableDoubleHistory);
            clone.OtherDormsDoneToday.UnionWith(this.OtherDormsDoneToday);
            clone.ScheduleHistory.UnionWith(this.ScheduleHistory);

            var keys = this.DormPriorities.Keys.ToArray();
            for (int i = 0; i < keys.Length; ++i)
            {
                var tracker = this.DormPriorities[keys[i]];
                clone.DormPriorities.Add(keys[i], (InterDormTracking)tracker.Clone());
            }
            for (int i = 0; i < UsedUpOtherDorms.Count; ++i)
            {
                clone.UsedUpOtherDorms.Add((InterDormTracking)this.UsedUpOtherDorms[i].Clone());
            }
            return clone;
        }
    }
}
