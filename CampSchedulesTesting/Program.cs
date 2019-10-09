using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CampSchedules_v3;
using CampSchedules_v3.SimpleGeneticAlgorithm;
using CampSchedulesLib_v2;
using CampSchedulesLib_v2.Models;
using CampSchedulesLib_v2.Models.Info;
using CampSchedulesLib_v2.Models.CSV;
using CampSchedulesLib_v2.Models.Scheduling;
using CsvHelper;
using CsvHelper.Configuration;
using ZachLib;
using ZachLib.Statistics;

namespace CampSchedulesTesting
{
    class Program
    {
        private const string PATH = @"E:\Work Programming\Higher Ground Program Files\";
        private const string PRIORITIES_PATH = @"C:\Users\ZACH-GAMING\Documents\Higher Ground\Priorities\";

        static void Main(string[] args)
        {
            //Console.WriteLine("Chromosome length: {0}", SimpleGenePool.CreateInitialPool(PATH));
            //Console.WriteLine("Gene length: {0}", SimpleGenePool.FreezeGenePool());
            /*SimpleGenePool.Evolve(PATH);

            Console.WriteLine("FINISHED");
            Console.ReadLine();*/
            
            var activities = Utils.LoadCSV<ActivityCSV>(
                PATH + "Activities.csv"
            );
            foreach(var activity in activities)
            {
                Schedule.Activities.Add(activity.ToInfo());
            }

            Schedule.Dorms.AddRange(
                File.ReadAllLines(
                    PATH + "ExclusiveActivities.txt"
                ).Select(
                    l =>
                    {
                        var split = l.Split(
                            new string[] { " - ", ", " },
                            StringSplitOptions.None
                        );
                        return new DormInfo(
                            split[0],
                            split.Skip(1).Select(
                                a => Schedule.Activities.First(a2 => a2.Abbreviation == a).ID
                            ).ToArray()
                        );
                    }
                )
            );

            //GetPriorities();

            Schedule schedule = new Schedule(PATH);
            //schedule.AddBlocks(Utils.LoadCSV<BlockCSV>(PATH + "Days.csv"));
            schedule.Create(PATH);
            schedule.Reports(PATH + "Reports");
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        public static void GeneticAlgorithm()
        {
            double[][][] chances = new double[3][][]
            {
                new double[3][] {
                    new double[] { 1, 0, 0 },
                    new double[] { 0.5, 0.5, 0 },
                    new double[] { 0, 1, 0 }
                },
                new double[3][]
                {
                    null,
                    new double[] { 0.25, 0.5, 0.25 },
                    new double[] { 0, 0.5, 0.5 }
                },
                new double[3][]
                {
                    null,
                    null,
                    new double[] { 0, 0, 1 }
                }
            };
            double[] population = new double[] { 1.0 / 3, 5.0 / 9, 1.0 / 9 };
            int writePoint = 1;
            int endPoint = 4;
            int generation = 0;
            for (int i = 1; i <= 4; ++i)
            {
                for (int g = 1; g <= endPoint; ++g)
                {
                    if (g % writePoint == 0)
                        Console.WriteLine("Generation {0}: {1}", generation, population.ToArrayString("#.000"));
                    ++generation;

                    double[] newPop = new double[3];
                    double totalChances = 0;
                    for (int h = 0; h < 3; ++h)
                    {
                        var thisChances = chances[h];
                        for (int j = h; j < 3; ++j)
                        {
                            var relationshipChances = thisChances[j];
                            double chance = population[h] * population[j];
                            for (int t = 0; t < 3; ++t)
                            {
                                newPop[t] += relationshipChances[t] * chance;
                            }
                        }
                    }
                    double sum = newPop.Sum();
                    population = newPop.Select(chance => chance / sum).ToArray();
                }
                endPoint *= 4;
            }
            Console.WriteLine("Generation {0}: {1}", generation, population.ToArrayString("#.000"));
            Console.ReadLine();
        }

        public static void GetMissing()
        {
            List<string> allPossible = new List<string>();
            foreach(var activity in Schedule.Activities)
            {
                if (!activity.Flags.HasFlag(ActivityFlags.Manual) && !activity.Flags.HasFlag(ActivityFlags.Exclusive))
                {
                    allPossible.Add(activity.Abbreviation);
                    if (activity.Flags.HasFlag(ActivityFlags.Repeatable))
                        allPossible.Add(activity.Abbreviation);
                }
            }

            var dorms = Utils.LoadJSON<PriorityDorm[]>(PRIORITIES_PATH + "PriorityDorms.txt").ToDictionary(d => d.Name);

            foreach(var dorm in Schedule.Dorms)
            {
                List<string> allPossibleDorm = new List<string>(allPossible);
                allPossibleDorm.AddRange(dorm.AllowedExclusiveActivities.Select(a => Schedule.Activities[a].Abbreviation));
                List<string> missing = new List<string>();

                foreach(var activity in allPossibleDorm)
                {

                }
            }
        }

        public static void GetPriorities()
        {
            //CreatePrioritiesObjects();

            var dormsNumDays = new float[]
            {
                4,
                4,
                4,
                4,
                4,
                4,
                3.5f,   // 4B - RC
                4,
                3,      // 5B - CT
                3.5f,   // 5G - RC
                2.5f,   // 6B - CV
                3,      // 6G - CT
                4
            };
            var avgNumDays = dormsNumDays.Average();

            var days = Utils.LoadJSON<PriorityDay[]>(PRIORITIES_PATH + "PriorityDays.txt");
            var dorms = Utils.LoadJSON<PriorityDorm[]>(PRIORITIES_PATH + "PriorityDorms.txt").ToDictionary(d => d.Name);
            var activities = Utils.LoadJSON<PriorityActivityFull[]>(PRIORITIES_PATH + "PriorityActivities.txt");
            SortedDictionary<string, float> activityTotalPriorities = new SortedDictionary<string, float>();
            SortedDictionary<string, int> activityGirlPriorities = new SortedDictionary<string, int>();
            SortedDictionary<string, int> activityBoyPriorities = new SortedDictionary<string, int>();
            SortedDictionary<string, double> activityYoungPriorities = new SortedDictionary<string, double>();
            SortedDictionary<string, int> activityOldPriorities = new SortedDictionary<string, int>();

            List<KeyValuePair<string, int>>[] dormActivityPriorities = new List<KeyValuePair<string, int>>[dorms.Count];

            List<string> dormIndices = new List<string>();
            float dayIndexIncrement = days.Length / avgNumDays;

            int dormIndex = 0;
            foreach (var dorm in dorms.Values)
            {
                dormIndices.Add(dorm.Name);
                List<KeyValuePair<string, int>> priorities = new List<KeyValuePair<string, int>>();
                int activityPriority = dorm.Activities.Count;
                foreach (var activity in dorm.Activities)
                {
                    priorities.Add(new KeyValuePair<string, int>(activity, activityPriority));
                    --activityPriority;
                }
                dormActivityPriorities[dormIndex] = priorities;

                ++dormIndex;
            }
            
            foreach (var activity in activities)
            {
                var activityInfo = Schedule.Activities.First(a => a.Abbreviation == activity.Name);
                float totalPriority = 0;
                if (activity.DormPairs.Any())
                {
                    for (int i = 0; i < activity.Days.Length; ++i)
                    {
                        var day = activity.Days[i].Value;
                        int dayIndex = i + 1;
                        float dayPriorityTemp = 0;

                        foreach(var dorm in day)
                        {
                            dayPriorityTemp += dormsNumDays[dormIndices.IndexOf(dorm)] - (dayIndex * dayIndexIncrement);
                        }

                        dayPriorityTemp /= 2;
                        if (dayPriorityTemp % 1 == 0.5f)
                            dayPriorityTemp -= 0.5f;
                        totalPriority += dayPriorityTemp;
                    }
                }
                else
                {
                    for (int i = 0; i < activity.Days.Length; ++i)
                    {
                        var day = activity.Days[i].Value;
                        int dayIndex = i + 1;
                        float dayPriorityTemp = 0;

                        foreach (var dorm in day)
                        {
                            dayPriorityTemp += dormsNumDays[dormIndices.IndexOf(dorm)] - (dayIndex * dayIndexIncrement);
                        }
                        
                        totalPriority += dayPriorityTemp;
                    }
                }

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
                    totalPriority /= (float)Schedule.Dorms.Count(d => d.AllowedExclusiveActivities.Contains(activityInfo.ID)) / Schedule.Dorms.Count;

                Console.WriteLine(activity.Name + " - " + totalPriority.ToString("#.0"));

                activityTotalPriorities.Add(activity.Name, totalPriority);
                activityGirlPriorities.Add(activity.Name, 0);
                activityBoyPriorities.Add(activity.Name, 0);
                activityYoungPriorities.Add(activity.Name, 0);
                activityOldPriorities.Add(activity.Name, 0);
            }

            Console.WriteLine(activityTotalPriorities.OrderByDescending(a => a.Value).Select(a => a.Key).ToArrayString());

            /*int dayPriority = 4;
            foreach(var day in days)
            {
                foreach(var activity in day.Activities)
                {
                    List<string> remainingDorms = new List<string>(activity.Dorms);
                    bool dontBother = false;
                    if (activity.DormPairs.Any())
                    {
                        if (activity.DormPairs.Length * 2 == activity.Dorms.Length)
                        {
                            dontBother = true;
                            remainingDorms = null;
                        }
                        else
                            remainingDorms = new List<string>(activity.Dorms);

                        foreach(var pair in activity.DormPairs)
                        {
                            foreach(var dorm in new string[] { pair.Dorm, pair.OtherDorm})
                            {
                                bool isGirl = dorm[1] == 'G';
                                double age = char.GetNumericValue(dorm[0]);
                                var priorities = dormActivityPriorities[dormIndices.IndexOf(dorm)];
                                int activityIndex = priorities.FindIndex(kv => kv.Key == activity.Name);
                                int priority = (priorities[activityIndex].Value * dayPriority) / 2;
                                priorities.RemoveAt(activityIndex);

                                activityTotalPriorities[activity.Name] += priority;
                                (isGirl ? activityGirlPriorities : activityBoyPriorities)[activity.Name] += priority;
                                activityYoungPriorities[activity.Name] += priority / age;
                                activityOldPriorities[activity.Name] += (int)(priority * age);
                            }

                            if (!dontBother)
                            {
                                remainingDorms.Remove(pair.Dorm);
                                remainingDorms.Remove(pair.OtherDorm);
                            }
                        }
                    }

                    if (!dontBother)
                    {
                        foreach (var dorm in remainingDorms)
                        {
                            bool isGirl = dorm[1] == 'G';
                            double age = char.GetNumericValue(dorm[0]);
                            var priorities = dormActivityPriorities[dormIndices.IndexOf(dorm)];
                            int activityIndex = priorities.FindIndex(kv => kv.Key == activity.Name);
                            int priority = priorities[activityIndex].Value * dayPriority;
                            priorities.RemoveAt(activityIndex);

                            activityTotalPriorities[activity.Name] += priority;
                            (isGirl ? activityGirlPriorities : activityBoyPriorities)[activity.Name] += priority;
                            activityYoungPriorities[activity.Name] += priority / age;
                            activityOldPriorities[activity.Name] += (int)(priority * age);
                        }
                    }
                }
                --dayPriority;
            }

            void PrintDict(IDictionary<string, int> dict, string title)
            {
                var reordered = dict.Where(
                    kv => kv.Value != 0
                ).OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key);
                Console.WriteLine(title);
                foreach(var activity in reordered)
                {
                    Console.WriteLine("\t{0} - {1}", activity.Key, activity.Value);
                }
            }

            PrintDict(activityTotalPriorities, "Total");
            PrintDict(activityGirlPriorities, "Girls");
            PrintDict(activityBoyPriorities, "Boys");
            PrintDict(activityYoungPriorities.ToDictionary(kv => kv.Key, kv => (int)Math.Round(kv.Value)), "Young");
            PrintDict(activityOldPriorities, "Old");*/

            /*var exclusivesDict = new SortedDictionary<string, List<KeyValuePair<string, bool>>>(
                Schedule.Activities.Where(
                    a => a.Flags.HasFlag(ActivityFlags.Exclusive)
                ).ToDictionary(
                    a => a.Abbreviation,
                    a => new List<KeyValuePair<string, bool>>()
                )
            );

            foreach(var dorm in Schedule.Dorms)
            {
                var priorityDorm = dorms[dorm.Abbreviation];
                foreach(var exclusive in dorm.AllowedExclusiveActivities)
                {
                    var abbrv = Schedule.Activities[exclusive].Abbreviation;
                    exclusivesDict[abbrv].Add(
                         new KeyValuePair<string, bool>(priorityDorm.Name, priorityDorm.Activities.Contains(abbrv))   
                    );
                }
            }*/

            Console.ReadLine();
        }

        private static void CreatePrioritiesObjects()
        {
            List<PriorityDay> days = new List<PriorityDay>();
            foreach (var day in new string[] { "Monday", "Tuesday", "Wednesday", "Thursday" })
            {
                days.Add(new PriorityDay(@"C:\Users\ZACH-GAMING\Documents\Higher Ground\Priorities\" + day + ".txt"));
            }

            days.SaveAs(@"C:\Users\ZACH-GAMING\Documents\Higher Ground\Priorities\PriorityDays.txt");
            SortedDictionary<string, PriorityDorm> dorms = new SortedDictionary<string, PriorityDorm>();
            foreach (var dorm in Schedule.Dorms)
            {
                dorms.Add(dorm.Abbreviation, new PriorityDorm(dorm.Abbreviation));
            }

            foreach (var day in days)
            {
                foreach (var activity in day.Activities)
                {
                    foreach (var dorm in activity.Dorms)
                    {
                        dorms[dorm].Activities.Add(activity.Name);
                    }

                    foreach (var pair in activity.DormPairs)
                    {
                        dorms[pair.Dorm].OtherDorms.Add(pair.OtherDorm);
                        dorms[pair.OtherDorm].OtherDorms.Add(pair.Dorm);
                    }
                }
            }

            dorms.Values.SaveAs(@"C:\Users\ZACH-GAMING\Documents\Higher Ground\Priorities\PriorityDorms.txt");

            Console.WriteLine("DORMS:");
            foreach (var dorm in dorms.Values)
            {
                Console.WriteLine("\t" + dorm.Name);
                Console.WriteLine("\t\t" + dorm.Activities.ToArrayString());
                Console.WriteLine("\t\t" + dorm.OtherDorms.ToArrayString());
            }
            Console.WriteLine();

            List<PriorityActivityFull> priorityActivities = new List<PriorityActivityFull>();
            foreach (
                var group in days.SelectMany(
                    d => d.Activities.Select(
                        a => new KeyValuePair<string, PriorityActivity>(d.Name, a)
                    )
                ).GroupBy(a => a.Value.Name)
            )
            {
                priorityActivities.Add(new PriorityActivityFull(group.Key, group));
            }

            priorityActivities.SaveAs(@"C:\Users\ZACH-GAMING\Documents\Higher Ground\Priorities\PriorityActivities.txt");

            Console.WriteLine("ACTIVITIES:");
            foreach (var activity in priorityActivities)
            {
                Console.WriteLine("\t" + activity.Name);
                Console.WriteLine("\t\t" + activity.Count.ToString());
                Console.WriteLine("\t\t" + activity.Dorms.ToArrayString());
                Console.WriteLine("\t\tDorm Pairs:");
                foreach (var dormPair in activity.DormPairs)
                {
                    Console.WriteLine("\t\t\t" + dormPair.Dorm + "/" + dormPair.OtherDorm);
                }
                Console.WriteLine("\t\tDays:");
                foreach (var day in activity.Days)
                {
                    Console.WriteLine("\t\t\t" + day.Key);
                    Console.WriteLine("\t\t\t\t" + day.Value.Length);
                    Console.WriteLine("\t\t\t\t" + day.Value.ToArrayString());
                }
            }
        }
    }

    public struct DormPair
    {
        public string Dorm { get; private set; }
        public string OtherDorm { get; private set; }

        public DormPair(string dorm, string otherDorm)
        {
            Dorm = dorm;
            OtherDorm = otherDorm;
        }
    }
}
