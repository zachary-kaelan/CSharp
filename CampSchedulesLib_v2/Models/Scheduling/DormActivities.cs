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
        public SortedSet<int> ActivitiesDoneToday { get; private set; }
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

        public DormActivities(DormActivities upstream)
        {
            Dorm = upstream.Dorm;

            ScheduleHistory = new SortedSet<string>();
            AvailableActivities = new SortedSet<int>();
            ActivityPriorities = new SortedDictionary<int, int>(upstream.ActivityPriorities);

            OtherDorms = new SortedSet<int>();
            DormPriorities = new SortedDictionary<int, InterDormTracking>();
            RepeatableHistory = new SortedSet<int>();
            RepeatableTodayHistory = new SortedSet<int>();
            RepeatableDoubleHistory = new SortedSet<int>();

            AvailableActivitiesToday = new SortedSet<int>();
            AvailableMultiDormActivities = new SortedSet<int>();
            AvailableMultiDormActivitiesToday = new SortedSet<int>();
            ActivitiesDoneToday = new SortedSet<int>();

            OtherDormsDoneToday = new SortedSet<int>();
            UsedUpOtherDorms = new List<InterDormTracking>();
        }

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
            ActivitiesDoneToday = new SortedSet<int>();

            OtherDormsDoneToday = new SortedSet<int>();
            UsedUpOtherDorms = new List<InterDormTracking>();
            //RemainderPriority = 0;
        }

        internal void ClearFromPrevDay(ScheduledActivity scheduledActivity, bool ignoreHasOther = false)
        {
            ScheduleHistory.Remove(scheduledActivity.Abbreviation);
            var activity = Schedule.Activities[scheduledActivity.Activity];

            bool repeatable = activity.Flags.HasFlag(ActivityFlags.Repeatable);
            bool multiDorm = activity.Flags.HasFlag(ActivityFlags.MultiDorm);

            if (scheduledActivity.HasOther)
            {
                if (!ignoreHasOther)
                {
                    if (!DormPriorities.TryGetValue(scheduledActivity.OtherDorm, out InterDormTracking interDorm))
                    {
                        var index = UsedUpOtherDorms.FindIndex(d => d.Dorm == scheduledActivity.OtherDorm);
                        interDorm = (InterDormTracking)UsedUpOtherDorms[index].Clone();
                        UsedUpOtherDorms.RemoveAt(index);
                        DormPriorities.Add(scheduledActivity.OtherDorm, interDorm);
                    }

                    interDorm.ClearFromHistory(scheduledActivity, repeatable, multiDorm);
                }
            }
            else
                DormPriorities[Dorm].ClearFromHistory(scheduledActivity, repeatable, multiDorm);

            if (repeatable)
            {
                if (RepeatableHistory.Contains(activity.ID))
                {
                    // if it wasn't cleared, it was the first time the activity was done by that dorm
                    if (!AvailableActivities.Add(activity.ID))
                        RepeatableHistory.Remove(activity.ID);
                    if (multiDorm)
                        AvailableMultiDormActivities.Add(activity.ID);
                }
                else
                    throw new NotImplementedException("Activity being cleared hasn't been undergone.");

                if (ActivityPriorities.TryGetValue(activity.ID, out int activityPriority))
                {
                    if (++activityPriority == 0)
                        ActivityPriorities.Remove(activity.ID);
                }
                else
                    ActivityPriorities.Add(activity.ID, 1);
            }
            else
            {
                if (!AvailableActivities.Add(activity.ID))
                    throw new NotImplementedException("Activity being cleared hasn't been undergone.");
                if (multiDorm)
                    AvailableMultiDormActivities.Add(activity.ID);
            }

            if (!activity.Flags.HasFlag(ActivityFlags.Manual) && (!activity.Flags.HasFlag(ActivityFlags.Exclusive) || !activity.Flags.HasFlag(ActivityFlags.Excess)))
                TotalScheduledDuration -= activity.Duration;
            else
                TotalManualDuration -= activity.Duration;
        }

        internal void ClearFromHistory(ScheduledActivity scheduledActivity, bool ignoreHasOther = false)
        {
            ScheduleHistory.Remove(scheduledActivity.Abbreviation);
            var activity = Schedule.Activities[scheduledActivity.Activity];

            bool repeatable = activity.Flags.HasFlag(ActivityFlags.Repeatable);
            bool multiDorm = activity.Flags.HasFlag(ActivityFlags.MultiDorm);

            if (activity.IncompatibleActivities.Any())
            {
                var incompat = new SortedSet<int>(activity.IncompatibleActivities);
                incompat.ExceptWith(ActivitiesDoneToday);
                if (incompat.Any())
                {
                    AvailableActivitiesToday.UnionWith(incompat);
                    AvailableMultiDormActivitiesToday.UnionWith(incompat.Intersect(ActivityInfo.MultiDormActivities));
                }
            }
            ActivitiesDoneToday.Remove(activity.ID);

            if (scheduledActivity.HasOther)
            {
                if (!ignoreHasOther)
                {
                    if (!DormPriorities.TryGetValue(scheduledActivity.OtherDorm, out InterDormTracking interDorm))
                    {
                        var index = UsedUpOtherDorms.FindIndex(d => d.Dorm == scheduledActivity.OtherDorm);
                        interDorm = (InterDormTracking)UsedUpOtherDorms[index].Clone();
                        UsedUpOtherDorms.RemoveAt(index);
                        DormPriorities.Add(scheduledActivity.OtherDorm, interDorm);
                    }

                    interDorm.ClearFromHistory(scheduledActivity, repeatable, multiDorm);
                    OtherDormsDoneToday.Remove(scheduledActivity.OtherDorm);
                }
            }
            else
            {
                DormPriorities[Dorm].ClearFromHistory(scheduledActivity, repeatable, multiDorm);
                OtherDormsDoneToday.Remove(Dorm);
            }

            if (repeatable)
            {
                if (RepeatableHistory.Contains(activity.ID))
                {
                    // if it wasn't cleared, it was the first time the activity was done by that dorm
                    if (!AvailableActivities.Add(activity.ID))
                        RepeatableHistory.Remove(activity.ID);
                    if (multiDorm)
                        AvailableMultiDormActivities.Add(activity.ID);
                    RepeatableTodayHistory.Remove(activity.ID);
                }
                else
                    throw new NotImplementedException("Activity being cleared hasn't been undergone.");

                if (ActivityPriorities.TryGetValue(activity.ID, out int activityPriority))
                {
                    if (++activityPriority == 0)
                        ActivityPriorities.Remove(activity.ID);
                }
                else
                    ActivityPriorities.Add(activity.ID, 1);
            }
            else
            {
                if (!AvailableActivities.Add(activity.ID))
                    throw new NotImplementedException("Activity being cleared hasn't been undergone.");
                if (multiDorm)
                    AvailableMultiDormActivities.Add(activity.ID);
            }

            AvailableActivitiesToday.Add(activity.ID);
            if (multiDorm)
                AvailableMultiDormActivitiesToday.Add(activity.ID);

            if (!activity.Flags.HasFlag(ActivityFlags.Manual) && (!activity.Flags.HasFlag(ActivityFlags.Exclusive) || !activity.Flags.HasFlag(ActivityFlags.Excess)))
                TotalScheduledDuration -= activity.Duration;
            else
                TotalManualDuration -= activity.Duration;
        }

        public void NewDay()
        {
            AvailableActivitiesToday.Clear();
            AvailableActivitiesToday.UnionWith(AvailableActivities);
            AvailableMultiDormActivitiesToday.Clear();
            AvailableMultiDormActivitiesToday.UnionWith(AvailableMultiDormActivities);
            OtherDormsDoneToday.Clear();
            RepeatableTodayHistory.Clear();
            ActivitiesDoneToday.Clear();
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

        internal void RescheduleActivity(ActivityInfo activity, ScheduledActivity scheduledActivity, bool ignoreOther)
        {
            bool multiDorm = activity.Flags.HasFlag(ActivityFlags.MultiDorm);

            AvailableActivitiesToday.Remove(activity.ID);
            if (multiDorm)
                AvailableMultiDormActivitiesToday.Remove(activity.ID);

            if (activity.Flags.HasFlag(ActivityFlags.Repeatable))
                RepeatableTodayHistory.Add(activity.ID);

            ActivitiesDoneToday.Add(activity.ID);
            if (activity.IncompatibleActivities.Any())
            {
                AvailableActivitiesToday.ExceptWith(activity.IncompatibleActivities);
                AvailableMultiDormActivitiesToday.ExceptWith(activity.IncompatibleActivities);
            }

            if (!ignoreOther && scheduledActivity.HasOther)
            {
                if (DormPriorities.TryGetValue(scheduledActivity.OtherDorm, out InterDormTracking interDorm))
                    interDorm.RescheduleActivity(!OtherDormsDoneToday.Add(scheduledActivity.OtherDorm));
            }
            else
                OtherDormsDoneToday.Add(Dorm);
        }

        public void ScheduleActivity(ActivityInfo activity, ScheduledActivity scheduledActivity)
        {
            if (!activity.Flags.HasFlag(ActivityFlags.Manual) && (!activity.Flags.HasFlag(ActivityFlags.Exclusive) || !activity.Flags.HasFlag(ActivityFlags.Excess)))
                TotalScheduledDuration += activity.Duration;
            else
                TotalManualDuration += activity.Duration;

            bool repeatable = activity.Flags.HasFlag(ActivityFlags.Repeatable);
            bool multiDorm = activity.Flags.HasFlag(ActivityFlags.MultiDorm);
            AvailableActivitiesToday.Remove(activity.ID);
            if (multiDorm)
                AvailableMultiDormActivitiesToday.Remove(activity.ID);

            bool clear = !repeatable;
            if (repeatable)
            {
                if (!RepeatableHistory.Add(activity.ID))
                    clear = true;
                else
                {
                    if (ActivityPriorities.TryGetValue(activity.ID, out int activityPriority))
                    {
                        if (--activityPriority == 0)
                            ActivityPriorities.Remove(activity.ID);
                    }
                    else
                        ActivityPriorities.Add(activity.ID, -1);
                }
                /*if (!RepeatableTodayHistory.Add(activity.ID))
                    throw new IndexOutOfRangeException("Activity repeated twice the same day.");*/
            }

            if (clear)
            {
                AvailableActivities.Remove(activity.ID);
                if (multiDorm)
                    AvailableMultiDormActivities.Remove(activity.ID);
            }

            ActivitiesDoneToday.Add(activity.ID);
            if (activity.IncompatibleActivities.Any())
            {
                AvailableActivitiesToday.ExceptWith(activity.IncompatibleActivities);
                AvailableMultiDormActivitiesToday.ExceptWith(activity.IncompatibleActivities);
            }

            ScheduleHistory.Add(scheduledActivity.Abbreviation);
            if (scheduledActivity.HasOther)
            {
                if (scheduledActivity.Dorm == Dorm)
                {
                    if (
                        DormPriorities.TryGetValue(
                            scheduledActivity.OtherDorm, 
                            out InterDormTracking otherDorm
                        ) && otherDorm.ScheduleActivity(
                            scheduledActivity, 
                            !OtherDormsDoneToday.Add(scheduledActivity.OtherDorm), 
                            repeatable, multiDorm
                        )
                    ) {
                        UsedUpOtherDorms.Add((InterDormTracking)otherDorm.Clone());
                        DormPriorities.Remove(scheduledActivity.OtherDorm);
                    }
                }
            }
            else
            {
                DormPriorities[Dorm].ScheduleActivity(scheduledActivity, false, repeatable, multiDorm);
                OtherDormsDoneToday.Add(Dorm);
            }
        }

        public object Clone()
        {
            var clone = new DormActivities(this)
            {
                TotalManualDuration = this.TotalManualDuration,
                TotalScheduledDuration = this.TotalScheduledDuration,
                TotalActivityDuration = this.TotalActivityDuration
            };

            clone.ScheduleHistory.UnionWith(this.ScheduleHistory);
            clone.AvailableActivities.UnionWith(this.AvailableActivities);

            clone.OtherDorms.UnionWith(this.OtherDorms);
            clone.RepeatableHistory.UnionWith(this.RepeatableHistory);
            clone.RepeatableTodayHistory.UnionWith(this.RepeatableTodayHistory);
            clone.RepeatableDoubleHistory.UnionWith(this.RepeatableDoubleHistory);

            clone.AvailableActivitiesToday.UnionWith(this.AvailableActivitiesToday);
            clone.AvailableMultiDormActivities.UnionWith(this.AvailableMultiDormActivities);
            clone.AvailableMultiDormActivitiesToday.UnionWith(this.AvailableMultiDormActivitiesToday);
            clone.ActivitiesDoneToday.UnionWith(this.ActivitiesDoneToday);

            clone.OtherDormsDoneToday.UnionWith(this.OtherDormsDoneToday);

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
