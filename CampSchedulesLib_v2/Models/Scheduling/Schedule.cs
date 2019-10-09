using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models;
using CampSchedulesLib_v2.Models.CSV;
using CampSchedulesLib_v2.Models.Info;
using ZachLib;
using ZachLib.Logging;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    public class Schedule : IReadOnlyList<Day>
    {
        internal static readonly Random GEN = new Random(523947200);
        private const double MUTATION_RATE =                       1.5;
        private const double CHANCE_CRITICAL =          0.1      * MUTATION_RATE; 
        private const double CHANCE_FIRST_ELEMENT = 1 - ((1 - 0.85) * MUTATION_RATE);
        private const double CHANCE_INCREASE_SCORE =    0.25     * MUTATION_RATE;
        private const double CHANCE_THIRD_REPEATS =     0        * MUTATION_RATE;
        private const double CHANCE_DOUBLE_BACKTRACK =  0.25     * MUTATION_RATE;
        private const int NUM_TRIES_BLOCKS = 3;
        private const int NUM_TRIES_DAYS = 2;
        internal static bool RANDOMNESS_ENABLED = false;

        static Schedule()
        {
            LogManager.AddLog("CampSchedules", LogType.FolderFilesByDate);
            LogManager.Start(System.Threading.ThreadPriority.AboveNormal, false, true);
        }

        public static readonly List<ActivityInfo> Activities = new List<ActivityInfo>();
        public static readonly List<DormInfo> Dorms = new List<DormInfo>();

        public DormActivities[] DormActivities { get; private set; }
        public SortedSet<string>[] ActivitiesScheduleHistories { get; private set; }
        public SortedDictionary<string, int> ScheduledActivityAbbrvs { get; private set; }
        public List<ScheduledActivity> ScheduledActivities { get; private set; }
        public ScheduledActivity this[string abbrv] => ScheduledActivities[ScheduledActivityAbbrvs[abbrv]];
        public List<Block> Blocks { get; private set; }
        private SortedSet<int> ManuallyScheduled = new SortedSet<int>();
        public SortedDictionary<int, SortedSet<int>> ManuallyScheduledBlocks { get; private set; }
        private int NumDorms { get; set; }
        private int NumGirls { get; set; }
        private int[] DormAgeIndices { get; set; }
        public static float TotalBlocksDuration { get; private set; }
        private SortedDictionary<int, int> RecheckDormCompatibility { get; set; }
//        private Stack<DormActivities[]> DormActivitiesHistory { get; set; }
        private DormActivities[] InitialDormActivities { get; set; }
        private SortedSet<string> LastSuccessfulHistory { get; set; }

        public string PATH_FOLDER { get; set; }

        private delegate bool FilterOption(DormActivityOption opt);

        #region Schedule_Setup
        public Schedule(string path)
        {
            PATH_FOLDER = path.TrimEnd('\\') + '\\';

            string[] activityIncompatibilities = File.ReadAllLines(PATH_FOLDER + "Activity Incompatibilities.txt");
            foreach (var incompatibility in activityIncompatibilities)
            {
                var activities = incompatibility.Split(
                    new char[] { ',', ' ' }, 
                    StringSplitOptions.RemoveEmptyEntries
                ).Select(
                    abbrv => Activities.FindIndex(a => a.Abbreviation == abbrv)
                ).ToArray();

                for (int a = 0; a < activities.Length - 1; ++a)
                {
                    var activity = Activities[activities[a]];
                    for (int a2 = a + 1; a2 < activities.Length; ++a2)
                    {
                        var id = activities[a2];
                        activity.IncompatibleActivities.Add(id);
                        Activities[id].IncompatibleActivities.Add(activity.ID);
                    }
                }
            }

            //DormActivitiesHistory = new Stack<DormActivities[]>();
            RecheckDormCompatibility = new SortedDictionary<int, int>();

            TotalBlocksDuration = 0;
            NumDorms = Dorms.Count;
            var altAgeIndex = Dorms.OrderBy(d => d.AgeGroup).ThenBy(d => !d.IsGirl).Select(d => d.ID).ToArray();
            var girlsMaxAgeGroup = Dorms.Last(d => d.IsGirl).AgeGroup;
            var boysMaxAgeGroup = Dorms.Last(d => !d.IsGirl).AgeGroup;
            DormAgeIndices = new int[Dorms.Count];
            for (int i = 0; i < NumDorms; ++i)
            {
                DormAgeIndices[Dorms[i].GetAgeIndex(boysMaxAgeGroup, girlsMaxAgeGroup)] = i;
                Dorms[altAgeIndex[i]].AgeIndex = i;
            }

            Days = new Day[]
            {
                null,
                new Day(DayOfWeek.Monday),
                new Day(DayOfWeek.Tuesday),
                new Day(DayOfWeek.Wednesday),
                new Day(DayOfWeek.Thursday),
                new Day(DayOfWeek.Friday),
                null
            };

            ScheduledActivityAbbrvs = new SortedDictionary<string, int>();
            ScheduledActivities = new List<ScheduledActivity>();
            Blocks = new List<Block>();
            ActivitiesScheduleHistories = new SortedSet<string>[Activities.Count];
            for (int i = 0; i < Activities.Count; ++i)
            {
                ActivitiesScheduleHistories[i] = new SortedSet<string>();
            }

            AddBlocks(Utils.LoadCSV<BlockCSV>(PATH_FOLDER + "Days.csv"));

            var dormActivityPriorities = Utils.LoadCSV<DormActivityPriorityCSV>(PATH_FOLDER + "Additional Dorm Priorities.csv").GroupBy(
                p => p.Dorm
            ).ToDictionary(
                g => g.Key,
                g => g.ToDictionary(
                    p => Activities.FindIndex(
                        a => a.Abbreviation == p.ActivityAbbreviation
                    ), p => p.PriorityChange
                )
            );
            DormActivities = Dorms.Select(
                d => new DormActivities(d, dormActivityPriorities.TryGetValue(d.Abbreviation, out Dictionary<int, int> priorities) ? priorities : new Dictionary<int, int>())
            ).ToArray();

            var dorms = DormAgeIndices.Select(d => Dorms[d]).ToList();
            var olderDorms = dorms.SkipWhile(d => d.AgeGroup < 5);
            NumGirls = Dorms.Count(d => d.IsGirl);

            bool equalSizedDorms = true;
            double genderRatio = -1;
            double genderDiffRatio = -1;
            if (NumGirls != NumDorms - NumGirls)
            {
                genderRatio = ((double)NumGirls) / (NumDorms - NumGirls);
                genderDiffRatio = ((genderRatio >= 1 ? genderRatio : (1.0 / genderRatio)) - 1);
                equalSizedDorms = false;
            }

            for (int i = 0; i < NumDorms; ++i)
            {
                var dorm = dorms[i];
                var dormPriorties = DormActivities[dorm.ID].DormPriorities;
                dormPriorties.Add(dorm.ID, new InterDormTracking(dorm.ID, dorm.ID, 1));
                IEnumerable<DormInfo> otherDorms = null;

                if (equalSizedDorms)
                    otherDorms = dorms.Skip(i + 1).TakeWhile(d => (d.AgeGroup - dorm.AgeGroup) <= 1);
                else
                {
                    var girlEquivalent = (int)(dorm.AgeGroup * genderRatio);
                    if (!dorm.IsGirl && girlEquivalent > dorm.AgeGroup)
                    {
                        int diffCutOff = (girlEquivalent - dorm.AgeGroup) + 1;
                        otherDorms = dorms.Skip(i + 1).Where(
                            d => (d.AgeGroup - dorm.AgeGroup) <= 1 ||
                                 (d.IsGirl && (d.AgeGroup - dorm.AgeGroup) <= diffCutOff)
                        );
                    }
                    else
                        otherDorms = dorms.Skip(i + 1).TakeWhile(d => (d.AgeGroup - dorm.AgeGroup) <= 1);
                }

                foreach (var other in otherDorms)
                {
                    dormPriorties.Add(
                        other.ID,
                        new InterDormTracking(
                            dorm.ID,
                            other.ID,
                            dorm.IsGirl == other.IsGirl ?
                                3 - (other.AgeGroup - dorm.AgeGroup) :
                                4 - (2 * (other.AgeGroup - dorm.AgeGroup))
                        )
                    );
                }

                // youngest dorms have a chance to connect with older dorms
                if (dorm.AgeGroup == 1)
                {
                    foreach (var olderDorm in olderDorms)
                    {
                        dormPriorties.Add(olderDorm.ID, new InterDormTracking(dorm.ID, olderDorm.ID, 1));
                    }
                }
            }

            ManuallyScheduledBlocks = new SortedDictionary<int, SortedSet<int>>();
            var manuallyScheduled = Utils.LoadCSV<ManuallyScheduledCSV>(PATH_FOLDER + "Manually Scheduled.csv");
            foreach (var manual in manuallyScheduled)
            {
                var dorm = Dorms.FindIndex(d => d.Abbreviation == manual.Dorm);
                SortedSet<int> blocks = null;
                if (!ManuallyScheduledBlocks.ContainsKey(dorm))
                    ManuallyScheduledBlocks.Add(dorm, new SortedSet<int>());
                blocks = ManuallyScheduledBlocks[dorm];

                if (manual.Activity == "CT")
                    DormActivities[dorm].AvailableActivities.Remove(Activities.Last(a => a.Abbreviation == "CA").ID);
                var dayBlocks = Days[(int)manual.Day].Blocks;
                ScheduledActivity activity = null;
                foreach (var blockID in dayBlocks)
                {
                    var block = Blocks[blockID];
                    if (block.Start >= manual.Start && block.Start <= manual.End)
                    {
                        if (activity == null)
                        {
                            activity = new ScheduledActivity(
                                Dorms.FindIndex(d => d.Abbreviation == manual.Dorm),
                                (int)Math.Round((manual.End - manual.Start).TotalHours),
                                Activities.FindIndex(a => a.Abbreviation == manual.Activity),
                                blockID
                            );
                            ManuallyScheduled.Add(activity.ID);
                            var dormActivities = DormActivities[activity.Dorm];
                            dormActivities.TotalScheduledDuration += activity.Duration;
                            ScheduledActivities.Add(activity);
                            ScheduledActivityAbbrvs.Add(activity.Abbreviation, activity.ID);
                            dormActivities.ScheduleHistory.Add(activity.Abbreviation);
                            dormActivities.DormPriorities[activity.Dorm].ScheduleHistory.Add(activity.Abbreviation);
                            ActivitiesScheduleHistories[activity.Activity].Add(activity.Abbreviation);
                        }
                        block.ScheduleHistory.Add(activity.Abbreviation);
                        blocks.Add(blockID);
                    }
                }
                /*if (manual.Dorm.ToLower().Trim().StartsWith("all"))
                {

                }*/
            }

            var dormActivitiesCopy = new DormActivities[DormActivities.Length];
            for (int i = 0; i < DormActivities.Length; ++i)
            {
                dormActivitiesCopy[i] = (DormActivities)DormActivities[i].Clone();
            }
            InitialDormActivities = dormActivitiesCopy;
            //DormActivitiesHistory.Push(dormActivitiesCopy);
            //Console.WriteLine("\t\tPUSH INITIAL: " + DormActivitiesHistory.Count);*/
        }

        public void AddBlock(DayOfWeek day, Block block)
        {
            var dayInfo = Days[(int)day];
            Blocks.Add(block);
            if (block.IsExcess && !dayInfo.Blocks.Any())
                dayInfo.ExcessPreceeding = true;
            dayInfo.Blocks.Add(block.ID);
        }

        public void AddBlocks(BlockCSV[] blocks)
        {
            DayOfWeek day = DayOfWeek.Sunday;
            for (int b = 0; b < blocks.Length; ++b)
            {
                var block = blocks[b];
                if (block.Day != day)
                    day = block.Day;
                if (!block.Excess)
                    ++TotalBlocksDuration;
                AddBlock(day, new Block(new TimeSpan(block.Hour, block.Minute, 0), block.Excess));
            }
        }
        #endregion

        public void Create(string path)
        {
            bool backtracked = false;
            int totalBacktracked = 0;
            Stopwatch timer = Stopwatch.StartNew();

            // the index of the backtracking point, within the Backtracking list
            int backtrackIndex = 0;
            int pointerCopy = 0;

            for (int i = 1; i <= 5; ++i)
            {
                var day = (DayOfWeek)i;

                var dayInfo = Days[i];
                ScheduledActivity[][] scheduled = null;

                if (backtracked)
                {
                    SortedSet<string> prevScheduled = new SortedSet<string>();
                    PrevDay(day);
                    var dormActivitiesCopy = DormActivities.Select(d => (DormActivities)d.Clone()).ToArray();

                    LogManager.Enqueue(
                        "CampSchedules",
                        EntryType.DEBUG,
                        "Backtracked to " + day + ", " + backtrackIndex + ", " + dayInfo.Backtracking.Last(),
                        "Total Scheduled by Day: " + Days.Skip(1).Take(5).Select(
                            d => d.Blocks.Sum(b => Blocks[b].ScheduleHistory.Count)
                        ).ToArrayString()
                    );

                    if (backtrackIndex == 0)
                        Console.WriteLine(">{0}", day);
                        // need to make sure the day ahead was cleared
                    else
                    {
                        Console.WriteLine(">{0}<", day);
                        backtrackIndex = 0;
                    }

                    byte numBacktracks = 0;

                    // start at the end of the day, and make way towards the end
                    for (int backtrackPointer = dayInfo.Backtracking.Count - 1; backtrackPointer >= 0; --backtrackPointer)
                    {
                        ++numBacktracks;

                        //FinishHistoryClear();
                        //var dormActivitiesCopy = DormActivitiesHistory.Count == 1 ? DormActivitiesHistory.Peek().Select(d => (DormActivities)d.Clone()).ToArray() : DormActivitiesHistory.Pop();
                        //Console.Write("\tPOP BACKTRACK: " + DormActivitiesHistory.Count);
                        /*for (int d = 0; d < DormActivities.Length; ++d)
                        {
                            DormActivities[d] = (DormActivities)dormActivitiesCopy[d].Clone();
                        }*/
                        pointerCopy = backtrackPointer;
                        scheduled = ScheduleDay(day, path + @"Days\" + day.ToString() + ".txt", ref pointerCopy, true);

                        if (
                            scheduled != null &&
                            !prevScheduled.SetEquals(
                                new SortedSet<string>(scheduled.SelectMany(b => b.Select(s => s.Abbreviation)))
                            )
                        )
                        {
                            //DormActivitiesHistory.Push(dormActivitiesCopy);
                            //Console.WriteLine("\t~~PUSH BACKTRACK~~: " + DormActivitiesHistory.Count);
                            LogManager.Enqueue(
                                "CampSchedules",
                                EntryType.DEBUG,
                                "BACKTRACK SUCCEEDED",
                                "backTracked " + numBacktracks.ToString() + " block sets"
                            );
                            backtrackIndex = pointerCopy;
                            break;
                        }

                        ClearBlocksFromHistory(day, dayInfo.Backtracking[backtrackPointer - 1], dayInfo.Backtracking[backtrackPointer]);
                    }

                    totalBacktracked += numBacktracks;
                    backtracked = false;
                }
                else
                {
                    backtrackIndex = 0;
                    RANDOMNESS_ENABLED = false;
                    Console.WriteLine(day.ToString());// + " (" + DormActivitiesHistory.Count + ")");
                    
                    scheduled = ScheduleDay(day, path + @"Days\" + day.ToString() + ".txt", ref backtrackIndex);
                }

                if (scheduled == null)
                {
                    ++totalBacktracked;
                    LogManager.Enqueue(
                        "CampSchedules",
                        EntryType.ERROR,
                        "BACKTRACKING",
                        day.ToString()
                    );
                    
                    i -= 2;
                    // next iteration, will be day before current one

                    /*if (backtrackIndex == 0)
                    {
                        dayInfo.Pointer = 0;
                        if (pointerCopy == 1)
                        {
                            foreach (var abbrv in LastSuccessfulHistory)
                            {
                                ClearFromHistory(abbrv);
                            }
                        }
                        else if (pointerCopy == 2)
                            throw new NotImplementedException("Can't handle days with more than two sections of blocks.");
                    }*/
                    if (i == 0)
                        ++i;
                    else if (i > 1 && GEN.NextDouble() < CHANCE_DOUBLE_BACKTRACK)
                    {
                        LogManager.Enqueue(
                            "CampSchedules",
                            EntryType.DEBUG,
                            "Extra Backtrack, from " + day + " to " + (DayOfWeek)(i - 1)
                        );

                        // currently i is the target day, for the next iteration
                        ClearBlocksFromHistory((DayOfWeek)(i + 1), 0, Days[i + 1].Blocks.Count);
                        --i;
                        
                    }
                    backtracked = true;
                }
                else
                {
                    scheduled.SaveDividedAs(path + @"Days\" + day.ToString() + ".json");
                    pointerCopy = 0;
                }

                if (timer.Elapsed.TotalSeconds >= 15)
                {
                    Console.WriteLine("Day {0}, {1} backtracks", day, totalBacktracked);
                    timer.Restart();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Total backtracks: " + totalBacktracked);

            var dorms = Dorms.OrderBy(d => d.AgeGroup).ThenBy(d => !d.IsGirl).ToArray();

            var orderedScheduled = ScheduledActivityAbbrvs.Values.Select(s => ScheduledActivities[s]).OrderBy(s => s.BlockID);

            var scheduledEnumerator = orderedScheduled.GetEnumerator();
            scheduledEnumerator.MoveNext();
            foreach(var day in Days)
            {
                if (day == null)
                    continue;
                var multiBlock = new List<ScheduledActivity.StringableScheduled>();
                var writer = new System.IO.StreamWriter(path + @"Days\" + day.DayOfWeek.ToString() + ".txt");
                foreach (var blockID in day.Blocks)
                {
                    var block = Blocks[blockID];
                    if (block.ScheduleHistory.Any())
                    {
                        string[] lines = new string[Dorms.Count + 1];
                        writer.WriteLine(
                                String.Format(
                                    " -- {0}:{1} {2} -- ",
                                    block.Start.Hours > 12 ? block.Start.Hours - 12 : block.Start.Hours,
                                    block.Start.Minutes.ToString().PadLeft(2, '0'),
                                    block.Start.Hours >= 12 ? "PM" : "AM"
                                )
                            );

                        lines[Dorms.Count] = "";

                        List<ScheduledActivity.StringableScheduled> newMultiBlock = new List<ScheduledActivity.StringableScheduled>();
                        foreach (var str in multiBlock)
                        {
                            lines[str.DormAgeIndex] = str.DormEntry;
                            if (str.HasOther)
                                lines[str.OtherDormAgeIndex] = str.OtherDormEntry;
                            if (str.Duration > 1)
                            {
                                --str.Duration;
                                newMultiBlock.Add(str);
                            }
                        }
                        multiBlock.Clear();
                        multiBlock.AddRange(newMultiBlock);
                        newMultiBlock = null;

                        while (scheduledEnumerator.Current.BlockID == blockID)
                        {
                            var str = new ScheduledActivity.StringableScheduled(scheduledEnumerator.Current);
                            lines[str.DormAgeIndex] = str.DormEntry;
                            if (str.HasOther)
                                lines[str.OtherDormAgeIndex] = str.OtherDormEntry;
                            if (str.Duration > 1)
                            {
                                --str.Duration;
                                multiBlock.Add(str);
                            }
                            if (!scheduledEnumerator.MoveNext())
                                break;
                        }

                        if (block.IsExcess)
                        {
                            writer.WriteLine("    OPEN ACTIVITY TIME");
                            foreach(var line in lines.Where(l => !String.IsNullOrWhiteSpace(l)))
                            {
                                writer.WriteLine(line);
                            }
                            writer.WriteLine();
                        }
                        else
                        {
                            foreach (var line in lines)
                            {
                                writer.WriteLine(line);
                            }
                        }
                    }
                    else if (multiBlock.Any())
                    {
                        string[] lines = new string[Dorms.Count + 1];

                        List<ScheduledActivity.StringableScheduled> newMultiBlock = new List<ScheduledActivity.StringableScheduled>();
                        foreach (var str in multiBlock)
                        {
                            lines[str.DormAgeIndex] = str.DormEntry;
                            if (str.HasOther)
                                lines[str.OtherDormAgeIndex] = str.OtherDormEntry;
                            if (str.Duration > 1)
                            {
                                --str.Duration;
                                newMultiBlock.Add(str);
                            }
                        }
                        multiBlock.Clear();
                        multiBlock.AddRange(newMultiBlock);
                        newMultiBlock = null;

                        if (block.IsExcess)
                        {
                            writer.WriteLine("    OPEN ACTIVITY TIME");
                            foreach (var line in lines.Where(l => !String.IsNullOrWhiteSpace(l)))
                            {
                                writer.WriteLine(line);
                            }
                            writer.WriteLine();
                        }
                        else
                        {
                            foreach (var line in lines.Where(l => !String.IsNullOrWhiteSpace(l)))
                            {
                                writer.WriteLine(line);
                            }
                            writer.WriteLine();
                        }
                    }
                }
                writer.Close();
                writer = null;
            }
            scheduledEnumerator.Dispose();

            orderedScheduled.SaveAs(path + @"Days\FullSchedule.json");
        }

        public void Reports(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!path.EndsWith(@"\"))
                path += @"\";

            int totalBlocks = Days.Sum(d => d == null ? 0 : d.Blocks.Count(b => !Blocks[b].IsExcess));
            float blocksPerDay = 4;
            var orderedScheduled = ScheduledActivityAbbrvs.Values.Select(s => ScheduledActivities[s]).OrderBy(s => s.BlockID);

            float[] dormsNumDays = new float[Dorms.Count];
            using (var dormsWriter = new StreamWriter(path + "Dorms.txt"))
            {
                var priorities = new List<KeyValuePair<string, float>>();
                //var final = DormActivitiesHistory.Pop();
                DormActivities[] initial = InitialDormActivities;
                /*int count = DormActivitiesHistory.Count;
                for (int i = 0; i < count; ++i)
                {
                    initial = DormActivitiesHistory.Pop();
                }*/

                /*for(int d = 0; d < NumDorms; ++d)
                {
                    final[d].ScheduleHistory.Clear();
                }
                foreach(var activity in orderedScheduled)
                {
                    DormActivities[activity.Dorm].ScheduleHistory.Add(activity.Abbreviation);
                    if (activity.HasOther)
                        DormActivities[activity.OtherDorm].ScheduleHistory.Add(activity.Abbreviation);
                }*/

                for (int i = 0; i < Dorms.Count; ++i)
                {
                    var dormInfo = Dorms[i];
                    var initialDormActivities = initial[i];
                    var finalDormActivities = DormActivities[i];

                    //finalDormActivities.AvailableActivities.IntersectWith(final[i].AvailableActivities);
                    //finalDormActivities.RepeatableHistory.UnionWith(final[i].RepeatableHistory);

                    List<string> activities = new List<string>();
                    List<string> dorms = new List<string>();
                    List<string>[] days = new List<string>[Days.Length];
                    int[] daysExhaustion = new int[Days.Length];
                    float[] totalDormPriorities = new float[Dorms.Count];
                    for (int d = 0; d < days.Length; ++d)
                    {
                        days[d] = new List<string>();
                    }
                    float totalPriority = 0;

                    dormsWriter.WriteLine(dormInfo.Abbreviation + " (" + finalDormActivities.ScheduleHistory.Count + ")");

                    foreach (var abbrv in finalDormActivities.ScheduleHistory)
                    {
                        float priority = 0;
                        var activity = this[abbrv];
                        var activityInfo = Activities[activity.Activity];
                        activities.Add(activityInfo.Abbreviation);
                        if (activityInfo.Flags.HasFlag(ActivityFlags.Manual))
                            continue;
                        bool repeatable = activityInfo.Flags.HasFlag(ActivityFlags.Repeatable);
                        var dayIndex = Array.FindIndex(Days, d => d != null && d.Blocks.Contains(activity.BlockID));

                        float dormPriority = initialDormActivities.ActivityPriorities.TryGetValue(activity.Activity, out int activityPriority) ? activityPriority : 0;
                        priority = activityInfo.Priority;
                        var dayMultiplier = (1 + dayIndex) / 2;
                        if (activityInfo.Flags.HasFlag(ActivityFlags.MultiDorm))
                        {
                            var priorityTemp = priority;
                            priority += dormPriority;
                            priority /= 2;
                            priority /= dayMultiplier;
                            if (repeatable)
                                priority /= 2;
                            if (activity.HasOther)
                            {
                                bool amOther = activity.OtherDorm == i;
                                var otherDorm = amOther ? activity.Dorm : activity.OtherDorm;
                                totalDormPriorities[otherDorm] += priority;
                                float otherDormPriority = initial[otherDorm].ActivityPriorities.TryGetValue(activity.Activity, out int activityPriorityTemp) ? activityPriorityTemp : 0;
                                priorityTemp += dormPriority == 0 || otherDormPriority == 0 ? 0 :
                                    (2f * dormPriority * otherDormPriority) / (dormPriority + otherDormPriority);
                                priorityTemp += amOther ? initial[otherDorm].DormPriorities[i].Priority : initialDormActivities.DormPriorities[otherDorm].Priority;
                                priorityTemp = priorityTemp / 3;
                                dorms.Add(Dorms[otherDorm].Abbreviation);
                            }
                            else
                            {
                                dorms.Add("Me");
                                totalDormPriorities[activity.Dorm] += priority;
                                priorityTemp += dormPriority;
                                priorityTemp = priorityTemp / 2;
                            }
                            priorityTemp /= dayMultiplier;
                            if (repeatable)
                                priorityTemp /= 2;
                            priority = priorityTemp;
                        }
                        else if (repeatable)
                            priority /= 2;
                        totalPriority += priority;

                        days[dayIndex].Add(activityInfo.Abbreviation);
                        daysExhaustion[dayIndex] += activityInfo.ExhaustionLevel;
                    }

                    float numDays = ManuallyScheduledBlocks.TryGetValue(i, out SortedSet<int> manual) ?
                        (totalBlocks - manual.Count) / blocksPerDay : 4;
                    dormsNumDays[i] = numDays;
                    totalPriority /= numDays;
                    dormsWriter.WriteLine("\t" + activities.ToArrayString());
                    dormsWriter.WriteLine("\t" + dorms.ToArrayString());
                    dormsWriter.WriteLine("\tDays:");
                    for (int d = 1; d <= 5; ++d)
                    {
                        dormsWriter.WriteLine(
                            "\t\t" + Days[d].DayOfWeek.ToString() + " (" + days[d].Count.ToString() + "): " + days[d].ToArrayString() + "  ~  " + daysExhaustion[d] + " exertion points"
                        );
                    }

                    SortedSet<string> missingActivities = new SortedSet<string>();
                    foreach (var activity in finalDormActivities.AvailableActivities)
                    {
                        var activityInfo = Activities[activity];
                        if (!activities.Contains(activityInfo.Abbreviation))
                        {
                            missingActivities.Add(activityInfo.Abbreviation);
                            /*if (activityInfo.Flags.HasFlag(ActivityFlags.Repeatable))
                            {
                                if (!finalDormActivities.RepeatableHistory.Contains(activity))
                                    missingActivities.Add(activityInfo.Abbreviation);
                            }
                            else
                                missingActivities.Add(activityInfo.Abbreviation);*/
                        }
                    }
                    dormsWriter.WriteLine("\tActivities Missed: " + missingActivities.ToArrayString());

                    List<KeyValuePair<int, float>> otherDormPriorities = new List<KeyValuePair<int, float>>();
                    for (int d = 0; d < Dorms.Count; ++d)
                    {
                        if (totalDormPriorities[d] > 0)
                            otherDormPriorities.Add(new KeyValuePair<int, float>(d, totalDormPriorities[d] / numDays));
                    }
                    if (otherDormPriorities.Any())
                        dormsWriter.WriteLine("\tDorm Partners by Priority: " + otherDormPriorities.OrderByDescending(d => d.Value).Select(d => Dorms[d.Key].Abbreviation).ToArrayString());
                    dormsWriter.WriteLine("\tTotal Priority: " + totalPriority.ToString("#.0"));

                    priorities.Add(new KeyValuePair<string, float>(dormInfo.Abbreviation, totalPriority));
                }
                Console.WriteLine("Dorms by Priority: " + priorities.OrderByDescending(a => a.Value).Select(a => a.Key).ToArrayString());
            }

            var avgNumDays = dormsNumDays.Average();
            var dayIndexIncrement = avgNumDays / Days.Count(d => d != null);

            using (var activitiesWriter = new StreamWriter(path + "Activities.txt"))
            {
                var priorities = new List<KeyValuePair<string, float>>();
                for (int i = 0; i < Activities.Count; ++i)
                {
                    var activityInfo = Activities[i];
                    if (activityInfo.Flags.HasFlag(ActivityFlags.Manual))
                        continue;

                    var history = ActivitiesScheduleHistories[i].Select(a => this[a]);
                    activitiesWriter.WriteLine(activityInfo.Abbreviation + " (" + history.Count().ToString() + ")");
                    List<string> dorms = new List<string>();
                    List<string>[] days = new List<string>[Days.Length];
                    for (int d = 0; d < days.Length; ++d)
                    {
                        days[d] = new List<string>();
                    }
                    List<string> dormPairs = new List<string>();

                    
                    float totalPriority = 0;

                    if (activityInfo.Flags.HasFlag(ActivityFlags.MultiDorm))
                    {
                        foreach (var activity in history)
                        {
                            var dormInfo = Dorms[activity.Dorm];
                            dorms.Add(dormInfo.Abbreviation);
                            var dayIndex = Array.FindIndex(Days, d => d != null && d.Blocks.Contains(activity.BlockID));
                            var day = days[dayIndex];
                            day.Add(dormInfo.Abbreviation);
                            var priority = dormsNumDays[activity.Dorm] - (dayIndex * dayIndexIncrement);
                            if (activity.HasOther)
                            {
                                var otherDormInfo = Dorms[activity.OtherDorm];
                                dorms.Add(otherDormInfo.Abbreviation);
                                dormPairs.Add(dormInfo.Abbreviation + "/" + otherDormInfo.Abbreviation);
                                day.Add(otherDormInfo.Abbreviation);
                                priority += dormsNumDays[activity.OtherDorm] - (dayIndex * dayIndexIncrement);
                            }
                            else
                            {
                                dormPairs.Add(dormInfo.Abbreviation);
                                //priority += dormsNumDays[activity.OtherDorm] - dayIndex;
                            }
                            //priority /= 2;
                            totalPriority += priority;
                        }
                    }
                    else
                    {
                        foreach (var activity in history)
                        {
                            var dormInfo = Dorms[activity.Dorm];
                            dorms.Add(dormInfo.Abbreviation);
                            var dayIndex = Array.FindIndex(Days, d => d != null && d.Blocks.Contains(activity.BlockID));
                            days[dayIndex].Add(dormInfo.Abbreviation);
                            totalPriority += dormsNumDays[activity.Dorm] - dayIndex;
                        }
                    }
                    totalPriority /= avgNumDays;
                    if (activityInfo.Flags.HasFlag(ActivityFlags.Repeatable))
                        totalPriority /= 1.5f;
                    if (activityInfo.Flags.HasFlag(ActivityFlags.Concurrent))
                    {
                        if (activityInfo.MaxConcurrent > 1)
                            totalPriority /= activityInfo.MaxConcurrent;
                        else
                            totalPriority /= 2;
                    }

                    if (activityInfo.Flags.HasFlag(ActivityFlags.Exclusive))
                        totalPriority /= (float)Dorms.Count(d => d.AllowedExclusiveActivities.Contains(i)) / NumDorms;

                    activitiesWriter.WriteLine("\t" + dorms.ToArrayString());
                    if (dormPairs.Any())
                    {
                        activitiesWriter.WriteLine("\tDorm Pairs:");
                        foreach(var dormPair in dormPairs)
                        {
                            activitiesWriter.WriteLine("\t\t" + dormPair);
                        }
                    }

                    activitiesWriter.WriteLine("\tDays:");
                    for (int d = 0; d < days.Length; ++d)
                    {
                        if (days[d].Any())
                        {
                            activitiesWriter.WriteLine("\t\t" + Days[d].DayOfWeek.ToString() + " (" + days[d].Count.ToString() + "): " + days[d].ToArrayString());
                        }
                    }
                    activitiesWriter.WriteLine("\tTotal Priority: " + totalPriority.ToString("#.0"));
                    priorities.Add(new KeyValuePair<string, float>(activityInfo.Abbreviation, totalPriority));
                }
                Console.WriteLine("Activities by Priority: " + priorities.OrderByDescending(a => a.Value).Select(a => a.Key).ToArrayString());
            }

            using (var daysWriter = new StreamWriter(path + "Days.txt"))
            {
                for (int d = 0; d < Days.Length; ++d)
                {
                    float totalPriority = 0;
                    var dayInfo = Days[d];
                    if (dayInfo == null)
                        continue;
                    daysWriter.WriteLine(dayInfo.DayOfWeek.ToString());
                    float numBlocks = dayInfo.Blocks.Count(b => !Blocks[b].IsExcess);
                    var history = new SortedSet<string>(dayInfo.Blocks.SelectMany(b => Blocks[b].ScheduleHistory));
                    Dictionary<int, SortedSet<string>> dormsAndActivities = new Dictionary<int, SortedSet<string>>();

                    float multiplier = 0;
                    if (numBlocks != 0)
                    {
                        float actualNumBlocks = numBlocks * Dorms.Count;
                        foreach (var manualReserved in ManuallyScheduledBlocks)
                        {
                            actualNumBlocks -= manualReserved.Value.Intersect(dayInfo.Blocks).Count();
                        }
                        multiplier = (numBlocks * Dorms.Count) / actualNumBlocks;
                    }
                    else
                        numBlocks = 0.25f;

                    foreach(var block in dayInfo.Blocks)
                    {
                        var blockInfo = Blocks[block];
                        var activities = new List<string>();

                        foreach (var abbrv in blockInfo.ScheduleHistory)
                        {
                            var activity = this[abbrv];
                            var activityInfo = Activities[activity.Activity];
                            if (activityInfo.Flags.HasFlag(ActivityFlags.Manual))
                                continue;
                            activities.Add(activityInfo.Abbreviation);

                            var dormInfo = Dorms[activity.Dorm];
                            float priority = activityInfo.Priority;
                            float dormPriority = DormActivities[activity.Dorm].ActivityPriorities.TryGetValue(activity.Activity, out int activityPriority) ? activityPriority : 0;

                            SortedSet<string> dorms = null;
                            if (!dormsAndActivities.ContainsKey(activity.Activity))
                                dormsAndActivities.Add(activity.Activity, new SortedSet<string>());
                            dorms = dormsAndActivities[activity.Activity];

                            if (activity.HasOther)
                            {
                                float otherDormPriority = DormActivities[activity.OtherDorm].ActivityPriorities.TryGetValue(activity.Activity, out activityPriority) ? activityPriority : 0;
                                priority += dormPriority == 0 || otherDormPriority == 0 ? 0 :
                                        (2 * dormPriority * otherDormPriority) / (dormPriority + otherDormPriority);
                                priority /= 2;
                                priority /= ((dormsNumDays[activity.Dorm] + dormsNumDays[activity.OtherDorm]) / 2) / 4;
                                dorms.Add(dormInfo.Abbreviation + "/" + Dorms[activity.OtherDorm].Abbreviation);
                            }
                            else
                            {
                                dorms.Add(dormInfo.Abbreviation);
                                priority += dormPriority;
                                priority /= 2;
                                priority /= dormsNumDays[activity.Dorm] / 4;
                            }
                            if (activityInfo.Flags.HasFlag(ActivityFlags.Repeatable))
                                priority /= 2;
                            totalPriority += priority;
                        }

                        daysWriter.WriteLine("\t" + blockInfo.Start.ToString() + "  ~  " + activities.ToArrayString());
                    }

                    totalPriority /= numBlocks;
                    totalPriority *= multiplier;
                    
                    //daysWriter.WriteLine("\t" + activities.ToArrayString());
                    daysWriter.WriteLine("\tSchedule:");
                    List<string> filledUp = new List<string>();
                    foreach(var activity in dormsAndActivities)
                    {
                        var activityInfo = Activities[activity.Key];
                        daysWriter.Write("\t\t" + activityInfo.Abbreviation);
                        if (activity.Value.Count == (int)numBlocks / activityInfo.Duration)
                            filledUp.Add(activityInfo.Abbreviation);
                        else
                            daysWriter.Write(" (" + activity.Value.Count + ")");
                        daysWriter.WriteLine(": " + activity.Value.ToArrayString());
                    }
                    if (filledUp.Any())
                        daysWriter.WriteLine("\tFilled Up Activities: " + filledUp.ToArrayString());
                    daysWriter.WriteLine("\tTotal Priority: " + totalPriority.ToString("#.0"));
                }
            }
        }

        #region History_Management
        internal void PrevDay(DayOfWeek day)
        {
            var dayInfo = Days[(int)day];
            int index1 = dayInfo.Backtracking.Count - 2;
            int index2 = dayInfo.Backtracking.Last();

            SortedSet<string> fullHistory = new SortedSet<string>();
            for (int b = dayInfo.Blocks.Count - 1; b >= index2; --b)
            {
                fullHistory.UnionWith(Blocks[dayInfo.Blocks[b]].ScheduleHistory);
            }
            int prevDayCount = fullHistory.Count;
            foreach(var abbrv in fullHistory.Reverse())
            {
                ClearFromPrevDay(abbrv);
            }

            fullHistory.Clear();

            for (int b = index1; b < index2; ++b)
            {
                fullHistory.UnionWith(Blocks[dayInfo.Blocks[b]].ScheduleHistory);
            }

            LogManager.Enqueue(
                "CampSchedules",
                EntryType.DEBUG,
                "Rewinding Day",
                String.Format("{0} - [{1},{2}); {3} rescheduled activities, {4} cleared", day, index1, index2, fullHistory.Count, prevDayCount)
            );

            foreach (var abbrv in fullHistory)
            {
                var scheduledActivity = this[abbrv];
                DormActivities[scheduledActivity.Dorm].RescheduleActivity(Activities[scheduledActivity.Activity], scheduledActivity, false);
                if (scheduledActivity.HasOther)
                    DormActivities[scheduledActivity.OtherDorm].RescheduleActivity(Activities[scheduledActivity.Activity], scheduledActivity, true);
            }

            RANDOMNESS_ENABLED = true;
        }

        private void ClearBlocksFromHistory(DayOfWeek day, int pointerStart, int pointerEnd)
        {
            var dayInfo = Days[(int)day];

            SortedSet<string> fullHistory = new SortedSet<string>();
            for (int b = pointerEnd - 1; b >= pointerStart; --b)
            {
                fullHistory.UnionWith(Blocks[dayInfo.Blocks[b]].ScheduleHistory);
            }

            LogManager.Enqueue(
                "CampSchedules",
                EntryType.DEBUG,
                "Clearing Block Activities",
                String.Format("{0} - [{1},{2}); {3} cleared", day, pointerStart, pointerEnd, fullHistory.Count)
            );

            foreach (var abbrv in fullHistory.Reverse())
            {
                ClearFromPrevDay(abbrv);
            }
        }

        public void ClearFromPrevDay(string abbrv)
        {
            var scheduledActivity = this[abbrv];

            if (ManuallyScheduled.Contains(scheduledActivity.ID))
                return;

            ScheduledActivityAbbrvs.Remove(scheduledActivity.Abbreviation);

            ActivitiesScheduleHistories[scheduledActivity.Activity].Remove(scheduledActivity.Abbreviation);
            DormActivities[scheduledActivity.Dorm].ClearFromPrevDay(scheduledActivity);
            if (scheduledActivity.HasOther)
                DormActivities[scheduledActivity.OtherDorm].ClearFromPrevDay(scheduledActivity, true);

            int maxBlock = scheduledActivity.BlockID + scheduledActivity.Duration;
            for (int blockID = scheduledActivity.BlockID; blockID < maxBlock; ++blockID)
            {
                Blocks[blockID].ScheduleHistory.Remove(scheduledActivity.Abbreviation);
            }
        }

        public void ClearFromHistory(ScheduledActivity scheduledActivity)
        {
            if (ManuallyScheduled.Contains(scheduledActivity.ID))
                return;

            ScheduledActivityAbbrvs.Remove(scheduledActivity.Abbreviation);
            /*if (ScheduledActivityAbbrvs.ContainsKey(scheduled.Abbreviation))
                ScheduledActivityAbbrvs.Remove(scheduled.Abbreviation);
            else
                Utils.DoNothing();*/
            ActivitiesScheduleHistories[scheduledActivity.Activity].Remove(scheduledActivity.Abbreviation);
            DormActivities[scheduledActivity.Dorm].ClearFromHistory(scheduledActivity);
            if (scheduledActivity.HasOther)
                DormActivities[scheduledActivity.OtherDorm].ClearFromHistory(scheduledActivity, true);

            int maxBlock = scheduledActivity.BlockID + scheduledActivity.Duration;
            for (int blockID = scheduledActivity.BlockID; blockID < maxBlock; ++blockID)
            {
                Blocks[blockID].ScheduleHistory.Remove(scheduledActivity.Abbreviation);
            }

            /*int result = DormActivities[scheduled.Dorm].ClearFromHistory(scheduled);
            if (result >= 0)
            {
                if (RecheckDormCompatibility.TryGetValue(scheduled.Dorm, out int prevValue))
                {
                    if (result > prevValue)
                        RecheckDormCompatibility[scheduled.Dorm] = result;
                }
                else
                    RecheckDormCompatibility.Add(scheduled.Dorm, result);
            }
            if (scheduled.HasOther)
            {
                result = DormActivities[scheduled.OtherDorm].ClearFromHistory(scheduled, true);
                if (result >= 0)
                {
                    if (RecheckDormCompatibility.TryGetValue(scheduled.OtherDorm, out int prevValue))
                    {
                        if (result > prevValue)
                            RecheckDormCompatibility[scheduled.OtherDorm] = result;
                    }
                    else
                        RecheckDormCompatibility.Add(scheduled.OtherDorm, result);
                }
            }*/
        }

        public void ClearFromHistory(string scheduledActivityAbbrv)
        {
            if (ScheduledActivityAbbrvs.TryGetValue(scheduledActivityAbbrv, out int index))
            {
                var scheduled = ScheduledActivities[index];
                ClearFromHistory(scheduled);
            }
        }
        #endregion

        public List<DormActivityOption>[] GetOptions(SortedSet<int> availableActivities, int blocksAvailable = 1, bool extendedDurationAvailable = false)
        {
            // Order so that dorms can be matched with other dorms on a triangular matrix
            List<DormActivityOption>[] options = Enumerable.Range(0, NumDorms).Select(n => new List<DormActivityOption>()).ToArray();

            /*int maxAllAdditiveSolo = -1;
            int maxAllMultiplicativeSolo = -1;
            float maxAllCombinedSolo = -1;

            int maxAllAdditiveMulti = -1;
            int maxAllMultiplicativeMulti = -1;
            float maxAllCombinedMulti = -1;*/
            
            foreach(var dormAgeIndex in DormAgeIndices)
            {
                /*int maxAdditiveSolo = -1;
                int maxMultiplicativeSolo = -1;
                float maxCombinedSolo = -1;

                int maxAdditiveMulti = -1;
                int maxMultiplicativeMulti = -1;
                float maxCombinedMulti = -1;*/

                var dorm = Dorms[dormAgeIndex];
                var activities = DormActivities[dorm.ID];
                var dormAvailableActivities = activities.AvailableActivitiesToday.Intersect(availableActivities);
                var dormOptions = options[dorm.ID];

                if (dormAvailableActivities.Any())
                {
                    var otherDorms = activities.DormPriorities.Values.Where(d => d.AvailableToday);
                    var dormRepeats = new SortedSet<int>();
                    bool multiDormAvailable = otherDorms.Any() && activities.AvailableMultiDormActivitiesToday.Any();

                    /*if (multiDormAvailable)
                    {
                        
                    }*/
                    foreach (var otherDorm in otherDorms)
                    {
                        var interDorm = activities.DormPriorities[otherDorm.Dorm];
                        interDorm.Options = 0;
                        if (interDorm.ScheduleHistory.Count > 0)
                            dormRepeats.Add(interDorm.Dorm);
                    }

                    var soloTracking = activities.DormPriorities[dorm.ID];
                    soloTracking.Options = 0;

                    var multiAvailableToday = new SortedSet<int>(activities.AvailableMultiDormActivitiesToday);
                    multiAvailableToday.IntersectWith(availableActivities);

                    foreach (var activity in activities.AvailableActivitiesToday)
                    {
                        if (!availableActivities.Contains(activity))
                            continue;

                        var activityInfo = Activities[activity];
                        if (activityInfo.Flags.HasFlag(ActivityFlags.PriorityExclusive) && !activities.ActivityPriorities.ContainsKey(activity))
                            continue;

                        bool repeatable = activityInfo.Flags.HasFlag(ActivityFlags.Repeatable);
                        bool isRepeatedActivity = repeatable && activities.RepeatableHistory.Contains(activity);

                        int priority = (activities.ActivityPriorities.TryGetValue(
                                activity,
                                out int activityPriority
                            ) ? activityPriority : 0);

                        /*int additive = priority + activityInfo.Priority;
                        int multiplicative = priority * 2;
                        if (priority > 1)
                            multiplicative *= priority;
                        priority = additive;*/

                        if (activityInfo.Flags.HasFlag(ActivityFlags.SingleDorm) && (!soloTracking.PreviousRepeatableActivities.Contains(activityInfo.ID) || !activityInfo.Flags.HasFlag(ActivityFlags.MultiDorm)))
                        {
                            var option = activityInfo.Flags.HasFlag(ActivityFlags.Excess) && (extendedDurationAvailable || blocksAvailable == 0) ?
                                new DormActivityOption(
                                    dorm.ID,
                                    activity,
                                    priority,
                                    true,
                                    // High Ropes and Rock Climbing are the only activities with an extended duration if more blocks are available
                                    extendedDurationAvailable ? 3 : 2
                                /*blocksAvailable >= 2 ?
                                    (
                                        activityInfo.Abbreviation == "HR" ?
                                            3 : (
                                                activityInfo.Abbreviation == "RC" ?
                                                    2 + excessBlocksAvailable :
                                                    activityInfo.Duration
                                            )
                                    ) : activityInfo.Duration*/
                                ) : new DormActivityOption(
                                    dorm.ID,
                                    activity,
                                    priority,
                                    false,
                                    activityInfo.Duration
                                );

                            if (isRepeatedActivity)
                            {
                                option.IsRepeatedActivity = true;
                                ++option.SecondaryScore;
                            }

                            ++soloTracking.Options;
                            dormOptions.Add(option);

                            /*if (additive > maxAdditiveSolo)
                                maxAdditiveSolo = additive;
                            if (multiplicative > maxMultiplicativeSolo)
                                maxMultiplicativeSolo = multiplicative;

                            float combined = 1 - ((float)additive / multiplicative);
                            if (combined > maxCombinedSolo)
                                maxCombinedSolo = combined;*/
                        }

                        if (multiDormAvailable && activityInfo.Flags.HasFlag(ActivityFlags.MultiDorm) && multiAvailableToday.Contains(activityInfo.ID))
                        {
                            //var otherDorms = dorms.Skip(i + 1).TakeWhile(d => (d.AgeGroup - dorm.AgeGroup) <= 1);
                            IEnumerable<InterDormTracking> otherDormsTemp = otherDorms.AsEnumerable();
                            if (repeatable)
                                otherDormsTemp = otherDormsTemp.Where(d => !d.PreviousRepeatableActivities.Contains(activityInfo.ID));
                            otherDormsTemp = otherDormsTemp.OrderByDescending(d => d.Priority);

                            if (activityInfo.Flags.HasFlag(ActivityFlags.Exclusive))
                            {
                                otherDormsTemp = otherDormsTemp.Where(d => Dorms[d.Dorm].AllowedExclusiveActivities.Contains(activityInfo.ID));
                                if (!otherDormsTemp.Any())
                                    continue;
                            }

                            foreach (var otherDorm in otherDormsTemp)
                            {
                                var otherDormActivities = DormActivities[otherDorm.Dorm];
                                if (otherDormActivities.AvailableActivitiesToday.Contains(activityInfo.ID))
                                {
                                    int multiPriority = otherDormActivities.ActivityPriorities.TryGetValue(
                                            activity,
                                            out int otherPriority
                                        ) ? otherPriority : 0;

                                    var option = new DormActivityOption(
                                        dorm.ID,
                                        activityInfo.ID,
                                        priority,
                                        () => otherDorm,
                                        multiPriority,
                                        activityInfo.Duration
                                    );
                                    if (isRepeatedActivity || otherDormActivities.RepeatableHistory.Contains(activityInfo.ID))
                                    {
                                        option.IsRepeatedActivity = true;
                                        ++option.SecondaryScore;
                                    }
                                    if (dormRepeats.Contains(otherDorm.Dorm))
                                    {
                                        option.IsRepeatedDorm = true;
                                        ++option.SecondaryScore;
                                        if (activities.OtherDormsDoneToday.Contains(otherDorm.Dorm))
                                            ++option.SecondaryScore;
                                    }
                                    dormOptions.Add(option);

                                    ++activities.DormPriorities[otherDorm.Dorm].Options;

                                    /*int additiveMulti = additive + multiPriority + otherDorm.Priority;
                                    int multiplicativeMulti = multiplicative * 2 * otherDorm.Priority;
                                    if (multiPriority > 1)
                                        multiplicativeMulti *= multiPriority;

                                    if (additiveMulti > maxAdditiveMulti)
                                        maxAdditiveMulti = additiveMulti;
                                    if (multiplicativeMulti > maxMultiplicativeMulti)
                                        maxMultiplicativeMulti = multiplicativeMulti;

                                    float combined = 1 - ((float)additiveMulti / multiplicativeMulti);

                                    if (combined > maxCombinedMulti)
                                        maxCombinedMulti = combined;*/
                                }
                            }
                        }
                    }
                }

                /*Console.WriteLine(
                    "{0} - {1}/{2}/{3} - {4}/{5}/{6}",
                    dorm.Abbreviation,
                    maxAdditiveSolo,
                    maxMultiplicativeSolo,
                    maxCombinedSolo.ToString("#.00"),
                    maxAdditiveMulti,
                    maxMultiplicativeMulti,
                    maxCombinedMulti.ToString("#.00")
                );

                if (maxAdditiveSolo > maxAllAdditiveSolo)
                    maxAllAdditiveSolo = maxAdditiveSolo;
                if (maxMultiplicativeSolo > maxAllMultiplicativeSolo)
                    maxAllMultiplicativeSolo = maxMultiplicativeSolo;
                if (maxCombinedSolo > maxAllCombinedSolo)
                    maxAllCombinedSolo = maxCombinedSolo;

                if (maxAdditiveMulti > maxAllAdditiveMulti)
                    maxAllAdditiveMulti = maxAdditiveMulti;
                if (maxMultiplicativeMulti > maxAllMultiplicativeMulti)
                    maxAllMultiplicativeMulti = maxMultiplicativeMulti;
                if (maxCombinedMulti > maxAllCombinedMulti)
                    maxAllCombinedMulti = maxCombinedMulti;*/

                // Sorted primarily by priority for each dorm
                options[dorm.ID].Sort(new DormActivityOptionComparer(activities.DormPriorities));
                int optIndex = 0;
                foreach(var option in dormOptions)
                {
                    if (option.SortIndex == -2)
                        option.SortIndex = optIndex;
                    else
                    {
                        option.SortIndex = optIndex;
                        ++optIndex;
                    }
                }
                /*var keys = new SortedSet<int>(activities.DormPriorities.Keys);
                foreach(var key in keys)
                {
                    if ()
                }*/
            }

            /*Console.WriteLine();
            Console.WriteLine(
                "{0} - {1}/{2}/{3} - {4}/{5}/{6}",
                "ALL",
                maxAllAdditiveSolo,
                maxAllMultiplicativeSolo,
                maxAllCombinedSolo.ToString("#.00"),
                maxAllAdditiveMulti,
                maxAllMultiplicativeMulti,
                maxAllCombinedMulti.ToString("#.00")
            );
            Console.ReadLine();*/

            // Return the final options list
            return options;
        }

        private static Predicate<DormActivityOption>[] OPTION_FILTERS = new Predicate<DormActivityOption>[]
        {
            o => !(o.IsRepeatedActivity || o.IsRepeatedDorm),
            o => o.HasExcess,
            o => !o.IsRepeatedActivity,
            o => o.HasOther,
            o => !o.IsRepeatedDorm,
            o => o.Duration > 1,
            o => !(o.IsRepeatedActivity && o.IsRepeatedDorm)
        };

        private const string FORMAT_OVERLAP_ERROR = "Dorm: {0}\r\n\t" +
                                                        "Previously Reserved: {1}\r\n\t" +
                                                        "Newly Reserved: {2}\r\n\t" +
                                                        "Starting/Predicted Block: {3}/{4}\r\n\t" +
                                                        "Scheduled Activity: {5}";

        private delegate void CalcScoreHandler();
        private static event CalcScoreHandler DoCalcScore;

        public ScheduledActivity[] ScheduleActivities(SortedSet<int> availableActivities, List<DormActivityOption>[] options, int startingBlockID, int blocksAvailable = 1, IDictionary<int, SortedSet<int>> manuallyReserved = null, bool extendedDurationAvailable = false, bool RANDOMNESS_ENABLED = false)
        {
            #region PreLoop_Setup
            var dormActivitiesCopy = new DormActivities[NumDorms];
            for (int d = 0; d < NumDorms; ++d)
            {
                dormActivitiesCopy[d] = (DormActivities)DormActivities[d].Clone();
            }

            if (blocksAvailable == 0)
            {
                SortedSet<int> dorms = new SortedSet<int>();
                if (manuallyReserved != null)
                {
                    for (int d = 0; d < NumDorms; ++d)
                    {
                        if (options[d].Count > 0 && !manuallyReserved.ContainsKey(d))
                            dorms.Add(d);
                    }
                }
                else
                {
                    for (int d = 0; d < NumDorms; ++d)
                    {
                        if (options[d].Count > 0)
                            dorms.Add(d);
                    }
                }

                var dormsOrdered = dorms.OrderByDescending(d => options[d].First().TotalPriority).ThenByDescending(d => DormActivities[d].DurationScore);
                var chosen = options[dormsOrdered.First()].First();
                var scheduledActivity = new ScheduledActivity(
                    chosen.Dorm,
                    chosen.Duration,
                    chosen.Activity,
                    startingBlockID
                );

                /*if (ActivityInfo.HighFatigueActivities.Contains(scheduledActivity.Activity))
                    DormActivities[chosen.Dorm].AvailableActivitiesToday.ExceptWith(ActivityInfo.HighFatigueActivities);
                else if (ActivityInfo.LowFatigueActivities.Contains(scheduledActivity.Activity))
                    DormActivities[chosen.Dorm].AvailableActivitiesToday.ExceptWith(ActivityInfo.LowFatigueActivities);*/
                ScheduleActivity(
                    scheduledActivity,
                    Activities[chosen.Activity],
                    DormActivities[chosen.Dorm]
                );
                LastSuccessfulHistory = new SortedSet<string>();
                LastSuccessfulHistory.Add(scheduledActivity.Abbreviation);
                return new ScheduledActivity[] { scheduledActivity };
            }

            bool multiBlock = blocksAvailable > 1;
            int count = options.Length;
            int blockIter = 0;

            SortedSet<int> allBlocks = new SortedSet<int>(Enumerable.Range(startingBlockID, blocksAvailable));

            SortedSet<int> dormsWithOptions = new SortedSet<int>();
            SortedDictionary<int, DormScheduleTracking> dormsWithBlocks = new SortedDictionary<int, DormScheduleTracking>();
            var presetDelegates = new Dictionary<int, PresetDormConflicts>();
            var newConflicts = new List<ConflictNode>();

            SortedSet<int>[] activitiesRemaining = (
                multiBlock ?
                    Activities.Select(
                        a =>
                        {
                            SortedSet<int> blocks = new SortedSet<int>();
                            if (availableActivities.Contains(a.ID))
                            {
                                if (extendedDurationAvailable && a.Flags.HasFlag(ActivityFlags.Excess))
                                    blocks.Add(0);
                                var slotsAvailable = blocksAvailable / a.Duration;
                                for (int b = 0; b < slotsAvailable; ++b)
                                {
                                    blocks.Add(b * a.Duration + startingBlockID);
                                }
                            }
                            
                            return blocks;
                        }
                    ) : Activities.Select(
                        a => new SortedSet<int>() { startingBlockID } //Enumerable.Repeat(startingBlockID, a.Count).ToList()
                    )
            ).ToArray();
            // what the hell is this for
            if (extendedDurationAvailable && multiBlock)
                activitiesRemaining[Activities.Last(a => a.Abbreviation == "HR").ID] = new SortedSet<int>() { startingBlockID };

            // field is reserved Tuesday afternoon
            if (startingBlockID == 8)
                activitiesRemaining[Activities.First(a => a.Abbreviation == "AF").ID] = new SortedSet<int>() { startingBlockID };

            // For each activity available for a dorm, looks at the blocks available and covered by each activity
            // Compares those blocks with the blocks used by the current activity, to see if that activity for the dorm is no longer possible
            bool CheckAddUnavailableActivities(int dormID, DormActivities activities, IEnumerable<int> reservedBlocks, out PresetDormConflicts presets)
            {
                presets = new PresetDormConflicts();

                // this dorm also has to resolve multidorm options where it is the primary dorm
                var tracker = dormsWithBlocks[dormID];
                var hasMultiDormOptions = tracker.OtherDormOptionsRemaining > 0;

                foreach (var activity in activities.AvailableActivitiesToday.Intersect(availableActivities))
                {
                    var info = Activities[activity];

                    SortedSet<int> remainingBlocks = new SortedSet<int>(activitiesRemaining[activity]);
                    bool noPreviousConflicts = false;
                    if (tracker.Conflicts.TryGetValue(activity, out SortedSet<int> previousActivityConflicts))
                        remainingBlocks.ExceptWith(previousActivityConflicts);
                    else
                    {
                        noPreviousConflicts = true;
                        previousActivityConflicts = new SortedSet<int>();
                    }

                    int previousConflictsCount = previousActivityConflicts.Count;

                    bool activityAvailable = false;

                    if (info.Duration > 1)
                    {
                        //List<int> availableBlocks = new List<int>();
                        SortedSet<int> potentialConflicts = new SortedSet<int>();

                        // These activities operate on different rules
                        int duration = extendedDurationAvailable && info.Flags.HasFlag(ActivityFlags.Excess) ? 3 : info.Duration;
                        foreach (var remainingBlock in remainingBlocks)
                        {
                            var activitySlotBlocks = Enumerable.Range(remainingBlock, duration);
                            if (!reservedBlocks.Any(r => activitySlotBlocks.Contains(r)))
                                activityAvailable = true;
                            else
                                potentialConflicts.Add(remainingBlock);
                        }
                        previousActivityConflicts.UnionWith(potentialConflicts);
                    }
                    else //if (remainingBlocks.Overlaps(reservedBlocks))
                    {
                        int remainingCount = remainingBlocks.Count;
                        remainingBlocks.IntersectWith(reservedBlocks);
                        //remainingBlocks.RemoveWhere(rb => !reservedBlocks.Contains(rb));
                        if (remainingCount != remainingBlocks.Count)
                        {
                            activityAvailable = true;
                            previousActivityConflicts.UnionWith(remainingBlocks);
                        }
                        /*else if (remainingBlocks.Any())
                            previousActivityConflicts.AddRange(remainingBlocks);*/
                    }

                    if (activityAvailable)
                    {
                        if (previousActivityConflicts.Count > previousConflictsCount)
                        {
                            if (noPreviousConflicts)
                                tracker.Conflicts.Add(activity, previousActivityConflicts);
                            else
                                tracker.Conflicts[activity] = previousActivityConflicts;
                            if (info.MaxDorms > 1 && hasMultiDormOptions)
                            {
                                newConflicts.Add(new ConflictNode(dormID, activity, true));
                                presets.FreshActivityConflicts.Add(activity);
                            }
                        }
                    }
                    else
                    {
                        if (info.MaxDorms > 1)
                            newConflicts.Add(new ConflictNode(dormID, activity, false));
                        presets.UnavailableActivities.Add(activity);
                        if (!noPreviousConflicts)
                            tracker.Conflicts.Remove(activity);
                    }


                    /*if (activityAvailable && !noPreviousConflicts)
                    {
                        previousActivityConflicts = previousActivityConflicts.Except(reservedBlocks).ToList();
                        if (previousActivityConflicts.Count == 0)
                            previousConflicts.Remove(activity);
                    }*/
                }

                var unavailableCount = presets.UnavailableActivities.Count;
                bool result = unavailableCount > 0;
                if (result && unavailableCount > 1)
                {
                    var remaining = activities.AvailableActivitiesToday.Except(presets.UnavailableActivities);
                    LogManager.Enqueue(
                        new LogUpdate(
                            "CampSchedules", EntryType.DEBUG, new object[] {
                                "Dorm " + dormID.ToString(),
                                String.Format(
                                    "{0} activities remaining out of available {1}, lost {2} out of previous {3}", 
                                    remaining.Count(), 
                                    activities.AvailableActivities.Count, 
                                    unavailableCount, 
                                    activities.AvailableActivitiesToday.Count
                                ), "Unavailable Activities: [" + String.Join(", ", presets.UnavailableActivities) + "]",
                                "Remaining Activities: [" + String.Join(", ", remaining) + "]"
                            }
                        )
                    );
                }
                return result;
            }

            int prevRemainingDorms = dormsWithBlocks.Count;

            void ResolveConflicts(int activity, int activityBlockUsed = -1)
            {
                void ResolveFreshConflicts(DormActivities activities, ConflictsResolveTracking conflictsTracker, PresetDormConflicts thisDormPresets, DormScheduleTracking tracker)
                {
                    // if this dorm is a primary dorm, then it needs to resolve its own conflicts
                    SortedSet<int> presets = new SortedSet<int>(thisDormPresets.UnavailableActivities);
                    presets.IntersectWith(ActivityInfo.MultiDormActivities);
                    SortedSet<int> availableAnyDorm = new SortedSet<int>();
                    SortedSet<int> unavailableAnyDorm = new SortedSet<int>();
                    var includedDorms = new int[conflictsTracker.IncludedDorms.Count];
                    conflictsTracker.IncludedDorms.CopyTo(includedDorms);
                    foreach (var otherDorm in includedDorms)
                    {
                        // get all multidorm activities both dorms can participate in today
                        if (activities.CheckCompatibility(DormActivities[otherDorm], out SortedSet<int> overlapping))
                        {
                            overlapping.ExceptWith(presets);
                            if (overlapping.Count == 0)
                                conflictsTracker.AddDormForClearing(otherDorm);
                            else if (overlapping.Overlaps(thisDormPresets.FreshActivityConflicts)) // check to make sure there's actually potential conflict
                            {
                                var otherTracker = dormsWithBlocks[otherDorm];
                                SortedSet<int> baseBlocks = new SortedSet<int>(tracker.ReservedBlocks);
                                baseBlocks.UnionWith(otherTracker.ReservedBlocks);

                                if (baseBlocks.SetEquals(allBlocks))
                                    conflictsTracker.AddDormForClearing(otherDorm);
                                else
                                {
                                    overlapping.RemoveWhere(
                                        a =>
                                        {
                                            bool result = false;
                                            // only bothering to check activities that have fresh changes
                                            if (thisDormPresets.FreshActivityConflicts.Contains(a))
                                            {
                                                var activityBlocks = activitiesRemaining[a];
                                                if (!baseBlocks.IsSupersetOf(activityBlocks))
                                                {
                                                    var blocks = new SortedSet<int>(baseBlocks);
                                                    if (tracker.Conflicts.TryGetValue(a, out SortedSet<int> activityConflicts))
                                                        blocks.UnionWith(activityConflicts);
                                                    if (otherTracker.Conflicts.TryGetValue(a, out SortedSet<int> otherActivityConflicts))
                                                        blocks.UnionWith(otherActivityConflicts);
                                                    result = !blocks.IsSupersetOf(activityBlocks);
                                                }
                                            }
                                            if (result)
                                                availableAnyDorm.Add(a);
                                            return !result;
                                        }
                                     );
                                }

                                // overlapping now contains any incompatible activities
                                if (overlapping.Count > 0)
                                {
                                    conflictsTracker.InterDormActivityConflicts.Add(otherDorm, overlapping);
                                    unavailableAnyDorm.UnionWith(overlapping);
                                }
                            }
                        }
                        else // these two dorms can no longer possibly interact today
                            conflictsTracker.AddDormForClearing(otherDorm);
                    }

                    unavailableAnyDorm.ExceptWith(availableAnyDorm);
                    if (unavailableAnyDorm.Count > 0)
                    {
                        SortedSet<int> availableSolo = new SortedSet<int>();
                        foreach (var unavailableActivity in unavailableAnyDorm)
                        {
                            if (Activities[unavailableActivity].MinDorms == 2)
                                conflictsTracker.ActivitiesToClear.Add(unavailableActivity);
                            else
                                availableSolo.Add(unavailableActivity);
                        }

                        foreach (var otherDorm in conflictsTracker.IncludedDorms)
                        {
                            conflictsTracker.InterDormActivityConflicts[otherDorm].ExceptWith(unavailableAnyDorm);
                        }

                        if (availableSolo.Count > 0)
                            conflictsTracker.MainDelegates.Add(o => !o.HasOther && availableSolo.Contains(o.Activity));
                    }
                }

                bool activityNull = activity == -1;
                bool presetNull = presetDelegates.Count == 0;
                bool clearActivity = !activityNull && activityBlockUsed == -1;
                bool isMultiDormActivity = !activityNull && !clearActivity && Activities[activity].Flags.HasFlag(ActivityFlags.MultiDorm);

                ActivityInfo activityInfo = null;
                SortedSet<int> activitiesToCheck = null;
                //bool incompatibleActivities = false;
                
                if (!activityNull)
                {
                    activityInfo = Activities[activity];
                    activitiesToCheck = new SortedSet<int>();
                    activitiesToCheck.Add(activity);
                    activitiesToCheck.UnionWith(activityInfo.IncompatibleActivities);

                    if (activityBlockUsed == -1)
                        availableActivities.Remove(activity);
                }


                if (newConflicts.Count > 0)
                {
                    SortedSet<int> dormsBeingCleared = new SortedSet<int>(newConflicts.Where(c => c.Activity == -1).Select(c => c.Dorm));
                    if (dormsBeingCleared.Count > 0)
                        newConflicts.RemoveAll(c => c.Activity == -1 || c.Activity == activity);
                    IEnumerable<IGrouping<int, ConflictNode>> newConflictsGrouped = newConflicts.GroupBy(c => c.Dorm);

                    foreach (var dorm in dormsWithBlocks)
                    {
                        var activities = DormActivities[dorm.Key];
                        
                        var soloTracking = activities.DormPriorities[dorm.Key];
                        ConflictsResolveTracking conflictsTracker = new ConflictsResolveTracking(dorm.Key, dorm.Value.OtherDormOptionsRemaining > 0);
                        PresetDormConflicts thisDormDelegates = null;
                        if (!presetNull && presetDelegates.TryGetValue(dorm.Key, out thisDormDelegates))
                            conflictsTracker.HandlePreset(thisDormDelegates);
                        
                        var multiAvailableToday = new SortedSet<int>(activities.AvailableMultiDormActivitiesToday);
                        multiAvailableToday.IntersectWith(availableActivities);

                        // the variable could've been modified
                        if (conflictsTracker.GetPotentialConflictingDorms(dorm.Key, dormsBeingCleared, activities.DormPriorities) && !conflictsTracker.IsPresetDorm)
                        {
                            //var reservedBlocks = dormsReservedBlocks[dorm];
                            foreach (var conflictedDorm in newConflictsGrouped)
                            {
                                /*if (conflict.Key == dorm)
                                {
                                    mainDelegates.Add(o => o.Activity == conflict.Value);
                                }
                                else */
                                // Making sure these conflicts are relevant
                                if (conflictsTracker.IncludedDorms.Contains(conflictedDorm.Key))   
                                {
                                    var otherTracker = dormsWithBlocks[conflictedDorm.Key];
                                    var blocks = new SortedSet<int>();
                                    blocks.UnionWith(otherTracker.ReservedBlocks);
                                    blocks.UnionWith(dorm.Value.ReservedBlocks);

                                    if (blocks.Count == blocksAvailable)
                                        conflictsTracker.DormsToClear.Add(conflictedDorm.Key);
                                    else
                                    {
                                        var dormConflicts = new SortedSet<int>(
                                            conflictedDorm.Where(c => c.Conditional).Select(c => c.Activity).Concat(otherTracker.Conflicts.Keys)
                                        );
                                        dormConflicts.IntersectWith(multiAvailableToday);
                                        foreach (var conflict in dormConflicts)
                                        {
                                            SortedSet<int> conflictBlocks = new SortedSet<int>(blocks);
                                            conflictBlocks.UnionWith(otherTracker.Conflicts[conflict]);
                                            if (dorm.Value.Conflicts.TryGetValue(conflict, out SortedSet<int> dormActivityBlocks))
                                                conflictBlocks.UnionWith(dormActivityBlocks);

                                            if (conflictBlocks.IsSupersetOf(activitiesRemaining[conflict]))
                                                conflictsTracker.AddInterDormActivityConflict(conflictedDorm.Key, conflict);
                                            
                                        }
                                        foreach(var conflict in conflictedDorm.Where(c => !c.Conditional))
                                        {
                                            if (multiAvailableToday.Contains(conflict.Activity))
                                                conflictsTracker.AddInterDormActivityConflict(conflict.Dorm, conflict.Activity);
                                        }
                                    }
                                }
                            }
                        }

                        if (conflictsTracker.IsPresetDorm)
                        {
                            if (conflictsTracker.HasMultiDormOptions)
                                ResolveFreshConflicts(activities, conflictsTracker, thisDormDelegates, dorm.Value);
                        }
                        else if (!activityNull && activitiesToCheck.Overlaps(activities.AvailableActivitiesToday))
                        {
                            if (clearActivity)
                                conflictsTracker.ActivitiesToClear.UnionWith(activitiesToCheck);
                            else
                            {
                                var freshConflicts = new SortedSet<int>();
                                freshConflicts.UnionWith(activitiesToCheck);
                                freshConflicts.IntersectWith(activities.AvailableActivitiesToday);

                                foreach(var freshConflict in freshConflicts)
                                {
                                    var blocks = new SortedSet<int>(dorm.Value.ReservedBlocks);
                                    var activityBlocks = new SortedSet<int>(activitiesRemaining[freshConflict]);
                                    if (dorm.Value.Conflicts.TryGetValue(freshConflict, out SortedSet<int> activityConflictsTemp))
                                        blocks.UnionWith(activityConflictsTemp);
                                    if (!blocks.Contains(activityBlockUsed)) // its possible this dorm has already reserved and resolved that block
                                    {
                                        activityBlocks.ExceptWith(blocks);
                                        if (activityBlocks.Count == 0)
                                        {
                                            conflictsTracker.ActivitiesToClear.Add(freshConflict);
                                            continue;
                                        }
                                    }

                                    // however, secondary dorms might now be incompatible
                                    if (isMultiDormActivity && activities.AvailableMultiDormActivitiesToday.Contains(freshConflict))
                                    {
                                        activityBlocks.ExceptWith(blocks);
                                        // checking to see if this creates any activity-sourced conflicts
                                        foreach (var otherDorm in conflictsTracker.IncludedDorms)
                                        {
                                            if (DormActivities[otherDorm].AvailableMultiDormActivitiesToday.Contains(freshConflict))
                                            {
                                                var otherDormTracker = dormsWithBlocks[otherDorm];
                                                var otherDormBlocks = new SortedSet<int>(otherDormTracker.ReservedBlocks);
                                                if (otherDormTracker.Conflicts.TryGetValue(freshConflict, out SortedSet<int> otherDormConflictsTemp))
                                                    otherDormBlocks.UnionWith(otherDormConflictsTemp);
                                                // other dorm may have already resolved this conflict
                                                if (!otherDormBlocks.Contains(activityBlockUsed))
                                                {
                                                    otherDormBlocks.ExceptWith(blocks);
                                                    if (otherDormBlocks.Count > 0)
                                                    {
                                                        activityBlocks.ExceptWith(otherDormBlocks);
                                                        if (activityBlocks.Count == 0)
                                                            conflictsTracker.AddInterDormActivityConflict(otherDorm, freshConflict);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // its possible that this dorm got away without any conflicts, somehow
                        if (conflictsTracker.SelectDelegates())
                        {
                            if (conflictsTracker.DoDormDelegates)
                            {
                                options[dorm.Key].RemoveAll(
                                    o =>
                                    {
                                        if (conflictsTracker.MainDelegates.Any(d => d(o)))
                                        {
                                            if (o.HasOther)
                                            {
                                                if (conflictsTracker.IncludedDorms.Contains(o.OtherDorm))
                                                    --activities.DormPriorities[o.OtherDorm].Options;
                                            }
                                            else
                                                --soloTracking.Options;
                                            return true;
                                        }
                                        return false;
                                    }
                                );
                                foreach (var otherDorm in conflictsTracker.DormsToClear.Except(activities.UsedUpOtherDorms.Select(d => d.Dorm)))
                                {
                                    activities.DormPriorities[otherDorm].Options = 0;
                                }
                                if (conflictsTracker.HasMultiDormOptions)
                                    options[dorm.Key].Sort(new DormActivityOptionComparer(activities.DormPriorities));
                            }
                            else
                                options[dorm.Key].RemoveAll(
                                    o =>
                                    {
                                        if (conflictsTracker.MainDelegates.Any(d => d(o)))
                                        {
                                            --soloTracking.Options;
                                            return true;
                                        }
                                        return false;
                                    }
                                );
                        }
                        /*else
                        {
                            var logEntry = new List<string>()
                            {
                                "Resolving Dorm " + dorm.Key.ToString(),
                                "No conflicts found - " + dorm.Value.OptionsLeft + "/" + dorm.Value.OtherDormOptionsRemaining + " options left"
                            };
                            if (dorm.Value.OptionsLeft > 4)
                                logEntry.Add("[" + String.Join(", ", activities.AvailableActivitiesToday) + "]");
                            LogManager.Enqueue(
                                new LogUpdate(
                                    "CampSchedules",
                                    EntryType.DEBUG,
                                    logEntry
                                )
                            );
                        }*/
                    }

                    newConflicts.Clear();
                }
                else if (clearActivity)
                {
                    foreach (var dorm in dormsWithBlocks)
                    {
                        var activities = DormActivities[dorm.Key];
                        var soloTracking = activities.DormPriorities[dorm.Key];

                        if (!presetNull && presetDelegates.TryGetValue(dorm.Key, out PresetDormConflicts thisDormDelegates))
                        {
                            ConflictsResolveTracking conflictsTracker = new ConflictsResolveTracking(dorm.Key, dorm.Value.OtherDormOptionsRemaining > 0);
                            conflictsTracker.GetPotentialConflictingDorms(dorm.Key, null, activities.DormPriorities);
                            conflictsTracker.HandlePreset(thisDormDelegates);

                            if (conflictsTracker.HasMultiDormOptions)
                                ResolveFreshConflicts(activities, conflictsTracker, thisDormDelegates, dorm.Value);

                            if (conflictsTracker.SelectDelegates())
                            {
                                if (conflictsTracker.DoDormDelegates)
                                {
                                    options[dorm.Key].RemoveAll(
                                        o =>
                                        {
                                            if (conflictsTracker.MainDelegates.Any(d => d(o)))
                                            {
                                                if (o.HasOther)
                                                {
                                                    if (conflictsTracker.IncludedDorms.Contains(o.OtherDorm))
                                                        --activities.DormPriorities[o.OtherDorm].Options;
                                                }
                                                else
                                                    --soloTracking.Options;
                                                return true;
                                            }
                                            return false;
                                        }
                                    );
                                    foreach (var otherDorm in conflictsTracker.DormsToClear.Except(activities.UsedUpOtherDorms.Select(d => d.Dorm)))
                                    {
                                        activities.DormPriorities[otherDorm].Options = 0;
                                    }
                                    if (conflictsTracker.HasMultiDormOptions)
                                        options[dorm.Key].Sort();
                                }
                                else
                                    options[dorm.Key].RemoveAll(
                                        o =>
                                        {
                                            if (conflictsTracker.MainDelegates.Any(d => d(o)))
                                            {
                                                --soloTracking.Options;
                                                return true;
                                            }
                                            return false;
                                        }
                                    );
                            }
                        }
                        else if (activities.AvailableActivitiesToday.Overlaps(activitiesToCheck))
                        {
                            var freshConflicts = new SortedSet<int>();
                            freshConflicts.UnionWith(activitiesToCheck);
                            freshConflicts.IntersectWith(activities.AvailableActivitiesToday);

                            if (dorm.Value.OtherDormOptionsRemaining > 0)
                            {
                                options[dorm.Key].RemoveAll(
                                    o =>
                                    {
                                        if (activitiesToCheck.Contains(o.Activity))
                                        {
                                            if (o.HasOther)
                                                --activities.DormPriorities[o.OtherDorm].Options;
                                            else
                                                --soloTracking.Options;
                                            return true;
                                        }
                                        return false;
                                    }
                                );
                                options[dorm.Key].Sort(new DormActivityOptionComparer(activities.DormPriorities));
                            }
                            else
                                options[dorm.Key].RemoveAll(
                                    o =>
                                    {
                                        if (activitiesToCheck.Contains(o.Activity))
                                        {
                                            --soloTracking.Options;
                                            return true;
                                        }
                                        return false;
                                    }
                                );
                        }

                    }
                }
                else if (!presetNull || !activityNull)
                {
                    // even if none of the other dorms have any resolving to do, the presets do
                    foreach (var dorm in dormsWithBlocks)
                    {
                        var activities = DormActivities[dorm.Key];
                        var soloTracking = activities.DormPriorities[dorm.Key];

                        if (!presetNull && presetDelegates.TryGetValue(dorm.Key, out PresetDormConflicts thisDormDelegates))
                        {
                            ConflictsResolveTracking conflictsTracker = new ConflictsResolveTracking(dorm.Key, dorm.Value.OtherDormOptionsRemaining > 0);
                            conflictsTracker.GetPotentialConflictingDorms(dorm.Key, null, activities.DormPriorities);
                            conflictsTracker.HandlePreset(thisDormDelegates);

                            if (conflictsTracker.HasMultiDormOptions)
                                ResolveFreshConflicts(activities, conflictsTracker, thisDormDelegates, dorm.Value);

                            if (conflictsTracker.SelectDelegates())
                            {
                                if (conflictsTracker.DoDormDelegates)
                                {
                                    options[dorm.Key].RemoveAll(
                                        o =>
                                        {
                                            if (conflictsTracker.MainDelegates.Any(d => d(o)))
                                            {
                                                if (o.HasOther)
                                                {
                                                    if (conflictsTracker.IncludedDorms.Contains(o.OtherDorm))
                                                        --activities.DormPriorities[o.OtherDorm].Options;
                                                }
                                                else
                                                    --soloTracking.Options;
                                                return true;
                                            }
                                            return false;
                                        }
                                    );

                                    foreach (var otherDorm in conflictsTracker.DormsToClear.Except(activities.UsedUpOtherDorms.Select(d => d.Dorm)))
                                    {
                                        activities.DormPriorities[otherDorm].Options = 0;
                                    }

                                    if (conflictsTracker.HasMultiDormOptions)
                                        options[dorm.Key].Sort();
                                }
                                else
                                    options[dorm.Key].RemoveAll(
                                        o =>
                                        {
                                            if (conflictsTracker.MainDelegates.Any(d => d(o)))
                                            {
                                                --soloTracking.Options;
                                                return true;
                                            }
                                            return false;
                                        }
                                    );
                            }
                        }
                        else if (!activityNull && activities.AvailableActivitiesToday.Overlaps(activitiesToCheck))
                        {
                            ConflictsResolveTracking conflictsTracker = new ConflictsResolveTracking(dorm.Key, dorm.Value.OtherDormOptionsRemaining > 0);
                            conflictsTracker.GetPotentialConflictingDorms(dorm.Key, null, activities.DormPriorities);

                            var freshConflicts = new SortedSet<int>();
                            freshConflicts.UnionWith(activitiesToCheck);
                            freshConflicts.IntersectWith(activities.AvailableActivitiesToday);

                            foreach(var freshConflict in freshConflicts)
                            {
                                var blocks = new SortedSet<int>(dorm.Value.ReservedBlocks);
                                var activityBlocks = new SortedSet<int>(activitiesRemaining[freshConflict]);
                                if (dorm.Value.Conflicts.TryGetValue(freshConflict, out SortedSet<int> activityConflictsTemp))
                                    blocks.UnionWith(activityConflictsTemp);
                                if (!blocks.Contains(activityBlockUsed)) // its possible this dorm has already reserved and resolved that block
                                {
                                    activityBlocks.ExceptWith(blocks);
                                    if (activityBlocks.Count == 0)
                                    {
                                        conflictsTracker.ActivitiesToClear.Add(freshConflict);
                                        continue;
                                    }
                                }

                                // however, secondary dorms might now be incompatible
                                if (isMultiDormActivity && conflictsTracker.HasMultiDormOptions)
                                {
                                    var otherDormActivityConflicts = new SortedSet<int>();
                                    activityBlocks.ExceptWith(blocks);
                                    // checking to see if this creates any activity-sourced conflicts
                                    foreach (var otherDorm in conflictsTracker.IncludedDorms)
                                    {
                                        if (DormActivities[otherDorm].AvailableMultiDormActivitiesToday.Contains(freshConflict))
                                        {
                                            var otherDormTracker = dormsWithBlocks[otherDorm];
                                            var otherDormBlocks = new SortedSet<int>(otherDormTracker.ReservedBlocks);
                                            if (otherDormTracker.Conflicts.TryGetValue(freshConflict, out SortedSet<int> otherDormConflictsTemp))
                                                otherDormBlocks.UnionWith(otherDormConflictsTemp);
                                            // other dorm may have already resolved this conflict
                                            if (!otherDormBlocks.Contains(activityBlockUsed))
                                            {
                                                otherDormBlocks.ExceptWith(blocks);
                                                if (otherDormBlocks.Count > 0)
                                                {
                                                    activityBlocks.ExceptWith(otherDormBlocks);
                                                    if (activityBlocks.Count == 0)
                                                        otherDormActivityConflicts.Add(otherDorm);
                                                }
                                            }
                                        }
                                    }

                                    if (otherDormActivityConflicts.Count > 0)
                                    {
                                        if (otherDormActivityConflicts.Count == 1)
                                        {
                                            int otherDorm = otherDormActivityConflicts.First();
                                            conflictsTracker.AddInterDormActivityConflict(otherDorm, freshConflict);
                                        }
                                        else if (!Activities[freshConflict].Flags.HasFlag(ActivityFlags.SingleDorm))
                                            conflictsTracker.ActivitiesToClear.Add(freshConflict);
                                        else
                                        {
                                            foreach(var otherDorm in otherDormActivityConflicts)
                                            {
                                                conflictsTracker.AddInterDormActivityConflict(otherDorm, freshConflict);
                                            }
                                        }
                                    }
                                }
                            }
                            

                            if (conflictsTracker.SelectDelegates())
                            {
                                if (conflictsTracker.DoDormDelegates)
                                {
                                    options[dorm.Key].RemoveAll(
                                        o =>
                                        {
                                            if (conflictsTracker.MainDelegates.Any(d => d(o)))
                                            {
                                                if (o.HasOther)
                                                    --activities.DormPriorities[o.OtherDorm].Options;
                                                else
                                                    --soloTracking.Options;
                                                return true;
                                            }
                                            return false;
                                        }
                                    );
                                    
                                    if (conflictsTracker.HasMultiDormOptions)
                                        options[dorm.Key].Sort(new DormActivityOptionComparer(activities.DormPriorities));
                                }
                                else
                                    options[dorm.Key].RemoveAll(
                                        o =>
                                        {
                                            if (conflictsTracker.MainDelegates.Any(d => d(o)))
                                            {
                                                --soloTracking.Options;
                                                return true;
                                            }
                                            return false;
                                        }
                                    );
                            }
                        }
                    }
                }

                CountOptions();

                if (!presetNull)
                    presetDelegates.Clear();
            }

            int changed = 0;
            void CountOptions(bool log = false)
            {
                SortedDictionary<int, int> dormOptionsRemaining = new SortedDictionary<int, int>();

                var dorms = DormAgeIndices.Intersect(dormsWithBlocks.Keys);
                if (log)
                {
                    changed = 0;
                    List<int> optionsLeft = new List<int>();
                    List<int> changes = new List<int>();
                    foreach (var dorm in dorms)
                    {
                        var tracker = dormsWithBlocks[dorm];
                        if (
                            tracker.OptionsRemaining(
                                DormActivities[dorm].DormPriorities.Values,
                                dormOptionsRemaining//, options[dorm].Count
                            )
                        )
                        {
                            ++changed;
                            changes.Add(tracker.OptionsLeft - tracker.OtherDormOptionsRemaining);
                            if (!tracker.HasOptions && dormsWithOptions.Contains(dorm))
                                dormsWithOptions.Remove(dorm);
                        }
                        else
                            changes.Add(0);
                        optionsLeft.Add(tracker.OptionsLeft);
                    }
                    if (changed > 1)
                        LogManager.Enqueue(
                            new LogUpdate(
                                "CampSchedules",
                                EntryType.DEBUG,
                                new object[]
                                {
                                String.Format("Block: {0} + ({1}/{2})", startingBlockID, blockIter, blocksAvailable),
                                changed + " dorms out of " + prevRemainingDorms + " affected; " + dormsWithBlocks.Count + " dorms remaining",
                                "Dorms Left: " + dormsWithBlocks.Keys.ToArrayString(),
                                "Options Left: " + optionsLeft.ToArrayString(),
                                "Change: " + changes.ToArrayString()
                                }
                            )
                        );
                    
                }
                else
                {
                    foreach (var dorm in dorms)
                    {
                        var tracker = dormsWithBlocks[dorm];
                        if (
                            tracker.OptionsRemaining(
                                DormActivities[dorm].DormPriorities.Values,
                                dormOptionsRemaining//, options[dorm].Count
                            ) && !tracker.HasOptions && dormsWithOptions.Contains(dorm)
                        )
                            dormsWithOptions.Remove(dorm);
                    }
                }

                DoCalcScore();
                prevRemainingDorms = dormsWithBlocks.Count;
            }

            if (manuallyReserved != null)
            {
                //SortedDictionary<int, List<int>> clearList = new SortedDictionary<int, List<int>>();
                for (int d = 0; d < count; ++d)
                {
                    bool hasManualBlocks = manuallyReserved.TryGetValue(d, out SortedSet<int> reserved);
                    if (DormScheduleTracking.TryCreate(d, blocksAvailable, hasManualBlocks ? reserved : null, out DormScheduleTracking tracker))
                    {
                        DoCalcScore += tracker.CalculateScore;
                        dormsWithBlocks.Add(d, tracker);
                        if (hasManualBlocks)
                        {
                            CheckAddUnavailableActivities(d, DormActivities[d], reserved, out PresetDormConflicts presets);
                            presetDelegates.Add(d, presets);
                        }
                    }
                    else // other dorms still have this dorm listed as available
                    {
                        newConflicts.Add(new ConflictNode(d, -1));
                        options[d].Clear();
                    }
                }

                CountOptions(true);

                /*for (int i = 0; i < count; ++i)
                {
                    options[i].RemoveAll(o => o.HasOther && clearList.TryGetValue(o.OtherDorm, out List<int> unavailableTemp) && unavailableTemp.Contains(o.Activity));
                }*/

                ResolveConflicts(-1);
            }
            else
            {
                for (int d = 0; d < count; ++d)
                {
                    if (options[d].Count > 0)
                    {
                        var tracker = new DormScheduleTracking(d, blocksAvailable);
                        DoCalcScore += tracker.CalculateScore;
                        dormsWithBlocks.Add(d, tracker);
                    }
                }

                CountOptions(true);
            }


            //int[] fairShares = new int[count];

            SortedSet<CSVKeyValuePair<int, int>> previousConcurrentBlocks = new SortedSet<CSVKeyValuePair<int, int>>();
            int maxRemainingBlocks = blocksAvailable;
            List<ScheduledActivity> scheduled = new List<ScheduledActivity>();
            #endregion

            while (dormsWithBlocks.Count > 0)
            {
                #region Options_Setup
                dormsWithOptions.Clear();
                SortedSet<int> notAssigned = new SortedSet<int>();

                if (maxRemainingBlocks == 0 || (RANDOMNESS_ENABLED && GEN.NextDouble() > CHANCE_FIRST_ELEMENT))
                {
                    foreach (var dorm in dormsWithBlocks.Values)
                    {
                        notAssigned.Add(dorm.Dorm);
                        if (dorm.HasOptions)
                            dormsWithOptions.Add(dorm.Dorm);
                    }
                }
                else
                {
                    foreach (var dorm in dormsWithBlocks.Values.Where(d => d.RemainingBlocks >= maxRemainingBlocks))
                    {
                        notAssigned.Add(dorm.Dorm);
                        if (dorm.HasOptions)
                            dormsWithOptions.Add(dorm.Dorm);
                    }
                    if (dormsWithOptions.Count == 0)
                    {
                        foreach (var dorm in dormsWithBlocks.Values)
                        {
                            notAssigned.Add(dorm.Dorm);
                            if (dorm.HasOptions)
                                dormsWithOptions.Add(dorm.Dorm);
                        }
                    }
                }

                if (dormsWithOptions.Count == 0)
                {
                    /*if (GEN.NextDouble() < CHANCE_THIRD_REPEATS)
                    {
                        var repeatableActivities = Activities.Where(
                            a => a.Flags.HasFlag(ActivityFlags.Repeatable) &&
                                !a.Flags.HasFlag(ActivityFlags.Manual) &&
                                activitiesRemaining[a.ID].Count > 0
                        );

                        if (repeatableActivities.Any())
                        {
                            var dorms = new SortedSet<int>(dormsWithBlocks.Values.Select(d => d.Dorm));
                            var failedDorms = new SortedSet<int>();

                            foreach (var dorm in dormsWithBlocks.Values.OrderBy(d => Array.IndexOf(DormAgeIndices, d.Dorm)))
                            {
                                var dormInfo = Dorms[dorm.Dorm];
                                var activities = DormActivities[dorm.Dorm];
                                foreach (var activity in repeatableActivities)
                                {
                                    if (
                                        activity.Flags.HasFlag(ActivityFlags.Exclusive) &&
                                        !dormInfo.AllowedExclusiveActivities.Contains(activity.ID)
                                    )
                                        continue;

                                    if (
                                        activities.AvailableActivities.Contains(activity.ID) ||
                                        !activities.RepeatableHistory.Contains(activity.ID) ||              // this is a last resort, if it isn't for this activity, we know right away
                                        dorm.ReservedBlocks.IsSupersetOf(activitiesRemaining[activity.ID]) ||
                                        activities.RepeatableDoubleHistory.Contains(activity.ID) ||
                                        activities.RepeatableTodayHistory.Contains(activity.ID)
                                    )
                                        continue;

                                    var priority = activities.ActivityPriorities.TryGetValue(activity.ID, out int priorityTemp) ? priorityTemp : 0;

                                    if (activity.Flags.HasFlag(ActivityFlags.MultiDorm))
                                    {
                                        if (activities.DormPriorities.Count > 1)
                                        {
                                            foreach (var otherDorm in activities.DormPriorities.Keys)
                                            {
                                                if (
                                                    otherDorm == dorm.Dorm ||
                                                    !dorms.Contains(otherDorm) ||
                                                    dormsWithBlocks[otherDorm].ReservedBlocks.IsSupersetOf(activitiesRemaining[activity.ID])
                                                )
                                                    continue;

                                                var otherActivities = DormActivities[otherDorm];
                                                if (otherActivities.RepeatableDoubleHistory.Contains(activity.ID))
                                                    continue;

                                                options[dorm.Dorm].Add(
                                                    new DormActivityOption(
                                                        dorm.Dorm,
                                                        activity.ID,
                                                        priority,
                                                        () => activities.DormPriorities[otherDorm],
                                                        otherActivities.ActivityPriorities.TryGetValue(activity.ID, out priorityTemp) ? priorityTemp : 0
                                                    )
                                                );
                                            }
                                        }
                                    }

                                    if (activity.Flags.HasFlag(ActivityFlags.SingleDorm))
                                        options[dorm.Dorm].Add(
                                            new DormActivityOption(
                                                dorm.Dorm,
                                                activity.ID,
                                                priority
                                            )
                                        );
                                }

                                if (options[dorm.Dorm].Count == 0)
                                    failedDorms.Add(dorm.Dorm);
                                else
                                    options[dorm.Dorm].Sort(new DormActivityOptionComparer(activities.DormPriorities));
                            }

                            if (failedDorms.Count > 0)
                            {
                                LogManager.Enqueue(
                                    "CampSchedules",
                                    EntryType.ERROR,
                                    "DOUBLE REPEAT FAILED",
                                    failedDorms.ToArrayString() + " failed and " + dorms.Except(failedDorms).ToArrayString() + " succeeded"
                                );
                            }
                            else
                            {
                                CountOptions(false);

                                LogManager.Enqueue(
                                    "CampSchedules",
                                    EntryType.DEBUG,
                                    "DOUBLE REPEAT SUCCEEDED",
                                    dorms.ToArrayString() + " got " + dormsWithBlocks.Select(d => d.Value.OptionsLeft).ToArrayString() + " new options"
                                );
                            }
                        }
                    }*/
                    
                    if (dormsWithOptions.Count == 0)
                    {
                        LogManager.Enqueue(
                            "CampSchedules",
                            EntryType.ERROR,
                            "ScheduleActivities failed for startingBlockID " + startingBlockID,
                            dormsWithBlocks.Keys.ToArrayString()
                        );
                        scheduled.Reverse();
                        foreach (var actvity in scheduled)
                        {
                            ClearFromHistory(actvity);
                        }
                        //FinishHistoryClear();
                        return null;
                    }
                    /*var dormsInfo = dormsWithBlocks.Select(
                        d =>
                        {
                            var activities = DormActivities[d.Key];
                            return new KeyValuePair<int, Tuple<DormActivities, DormScheduleTracking, ScheduledActivity[], SortedDictionary<int, SortedSet<int>>>>(
                                d.Key, new Tuple<DormActivities, DormScheduleTracking, ScheduledActivity[], SortedDictionary<int, SortedSet<int>>>(
                                    activities,
                                    d.Value,
                                    scheduled.FindAll(
                                        s => activities.AvailableActivitiesToday.Contains(s.Activity)
                                    ).ToArray(),
                                    new SortedDictionary<int, SortedSet<int>>(
                                        activities.AvailableActivities.Intersect(availableActivities).Select(
                                            a => new KeyValuePair<int, SortedSet<int>>(a, activitiesRemaining[a])
                                        ).ToDictionary()
                                    )
                                )
                            );
                        }
                    ).ToDictionary();
                    Debug.Assert(false);*/
                    //return null;
                }
                #endregion

                maxRemainingBlocks = 0;

                bool alreadyLogged = false;
                bool alreadyLoggedCritical = false;
                while (notAssigned.Count > 0)
                {
                    #region Choosing_Option
                    string scoresString = "";

                    DormActivityOption chosen = null;
                    int predictedBlock = blockIter + startingBlockID;
                    if (notAssigned.Any(d => options[d].Count == 1) || (RANDOMNESS_ENABLED && GEN.NextDouble() < CHANCE_CRITICAL))
                    {
                        var critical = new SortedSet<int>(
                            RANDOMNESS_ENABLED ? 
                                notAssigned.Where(
                                    d => options[d].Count <= 1 || 
                                    GEN.NextDouble() < CHANCE_CRITICAL
                                ).Select(d => d) :
                                notAssigned.Where(
                                    d => options[d].Count <= 1
                                ).Select(d => d)
                            );
                        if (critical.Count == 0)
                            continue;
                        List<DormActivityOption> criticalOptions = new List<DormActivityOption>();
                        foreach(var dorm in dormsWithOptions)
                        {
                            if (critical.Contains(dorm))
                                criticalOptions.AddRange(options[dorm]);
                            else
                            {
                                var activities = DormActivities[dorm];
                                var common = new SortedSet<int>(critical.Intersect(activities.DormPriorities.Keys).Where(d => activities.DormPriorities[d].IncludeInResolving()));
                                if (common.Count > 0)
                                    criticalOptions.AddRange(options[dorm].FindAll(o => o.HasOther && common.Contains(o.OtherDorm)));
                            }
                        }

                        if (criticalOptions.Count == 1)
                            chosen = criticalOptions.First();
                        else if (criticalOptions.Count == 0)
                            continue;
                        else
                        {
                            var ties = criticalOptions.OrderByDescending(
                                d =>
                                {
                                    int score = 0;
                                    if (critical.Contains(d.Dorm))
                                        ++score;
                                    if (d.HasOther && critical.Contains(d.OtherDorm))
                                    {
                                        ++score;
                                        if (options[d.OtherDorm].Count == 0)
                                            ++score;
                                    }
                                    return score;
                                }
                            );
                            if (!alreadyLoggedCritical)
                            {
                                LogManager.Enqueue(
                                    new LogUpdate(
                                        "CampSchedules",
                                        EntryType.DEBUG,
                                        new object[]
                                        {
                                            "Dorms are critical; block " + predictedBlock,
                                            critical.ToArrayString(),
                                            ties.ToArrayString()
                                        }
                                    )
                                );
                                alreadyLoggedCritical = true;
                            }
                            chosen = (!RANDOMNESS_ENABLED || GEN.NextDouble() < CHANCE_FIRST_ELEMENT) ?
                                ties.First() : 
                                ties.ElementAt((int)(GEN.NextDouble() * GEN.NextDouble() * ties.Count()));
                        }
                    }
                    else if (dormsWithOptions.Count > 0)
                    {
                        var scores = new SortedSet<int>();
                        // Get the top priority out of all top priorities
                        // Make sure only to get options where the other dorm isn't already busy 
                        // Check all available blocks for that activity to see if the other dorm has any free
                        var grabCount = ((int)Math.Sqrt(dormsWithOptions.Count * 3)) - 1;
                        if (RANDOMNESS_ENABLED)
                            grabCount += GEN.Next(-2, 3);
                        grabCount = Math.Min(Math.Max(grabCount, 3), dormsWithOptions.Count);
                        int total = 0;
                        int firstScore = 0;
                        var ties = dormsWithOptions.Select(
                            d => options[d].Find(
                                opt => !opt.HasOther || (
                                        dormsWithBlocks.ContainsKey(opt.OtherDorm) &&
                                        notAssigned.Contains(opt.OtherDorm)
                                    )

                                /*(availableActivities.Contains(opt.Activity) || opt.HasExcess) && (
                                            !opt.HasOther || (
                                                dormsWithBlocks.ContainsKey(opt.OtherDorm) &&
                                                notAssigned.Contains(opt.OtherDorm)
                                            )
                                        )*/
                                )
                        ).Where(o => o != null).GroupBy(
                            t =>
                            {
                                var score = dormsWithBlocks[t.Dorm].Score;
                                if (t.HasOther)
                                    score = (score + dormsWithBlocks[t.OtherDorm].Score) / 2;
                                if (!DormActivities[t.Dorm].OtherDormsDoneToday.Contains(t.OtherDorm))
                                    ++score;
                                //score = Math.Min(score, dormsWithBlocks[t.OtherDorm].Score);
                                if (RANDOMNESS_ENABLED)
                                {
                                    while (GEN.NextDouble() < CHANCE_INCREASE_SCORE)
                                        score += GEN.Next(1, 3);
                                }
                                var finalScore = (int)(score / t.TotalPriority);
                                scores.Add(finalScore);
                                return finalScore;
                            }
                        ).TakeWhile(
                            (g, i) =>
                            {
                                bool result = false;
                                int scoreGroupCount = g.Count();
                                if (i == 0)
                                {
                                    result = true;
                                    firstScore = g.Key;
                                }
                                else if (total + scoreGroupCount <= grabCount && g.Key - firstScore < 4)
                                    result = true;

                                if (result || (RANDOMNESS_ENABLED && GEN.NextDouble() < CHANCE_CRITICAL))
                                {
                                    total += scoreGroupCount;
                                    return true;
                                }
                                return false;
                            }
                        ).SelectMany(g => g).OrderBy(o => o, new DormActivityOptionComparer(d => DormActivities[d])).ToList();
                        if (!alreadyLogged)
                        {
                            LogManager.Enqueue(
                                new LogUpdate(
                                    "CampSchedules",
                                    EntryType.DEBUG,
                                    new object[]
                                    {
                                        "Top options for block " + predictedBlock,
                                        ties.ToArrayString()
                                    }
                                )
                            );
                            alreadyLogged = true;
                        }
                        chosen = !RANDOMNESS_ENABLED || GEN.NextDouble() < CHANCE_FIRST_ELEMENT ?
                                ties.First() :
                                ties.ElementAt((int)(GEN.NextDouble() * GEN.NextDouble() * ties.Count()));
                        if (chosen.HasOther)
                            scoresString = String.Format(" - {0}, {1}, {2}", dormsWithBlocks[chosen.Dorm].Score, dormsWithBlocks[chosen.OtherDorm].Score, chosen.TotalPriority);
                    }
                    else
                    {
                        Debug.Assert(dormsWithOptions.All(d => dormsWithBlocks[d].OptionsLeft > 0));
                        dormsWithOptions.Clear();
                        break;
                    }
                    #endregion

                    #region Choosing_Block
                    SortedSet<int> potentialConflicts = new SortedSet<int>();
                    var activityInfo = Activities[chosen.Activity];
                    var dormActivities = DormActivities[chosen.Dorm];
                    var tracker = dormsWithBlocks[chosen.Dorm];
                    var interDormTracker = dormActivities.DormPriorities[chosen.OtherDorm == -1 ? chosen.Dorm : chosen.OtherDorm];

                    // If the activity wasn't previously scheduled, then the first block might be one that is unavailable to one of the dorms
                    var dormConflicts = new SortedSet<int>(tracker.ReservedBlocks);
                    if (tracker.Conflicts.TryGetValue(chosen.Activity, out SortedSet<int> dormConflictsTemp))
                        dormConflicts.UnionWith(dormConflictsTemp);
                    potentialConflicts.UnionWith(dormConflicts);

                    DormActivities otherDormActivities = null;
                    DormScheduleTracking otherDormTracker = null;
                    SortedSet<int> otherDormConflicts = null;
                    if (chosen.HasOther)
                    {
                        otherDormActivities = DormActivities[chosen.OtherDorm];
                        otherDormTracker = dormsWithBlocks[chosen.OtherDorm];
                        otherDormConflicts = new SortedSet<int>(otherDormTracker.ReservedBlocks);
                        if (otherDormTracker.Conflicts.TryGetValue(chosen.Activity, out SortedSet<int> otherDormConflictsTemp))
                            otherDormConflicts.UnionWith(otherDormConflictsTemp);
                        potentialConflicts.UnionWith(otherDormConflicts);
                        otherDormConflicts.SymmetricExceptWith(dormConflicts);
                    }

                    var activityBlocks = activitiesRemaining[chosen.Activity];
                    int block = predictedBlock;

                    if (potentialConflicts.Count > 0)
                    {
                        var usableActivityBlocks = activityBlocks.Except(potentialConflicts);
                        block = !RANDOMNESS_ENABLED || usableActivityBlocks.Count() == 1 || GEN.NextDouble() < (CHANCE_FIRST_ELEMENT / 2) ?
                            usableActivityBlocks.First() :
                            usableActivityBlocks.ElementAt(GEN.Next(usableActivityBlocks.Count()));
                        /*if (!usableActivityBlocks.Contains(predictedBlock))
                            block = usableActivityBlocks.First();*/

                        if (chosen.HasOther && otherDormConflicts.Count > 1)
                            LogManager.Enqueue(
                                new LogUpdate(
                                    "CampSchedules", EntryType.DEBUG, new object[]
                                    {
                                        "Chosen dorms have potential conflicts",
                                        "Predicted/Chosen Block: " + predictedBlock.ToString() + "/" + block.ToString(),
                                        activityInfo.ToString() + " Open Blocks: \t[" + String.Join(", ", activityBlocks) + "]",
                                        Dorms[chosen.Dorm].ToString() + " Reserved Blocks: \t[" + String.Join(", ", dormConflicts.OrderBy()) + "]",
                                        Dorms[chosen.OtherDorm].ToString() + " Reserved Blocks: \t[" + String.Join(", ", otherDormConflicts.OrderBy()) + "]"
                                    }
                                )
                            );
                    }
                    else if (!activityBlocks.Contains(predictedBlock))
                        block = !RANDOMNESS_ENABLED || activityBlocks.Count() == 1 || GEN.NextDouble() < (CHANCE_FIRST_ELEMENT / 2) ?
                            activityBlocks.Min :
                            activityBlocks.ElementAt(GEN.Next(activityBlocks.Count));
                    else if (RANDOMNESS_ENABLED && GEN.NextDouble() > CHANCE_FIRST_ELEMENT)
                        block = activityBlocks.ElementAt(GEN.Next(activityBlocks.Count));

                    options[chosen.Dorm].Remove(chosen);
                    //notAssigned.Remove(chosen.Dorm);

                    IEnumerable<int> reservedBlocks = Enumerable.Range(block, chosen.Duration);

                    ScheduledActivity scheduledActivity = new ScheduledActivity(
                        chosen.Dorm,
                        chosen.Duration,
                        chosen.Activity,
                        block,
                        chosen.HasOther ? chosen.OtherDorm : -1
                    );
                    scheduledActivity.StringExtension = scoresString;
                    #endregion

                    bool clearDorm = !multiBlock;
                    bool clearOtherDorm = !multiBlock && chosen.HasOther;
                    if (multiBlock)
                    {
                        if (chosen.Duration >= tracker.RemainingBlocks)
                            clearDorm = true;
                        else
                        {
                            tracker.RemainingBlocks -= chosen.Duration;
                            if (maxRemainingBlocks < tracker.RemainingBlocks)
                                maxRemainingBlocks = tracker.RemainingBlocks;
                        }

                        if (chosen.HasOther)
                        {
                            if (chosen.Duration >= otherDormTracker.RemainingBlocks)
                                clearOtherDorm = true;
                            else
                            {
                                otherDormTracker.RemainingBlocks -= chosen.Duration;
                                if (maxRemainingBlocks < otherDormTracker.RemainingBlocks)
                                    maxRemainingBlocks = otherDormTracker.RemainingBlocks;
                            }
                        }
                    }

                    if (block == predictedBlock)
                    {
                        if (!clearDorm)
                        {
                            dormsWithOptions.Remove(chosen.Dorm);
                            notAssigned.Remove(chosen.Dorm);
                        }
                        if (!clearOtherDorm && chosen.HasOther)
                        {
                            dormsWithOptions.Remove(chosen.OtherDorm);
                            notAssigned.Remove(chosen.OtherDorm);
                        }
                    }

                    scheduled.Add(scheduledActivity);

                    if (tracker.ReservedBlocks.Count > 0)
                        Debug.Assert( // we are asserting that it DOESN'T overlap
                            !tracker.ReservedBlocks.Overlaps(reservedBlocks),
                            "Previously reserved blocks overlap with newly reserved ones.",
                            FORMAT_OVERLAP_ERROR,
                            chosen.Dorm,
                            tracker.ReservedBlocks.ToArrayString(),
                            reservedBlocks.ToArrayString(),
                            startingBlockID, predictedBlock,
                            scheduledActivity
                        );
                    tracker.ReservedBlocks.UnionWith(reservedBlocks);

                    if (chosen.HasOther)
                    {
                        if (otherDormTracker.ReservedBlocks.Count > 0)
                            Debug.Assert(
                                !otherDormTracker.ReservedBlocks.Overlaps(reservedBlocks),
                                "Previously reserved blocks overlap with newly reserved ones.",
                                FORMAT_OVERLAP_ERROR,
                                chosen.Dorm,
                                otherDormTracker.ReservedBlocks.ToArrayString(),
                                reservedBlocks.ToArrayString(),
                                startingBlockID, predictedBlock,
                                scheduledActivity
                            );
                        otherDormTracker.ReservedBlocks.UnionWith(reservedBlocks);

                        ScheduleActivity(
                            scheduledActivity,
                            activityInfo,
                            dormActivities,
                            otherDormActivities
                        );

                        options[chosen.Dorm].RemoveAll(o => o.HasOther && o.OtherDorm == chosen.OtherDorm);
                    }
                    else
                        ScheduleActivity(
                            scheduledActivity,
                            activityInfo,
                            DormActivities[chosen.Dorm]
                        );

                    /*var exhausting = activityInfo.Flags.HasFlag(ActivityFlags.Exhausting);
                    var relaxing = activityInfo.Flags.HasFlag(ActivityFlags.Relaxing) && Dorms[chosen.Dorm].AgeGroup > 3;
                    if (exhausting)
                        dormActivities.AvailableActivitiesToday.ExceptWith(ActivityInfo.HighFatigueActivities);
                    if (relaxing)
                        dormActivities.AvailableActivitiesToday.ExceptWith(ActivityInfo.LowFatigueActivities);*/

                    if (activityInfo.IncompatibleActivities.Any())
                    {
                        foreach(var incompat in activityInfo.IncompatibleActivities)
                        {
                            activitiesRemaining[incompat].Remove(block);
                            /*if (activitiesRemaining[incompat].Any())
                                newConflicts.Add(new ConflictNode(-1, incompat, true));
                            else
                                newConflicts.Add(new ConflictNode(-1, incompat, false));*/
                        }
                    }

                    if (clearDorm)
                    {
                        newConflicts.Add(new ConflictNode(chosen.Dorm, -1));
                        options[chosen.Dorm].Clear();
                        tracker.RemainingBlocks = 0;
                        dormsWithBlocks.Remove(chosen.Dorm);
                        dormsWithOptions.Remove(chosen.Dorm);
                        notAssigned.Remove(chosen.Dorm);
                    }
                    else
                    {
                        bool isEmpty = !CheckAddUnavailableActivities(chosen.Dorm, dormActivities, reservedBlocks, out PresetDormConflicts presetConflicts);
                        if (isEmpty || !presetConflicts.UnavailableActivities.Contains(activityInfo.ID))
                        {
                            presetConflicts.UnavailableActivities.Add(activityInfo.ID);
                            if (activityInfo.MaxDorms > 1)
                                newConflicts.Add(new ConflictNode(chosen.Dorm, activityInfo.ID));
                        }
                        presetConflicts.UnavailableActivities.UnionWith(activityInfo.IncompatibleActivities);

                        if (chosen.HasOther && !clearOtherDorm)
                            presetConflicts.OtherDorm = chosen.OtherDorm;
                        /*if (exhausting)
                            presetConflicts.UnavailableActivities.UnionWith(ActivityInfo.HighFatigueActivities);
                        if (relaxing)
                            presetConflicts.UnavailableActivities.UnionWith(ActivityInfo.LowFatigueActivities);*/
                        
                        presetDelegates.Add(chosen.Dorm, presetConflicts);
                    }

                    if (chosen.HasOther)
                    {
                        /*var otherRelaxing = activityInfo.Flags.HasFlag(ActivityFlags.Relaxing) && Dorms[chosen.OtherDorm].AgeGroup > 3;
                        if (exhausting)
                            otherDormActivities.AvailableActivitiesToday.ExceptWith(ActivityInfo.HighFatigueActivities);
                        if (otherRelaxing)
                            otherDormActivities.AvailableActivitiesToday.ExceptWith(ActivityInfo.LowFatigueActivities);*/
                        
                        if (clearOtherDorm)
                        {
                            newConflicts.Add(new ConflictNode(chosen.OtherDorm, -1));
                            options[chosen.OtherDorm].Clear();
                            otherDormTracker.RemainingBlocks = 0;
                            dormsWithBlocks.Remove(chosen.OtherDorm);
                            dormsWithOptions.Remove(chosen.OtherDorm);
                            notAssigned.Remove(chosen.OtherDorm);
                        }
                        else
                        {
                            bool isEmpty = !CheckAddUnavailableActivities(chosen.OtherDorm, otherDormActivities, reservedBlocks, out PresetDormConflicts otherPresetConflicts);
                            if (isEmpty || !otherPresetConflicts.UnavailableActivities.Contains(activityInfo.ID))
                            {
                                otherPresetConflicts.UnavailableActivities.Add(activityInfo.ID);
                                // if there's an otherdorm, then the activity has to be multidorm
                                newConflicts.Add(new ConflictNode(chosen.OtherDorm, activityInfo.ID));
                            }

                            otherPresetConflicts.UnavailableActivities.UnionWith(activityInfo.IncompatibleActivities);

                            /*if (exhausting)
                                otherPresetConflicts.UnavailableActivities.UnionWith(ActivityInfo.HighFatigueActivities);
                            if (otherRelaxing)
                                otherPresetConflicts.UnavailableActivities.UnionWith(ActivityInfo.LowFatigueActivities);*/

                            presetDelegates.Add(chosen.OtherDorm, otherPresetConflicts);
                        }
                    }
                    
                    if (activityInfo.MaxConcurrent == -1)
                        ResolveConflicts(-1, block);
                    else if (activityInfo.Flags.HasFlag(ActivityFlags.Concurrent))
                    {
                        var concurrencyInfo = new CSVKeyValuePair<int, int>(block, chosen.Activity);
                        var concurrentCount = previousConcurrentBlocks.Count(c => c.Equals(concurrencyInfo));
                        previousConcurrentBlocks.Add(concurrencyInfo);
                        if (concurrentCount == activityInfo.MaxConcurrent - 1)
                        {
                            activityBlocks.Remove(block);
                            ResolveConflicts(chosen.Activity, activityBlocks.Count == 0 ? -1 : block);
                        }
                        else
                            ResolveConflicts(-1, block);
                    }
                    else
                    {
                        activityBlocks.Remove(block);
                        ResolveConflicts(chosen.Activity, activityBlocks.Count == 0 ? -1 : block);
                    }
                }
                ++blockIter;
            }
            LastSuccessfulHistory = new SortedSet<string>(scheduled.Select(s => s.Abbreviation));
            return scheduled.ToArray();
        }

        public ScheduledActivity[] ScheduleBlocks(Block[] blocks, bool excessPreceeding)
        {
            var first = blocks.First();
            int blocksAvailable = 0;
            int excessBlocksAvailable = 0;
            Dictionary<int, SortedSet<int>> dormsReservedBlocks = new Dictionary<int, SortedSet<int>>();
            List<ScheduledActivity> scheduled = new List<ScheduledActivity>();
            SortedSet<int> allBlocks = new SortedSet<int>();
            foreach(var block in blocks)
            {
                allBlocks.Add(block.ID);
                if (block.IsExcess)
                    ++excessBlocksAvailable;
                else
                {
                    ++blocksAvailable;
                    /*foreach (var activity in block.ScheduleHistory)
                    {
                        var scheduledActivity = this[activity];
                        if (dormsReservedBlocks.ContainsKey(scheduledActivity.Dorm))
                            dormsReservedBlocks[scheduledActivity.Dorm].Add(block.ID);
                        else
                            dormsReservedBlocks.Add(scheduledActivity.Dorm, new SortedSet<int>() { block.ID });
                        scheduled.Add(scheduledActivity);
                    }*/
                }
            }

            foreach(var reservedBlocks in ManuallyScheduledBlocks)
            {
                if (reservedBlocks.Value.Overlaps(allBlocks))
                    dormsReservedBlocks.Add(reservedBlocks.Key, new SortedSet<int>(reservedBlocks.Value.Intersect(allBlocks)));
            }

            bool extendedDurationAvailable = (excessBlocksAvailable > 0 && !excessPreceeding) || (excessPreceeding && blocksAvailable > 2);
            //Console.WriteLine("\t" + first.Abbreviation + "_" + blocksAvailable + (excessBlocksAvailable > 0 ? " (" + excessBlocksAvailable + ")" : ""));

            SortedSet<int> availableActivities = null;
            // Get all activities that can actually fit within the allotted time slot
            if (excessBlocksAvailable >= 2)
            {
                availableActivities =
                    new SortedSet<int>(
                        (
                            blocksAvailable == 0 ?
                                Activities.Where(
                                    a => a.Flags.HasFlag(ActivityFlags.Excess)
                                ) : Activities.Where(
                                    a => a.Duration <= blocksAvailable || a.Flags.HasFlag(ActivityFlags.Excess)
                                )
                        ).Select(a => a.ID)
                    );
            }
            else
                availableActivities = new SortedSet<int>(
                    Activities.Where(
                        a => a.Duration <= blocksAvailable
                    ).Select(a => a.ID)
                );
            
            var options = GetOptions(availableActivities, blocksAvailable, extendedDurationAvailable);

            DormActivityOption[][] optionsCopy = new DormActivityOption[options.Length][];
            for (int i = 0; i < options.Length; ++i)
            {
                optionsCopy[i] = new DormActivityOption[options[i].Count];
                options[i].CopyTo(optionsCopy[i]);
            }
            int[] activitiesCopy = new int[availableActivities.Count];
            availableActivities.CopyTo(activitiesCopy);

            /*var dormActivitiesCopy = new DormActivities[DormActivities.Length];
            for (int i = 0; i < DormActivities.Length; ++i)
            {
                dormActivitiesCopy[i] = (DormActivities)DormActivities[i].Clone();
            }*/           

            ScheduledActivity[] otherScheduled = null;
            bool infiniteTries = first.ID == 0;
            byte numTries = 0;
            do
            {
                otherScheduled = ScheduleActivities(
                    availableActivities,
                    options,
                    first.ID,
                    blocksAvailable,
                    dormsReservedBlocks.Count == 0 ?
                        null : dormsReservedBlocks,
                    extendedDurationAvailable
                );
                if (otherScheduled == null)
                {
                    /*foreach (var block in blocks)
                    {
                        var history = block.ScheduleHistory;
                        foreach (var abbrv in history)
                        {
                            ClearFromHistory(abbrv);
                        }
                    }*/

                    for (int i = 0; i < NumDorms; ++i)
                    {
                        options[i] = new List<DormActivityOption>(optionsCopy[i]);
                    }

                    /*for (int i = 0; i < DormActivities.Length; ++i)
                    {
                        DormActivities[i] = (DormActivities)dormActivitiesCopy[i].Clone();
                    }*/
                    availableActivities.UnionWith(activitiesCopy);
                    RANDOMNESS_ENABLED = true;
                }
                else
                {
                    RANDOMNESS_ENABLED = false;
                    break;
                }
                ++numTries;
            } while (numTries <= NUM_TRIES_BLOCKS || infiniteTries);

            if (otherScheduled == null)
                return null;
            else
            {
                //DormActivitiesHistory.Push(dormActivitiesCopy);
                //Console.Write("\tPUSH BLOCKS: " + DormActivitiesHistory.Count);
                scheduled.AddRange(otherScheduled);
                if (numTries > 1)
                {
                    Console.WriteLine(": {0} tries", numTries);
                    LogManager.Enqueue(
                        "CampSchedules",
                        EntryType.DEBUG,
                        "BLOCKS TRY_AGAIN SUCCEEDED",
                        first.Abbreviation + "_" + blocksAvailable + (excessBlocksAvailable > 0 ? " (" + excessBlocksAvailable + ")" : "") + ", " + numTries.ToString() + " tries"
                    );
                }
                else
                    Console.WriteLine();
            }
            return scheduled.ToArray();
        }

        public ScheduledActivity[][] ScheduleDay(DayOfWeek day, string filePath, ref int backtrackIndex, bool backtracking = false)
        {
            //Console.WriteLine(day);
            foreach (var dormActivities in DormActivities)
            {
                dormActivities.NewDay();
            }

            int firstBacktrackIndex = backtrackIndex;

            var dayInfo = this[(int)day];
            var blocks = dayInfo.Blocks;
            int lastPointer = dayInfo.Pointer;
            List<ScheduledActivity[]> scheduled = new List<ScheduledActivity[]>();
            if (backtracking)
            {
                dayInfo.Pointer = dayInfo.Backtracking[backtrackIndex];
                RANDOMNESS_ENABLED = true;
            }
            else
            {
                dayInfo.Pointer = 0;
                RANDOMNESS_ENABLED = false;
            }

            LogManager.Enqueue(
                "CampSchedules",
                EntryType.DEBUG,
                "Scheduling " + day,
                "backtrackIndex " + backtrackIndex + ", lastPointer + " + lastPointer + ", currentPointer" + dayInfo.Pointer
            );

            /*var manual = new SortedSet<string>(blocks.SelectMany(b => Blocks[b].ScheduleHistory));
            foreach (var abbrv in manual)
            {
                var activity = this[abbrv];
                if (Activities[activity.Activity].Flags.HasFlag(ActivityFlags.Exhausting))
                    DormActivities[activity.Dorm].AvailableActivitiesToday.ExceptWith(ActivityInfo.HighFatigueActivities);
                if (Activities[activity.Activity].Flags.HasFlag(ActivityFlags.Relaxing) && Dorms[activity.Dorm].AgeGroup > 3)
                    DormActivities[activity.Dorm].AvailableActivitiesToday.ExceptWith(ActivityInfo.LowFatigueActivities);
            }*/

            var firstPointer = dayInfo.Pointer;

            int numSuccesses = 0;
            var dormActivitiesCopy = new DormActivities[DormActivities.Length];
            for (int i = 0; i < DormActivities.Length; ++i)
            {
                dormActivitiesCopy[i] = (DormActivities)DormActivities[i].Clone();
            }

            int numTries = 0;

            while (dayInfo.Pointer < blocks.Count)
            {
                lastPointer = dayInfo.Pointer;

                List<Block> currentBlocks = new List<Block>();
                Block nextBlock = Blocks[blocks[dayInfo.Pointer]];
                do
                {
                    currentBlocks.Add(nextBlock);
                    ++dayInfo.Pointer;
                    if (dayInfo.Pointer != blocks.Count)
                        nextBlock = Blocks[blocks[dayInfo.Pointer]];
                } while (dayInfo.Pointer < blocks.Count && nextBlock.Start.Hours - currentBlocks.Last().Start.Hours <= 1);

                if (backtracking)
                {
                    if (lastPointer != firstPointer && (/*backtrackedPrevDay || */dayInfo.FullyBacktracked || dayInfo.Backtracking.Count > backtrackIndex))
                        Console.Write("\t>{0}<", lastPointer);
                    else
                        Console.Write("\t>{0}", lastPointer);
                }
                else
                    Console.Write("\t{0}", lastPointer);

                if (!dayInfo.FullyBacktracked && dayInfo.Backtracking.Count == backtrackIndex)
                    dayInfo.Backtracking.Add(lastPointer);

                var currentScheduled = ScheduleBlocks(currentBlocks.ToArray(), dayInfo.ExcessPreceeding);
                if (currentScheduled == null)
                {
                    ++numTries;
                    if (!backtracking)
                        ScheduledActivities.SaveAs(@"E:\Work Programming\Higher Ground Program Files\Schedule-" + day.ToString() + "-" + dayInfo.Pointer.ToString() + ".json");

                    //ClearBlocksFromHistory(day, dayInfo.Pointer, pointerTemp);
                    scheduled.Reverse();
                    foreach (var blockSection in scheduled)
                    {
                        foreach (var scheduledActivity in blockSection.Reverse())
                        {
                            ClearFromHistory(scheduledActivity);
                        }
                    }
                    scheduled.Clear();

                    if (numTries < NUM_TRIES_DAYS && backtrackIndex > firstBacktrackIndex)
                    {
                        foreach (var dormActivities in DormActivities)
                        {
                            dormActivities.NewDay();
                        }

                        backtrackIndex = 0;
                        var pointerTemp = dayInfo.Pointer;
                        dayInfo.Pointer = dayInfo.Backtracking[backtrackIndex];

                        backtracking = true;
                        RANDOMNESS_ENABLED = true;
                    }
                    else
                    {
                        Console.WriteLine(": FAILED");
                        if (numTries > 1)
                        {
                            Console.WriteLine("  : {0} tries", numTries);
                            LogManager.Enqueue(
                                "CampSchedules",
                                EntryType.DEBUG,
                                "DAYS TRY_AGAIN FAILED",
                                String.Format("{0} - {1}", day, lastPointer)
                            );
                        }
                        return null;
                    }

                    /*foreach (var scheduledActivity in scheduled.SelectMany(b => b))
                    {
                        ClearFromHistory(scheduledActivity);
                    }*/
                    /*for (int i = 0; i < DormActivities.Length; ++i)
                    {
                        DormActivities[i] = (DormActivities)dormActivitiesCopy[i].Clone();
                    }*/

                    /*if (firstPointer == 0)
                    {
                        for (int n = numSuccesses; n > 0; --n)
                        {
                            DormActivitiesHistory.Pop();
                        }
                        Console.Write("\tPOP DAY ({0} successes): " + DormActivitiesHistory.Count, numSuccesses);
                    }*/
                    //FinishHistoryClear();
                }
                else
                {
                    ++numSuccesses;
                    ++backtrackIndex;
                    scheduled.Add(currentScheduled);
                }
            }

            if (numTries > 1)
            {
                LogManager.Enqueue(
                    "CampSchedules",
                    EntryType.DEBUG,
                    "DAYS TRY_AGAIN SUCCEEDED",
                    String.Format("{0} - {1}, {2} tries", day, lastPointer, numTries)
                );
            }

            /*for (int i = 0; i < DormActivities.Length; ++i)
            {
                dormActivitiesCopy[i] = (DormActivities)DormActivities[i].Clone();
            }
            DormActivitiesHistory.Push(dormActivitiesCopy);
            Console.WriteLine("\tPUSH DAY: " + DormActivitiesHistory.Count);*/

            dayInfo.FullyBacktracked = true;
            
            return scheduled.ToArray();
        }

        #region ScheduleActivity
        private void ScheduleActivity(ScheduledActivity scheduled, ActivityInfo activity, params DormActivities[] activities)
        {
            for (int i = 0; i < activities.Length; ++i)
            {
                activities[i].ScheduleActivity(activity, scheduled);
            }

            if (ScheduledActivityAbbrvs.TryAdd(scheduled.Abbreviation, scheduled.ID))
                ScheduledActivities.Add(scheduled);
            else
                scheduled.DecrementID(ScheduledActivityAbbrvs[scheduled.Abbreviation]);
            int maxBlock = scheduled.BlockID + scheduled.Duration;
            for (int i = scheduled.BlockID; i < maxBlock; ++i)
            {
                Blocks[i].ScheduleHistory.Add(scheduled.Abbreviation);
            }
            //Console.WriteLine("\t\t" + scheduled.Abbreviation + scheduled.StringExtension);
            ActivitiesScheduleHistories[activity.ID].Add(scheduled.Abbreviation);
        }



        /*public void ScheduleActivity(int dormID, DormActivities activities, int block, ActivityInfo activity)
        {
            var scheduled = new ScheduledActivity(dormID, block, activity.ID);
            ScheduleActivity(scheduled, activity, activities);
        }

        public void ScheduleActivity(int dormID, DormActivities activities, int block, ActivityInfo activity, int otherID, DormActivities otherActivities)
        {
            var scheduled = new ScheduledActivity(dormID, block, activity.ID, otherID);
            ScheduleActivity(scheduled, activity, activities, otherActivities);
        }*/
        #endregion

        #region IReadOnlyList<Day> Implementation
        public Day this[int index] => Days[index];

        public int Count => Days.Length;

        private Day[] Days { get; set; }

        public IEnumerator<Day> GetEnumerator()
        {
            return ((IReadOnlyList<Day>)Days).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Days.GetEnumerator();
        }
        #endregion
    }
}
