using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Info;

namespace CampSchedules_v3
{
    [Flags]
    public enum ActivityMultiDorm : byte
    {
        Single = 1,
        Multi = 2,
        SingleOrMulti
    }

    public struct ActivitySurvivalInfo
    {
        internal static SortedSet<byte> LONGER;
        internal static byte NUM_LONGER;

        public bool IsRepeatable { get; set; }
        public bool IsExclusive { get; set; }
        public bool LongerDuration { get; set; }
        //public byte BasePriority { get; set; }
        public ActivityMultiDorm MultiDorm { get; set; }

        public ActivitySurvivalInfo(ActivityInfo info)
        {
            IsExclusive = info.Flags.HasFlag(ActivityFlags.Exclusive);
            IsRepeatable = info.Flags.HasFlag(ActivityFlags.Repeatable);
            LongerDuration = info.Duration > 1;
            if (info.Flags.HasFlag(ActivityFlags.MultiDorm))
            {
                if (info.Flags.HasFlag(ActivityFlags.SingleDorm))
                    MultiDorm = ActivityMultiDorm.SingleOrMulti;
                else
                    MultiDorm = ActivityMultiDorm.Multi;
            }
            else
                MultiDorm = ActivityMultiDorm.Single;
        }

        internal static bool IsLonger(byte activity) =>
            LONGER.Contains(activity);
    }
}
