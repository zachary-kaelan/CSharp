﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Info;

namespace CampSchedulesLib_v2.Models.CSV
{
    public class ActivityCSV
    {
        public int Duration { get; set; }
        public int ExhaustionLevel { get; set; }
        public bool IsExclusive { get; set; }
        public bool ManuallyScheduled { get; set; }
        public int MaxDorms { get; set; }
        public int MinDorms { get; set; }
        public int MaxConcurrent { get; set; }
        public string Name { get; set; }
        //public int Count { get; set; }
        public int Priority { get; set; }
        public bool Repeatable { get; set; }
        //public int Zone { get; set; }
        public string Abbreviation { get; set; }

        public ActivityInfo ToInfo()
        {
            ActivityFlags flags = ActivityFlags.None;
            if (ManuallyScheduled)
                flags |= ActivityFlags.Manual;
            if (Repeatable)
                flags |= ActivityFlags.Repeatable;
            if (IsExclusive)
                flags |= ActivityFlags.Exclusive;

            if (Duration >= 2)
            {
                if (Abbreviation == "HR" || Abbreviation == "RC")
                    flags |= ActivityFlags.Excess;
            }
            if (MaxConcurrent > 1 || MaxConcurrent == -1)
                flags |= ActivityFlags.Concurrent;
            if (MinDorms == 1)
                flags |= ActivityFlags.SingleDorm;
            if (MaxDorms > 1)
                flags |= ActivityFlags.MultiDorm;
            if (ExhaustionLevel >= 4)
                flags |= ActivityFlags.Exhausting;
            else if (ExhaustionLevel == 0)
                flags |= ActivityFlags.Relaxing;

            return new ActivityInfo(
                Name,
                Abbreviation,
                Priority,
                ExhaustionLevel,
                flags,
                MaxDorms,
                MinDorms,
                Duration,
                MaxConcurrent
            );
        }
    }
}
