using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    class ConflictsResolveTracking
    {
        public bool HasMultiDormOptions { get; private set; }
        public bool DoDormDelegates { get; private set; }
        public SortedSet<int> IncludedDorms { get; private set; }
        public SortedDictionary<int, SortedSet<int>> InterDormActivityConflicts { get; private set; }
        public SortedSet<int> DormsToClear { get; private set; }
        public SortedSet<int> ActivitiesToClear { get; private set; }

        public int Dorm { get; private set; }
        public bool IsPresetDorm { get; private set; }
        public List<Predicate<DormActivityOption>> MainDelegates { get; private set; }
        public List<Predicate<DormActivityOption>> DormDelegates { get; private set; }

        public ConflictsResolveTracking(int thisDorm, bool hasMultiDormOptions)
        {
            Dorm = thisDorm;
            IncludedDorms = new SortedSet<int>();
            InterDormActivityConflicts = new SortedDictionary<int, SortedSet<int>>();
            DormsToClear = new SortedSet<int>();
            ActivitiesToClear = new SortedSet<int>();
            MainDelegates = new List<Predicate<DormActivityOption>>();
            DormDelegates = new List<Predicate<DormActivityOption>>();
            HasMultiDormOptions = hasMultiDormOptions;
            DoDormDelegates = HasMultiDormOptions;
            IsPresetDorm = false;
        }

        public bool GetPotentialConflictingDorms(int thisDorm, SortedSet<int> dormsBeingCleared, IDictionary<int, InterDormTracking> dormPriorities)
        {
            if (HasMultiDormOptions)
            {
                if (dormsBeingCleared != null && dormsBeingCleared.Count > 0)
                {
                    foreach (var otherDorm in dormPriorities.Keys)
                    {
                        if (otherDorm != thisDorm)
                        {
                            var tracker = dormPriorities[otherDorm];
                            if (tracker.IncludeInResolving())
                            {
                                if (dormsBeingCleared.Contains(otherDorm))
                                {
                                    DormsToClear.Add(otherDorm);
                                    tracker.Options = 0;
                                }
                                else
                                    IncludedDorms.Add(otherDorm);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var otherDorm in dormPriorities.Keys)
                    {
                        if (otherDorm != thisDorm && dormPriorities[otherDorm].IncludeInResolving())
                            IncludedDorms.Add(otherDorm);
                    }
                }

                if (IncludedDorms.Count > 0)
                    return true;
                else
                    HasMultiDormOptions = false;
            }
            return false;
        }

        public void AddDormForClearing(int dorm)
        {
            DormsToClear.Add(dorm);
            IncludedDorms.Remove(dorm);
        }

        public void AddInterDormActivityConflict(int dorm, int activity)
        {
            if (InterDormActivityConflicts.ContainsKey(dorm))
                InterDormActivityConflicts[dorm].Add(activity);
            else
                InterDormActivityConflicts.Add(dorm, new SortedSet<int>() { activity });
        }

        public void HandlePreset(PresetDormConflicts thisDormDelegates)
        {
            IsPresetDorm = true;
            ActivitiesToClear.UnionWith(thisDormDelegates.UnavailableActivities);

            if (HasMultiDormOptions && thisDormDelegates.OtherDorm != -1)
                AddDormForClearing(thisDormDelegates.OtherDorm);
        }

        public bool SelectDelegates()
        {
            DormsToClear.Remove(Dorm);
            if (ActivitiesToClear.Count > 0)
                MainDelegates.Insert(0, o => ActivitiesToClear.Contains(o.Activity));
            
            if (DoDormDelegates)
            {
                if (DormsToClear.Count > 0)
                {
                    //IncludedDorms.ExceptWith(DormsToClear);
                    DormDelegates.Add(o => DormsToClear.Contains(o.OtherDorm));
                }
                if (InterDormActivityConflicts.Count > 0)
                    DormDelegates.Add(
                        o => InterDormActivityConflicts.TryGetValue(
                            o.OtherDorm,
                            out SortedSet<int> conflicts
                        ) && conflicts.Contains(o.Activity)
                    );
                if (DormDelegates.Count > 0)
                {
                    if (MainDelegates.Count > 0)
                        MainDelegates.Insert(
                            MainDelegates.Count - 1,
                            o => o.HasOther && DormDelegates.Any(d => d(o))
                        );
                    else
                        MainDelegates.Add(o => o.HasOther && DormDelegates.Any(d => d(o)));
                }                    
            }

            return MainDelegates.Count > 0;
        }
    }
}
