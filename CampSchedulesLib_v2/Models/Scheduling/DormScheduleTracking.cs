using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    public class DormScheduleTracking
    {
        public int RemainingBlocks { get; set; }
        public int OptionsLeft { get; private set; }
        public int PrevOptionsLeft { get; private set; }
        public int OtherDormOptionsRemaining { get; private set; }
        public double Score { get; private set; }
        public SortedSet<int> ReservedBlocks { get; private set; }
        public SortedDictionary<int, SortedSet<int>> Conflicts { get; private set; }
        public int Dorm { get; private set; }
        public bool HasOptions { get; private set; }
        public int TotalAvailableDuration { get; private set; }

        public DormScheduleTracking(int dorm, int remainingBlocks)
        {
            Dorm = dorm;
            ReservedBlocks = null;
            RemainingBlocks = remainingBlocks;
            OptionsLeft = 0;
            PrevOptionsLeft = 0;
            OtherDormOptionsRemaining = 0;
            Conflicts = new SortedDictionary<int, SortedSet<int>>();
            ReservedBlocks = new SortedSet<int>();
        }

        public void CalculateScore() => Score = (double)OptionsLeft / RemainingBlocks;

        private DormScheduleTracking(int dorm, int remainingBlocks, IEnumerable<int> reserved) : this(dorm, remainingBlocks - reserved.Count())
        {
            ReservedBlocks = new SortedSet<int>(reserved);
        }

        public static bool TryCreate(int dorm, int blocksAvailable, IEnumerable<int> reserved, out DormScheduleTracking tracker)
        {
            if (reserved != null && reserved.Any())
            {
                if (reserved.Count() >= blocksAvailable)
                {
                    tracker = null;
                    return false;
                }
                else
                    tracker = new DormScheduleTracking(dorm, blocksAvailable, reserved);
            }
            else
                tracker = new DormScheduleTracking(dorm, blocksAvailable);

            return true;
        }

        public bool OptionsRemaining(IEnumerable<InterDormTracking> dormPriorities, SortedDictionary<int, int> otherDormOptions/*, int optionsCount*/)
        {
            PrevOptionsLeft = OptionsLeft;
            OtherDormOptionsRemaining = 0;
            OptionsLeft = 0;// optionsCount;
            foreach(var otherDorm in dormPriorities)
            {
                OptionsLeft += otherDorm.Options;
                if (otherDorm.Dorm != Dorm && otherDorm.AvailableToday && otherDorm.Options > 0)
                {
                    OtherDormOptionsRemaining += otherDorm.Options;
                    if (otherDormOptions.TryGetValue(otherDorm.Dorm, out int otherDormOptionsLeft))
                        otherDormOptions[otherDorm.Dorm] = otherDormOptionsLeft + otherDorm.Options;
                    else
                        otherDormOptions.Add(otherDorm.Dorm, otherDorm.Options);
                }
            }
            HasOptions = OptionsLeft > 0;
            if (otherDormOptions.TryGetValue(Dorm, out int optionsLeft))
            {
                OptionsLeft += optionsLeft;
                otherDormOptions.Remove(Dorm);
            }
            return PrevOptionsLeft != OptionsLeft;
        }
    }
}
