using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace CampSchedulesLib_v1.Models
{
    public class Schedule
    {
        public int Year { get; private set; }
        public Day[] Days { get; private set; }
        public SortedDictionary<Dorm, DormInfo> Dorms { get; private set; }
        public ScheduledDay[] ScheduledDays { get; private set; }
        public SortedDictionary<string, DormActivities> DormsActivities { get; private set; }
        public SortedDictionary<string, ActivityInfo> Activities { get; private set; }
        private SortedDictionary<int, DormActivityOption> CalculatedOptions { get; set; }

        public Schedule()
        {
            DateTime now = DateTime.Now;
            Year = now.Year;
        }

        public Schedule(string dormsPath, string activitiesPath, string dormActivitiesPath)
        {
            Dorms = new SortedDictionary<Dorm, DormInfo>(
                Utils.LoadCSV<DormInfo>(dormsPath).ToDictionary(d => d.Dorm)
            );
            Activities = new SortedDictionary<string, ActivityInfo>(
                Utils.LoadCSV<ActivityInfo>(activitiesPath).ToDictionary(a => a.Name)
            );
            DormsActivities = new SortedDictionary<string, DormActivities>(
                Utils.LoadCSV<DormActivities>(dormActivitiesPath).ToDictionary(
                    d => d.Dorm.ToString().Substring(1)
                )
            );
        }

        public void DistributeActivities(Dictionary<Dorm, DormActivityOptionsList> dormsOptions, int blocksAvailable)
        {
            var allAvailableActivities = dormsOptions.Values.SelectMany(v => v.Options.Keys).Distinct().OrderBy().ToArray();
            Dictionary<Dorm, double> priorityChanges = dormsOptions.ToDictionary(o => o.Key, o => o.Value.FairShare);
            Dictionary<string, KeyValuePair<Dorm, Dorm>> assignedActivities = new Dictionary<string, KeyValuePair<Dorm, Dorm>>();
            Dictionary<Dorm, int> blocksLeft = dormsOptions.Keys.ToDictionary(d => d, d => blocksAvailable);
            foreach(var activity in allAvailableActivities)
            {
                Dictionary<Dorm, DormActivityOption> dormsActivity = new Dictionary<Dorm, DormActivityOption>();
                var activityInfo = Activities[activity];
                var optsTemp = activityInfo.Duration > 1 ?
                    dormsOptions.Where(d => blocksLeft[d.Key] >= activityInfo.Duration) :
                    dormsOptions;
                foreach(var dorm in optsTemp)
                {
                    if (dorm.Value.Options.TryGetValue(activity, out DormActivityOption option))
                        dormsActivity.Add(dorm.Key, option);
                }

                var maxDorm = dormsActivity.GetByMax(d => d.Value.Priority);
                if (maxDorm.Value.OtherDorm != Dorm.None)
                {
                    priorityChanges[maxDorm.Value.OtherDorm] -= maxDorm.Value.OtherPriority;
                    --blocksLeft[maxDorm.Value.OtherDorm];
                }
                assignedActivities.Add(activity, new KeyValuePair<Dorm, Dorm>(maxDorm.Key, maxDorm.Value.OtherDorm));
            }
        }

        public DormActivityOptionsList GetDormPriorities(Dorm dorm)
        {
            // Potential TO-DO: Consolidate priority functions into a single activities foreach loop.
            var dormInfo = Dorms[dorm];
            var dormActivities = DormsActivities[dormInfo.Name];
            List<DormActivityOption> options = new List<DormActivityOption>();

            foreach (var activity in dormActivities.AvailableActivities)
            {
                int priority = dormActivities.Priorities.TryGetValue(
                    activity, 
                    out int priorityTemp
                ) ? priorityTemp : 0;
                var activityInfo = Activities[activity];

                if (activityInfo.MaxDorms == 2)
                    options.AddRange(
                        dormActivities.PartnerPriorities.Select(
                            p =>
                            {
                                int hashcode = (int)(dorm | p.Key) ^ activity.GetHashCode();
                                if (CalculatedOptions.TryGetValue(hashcode, out DormActivityOption calculated))
                                    return calculated;
                                else
                                {
                                    var otherInfo = Dorms[p.Key];
                                    var otherActivities = DormsActivities[otherInfo.Name];
                                    var opt = otherActivities.Priorities.TryGetValue(activity, out int otherPriority) ?
                                        new DormActivityOption(
                                            activity, dormInfo.Dorm,
                                            p.Value + activityInfo.Priority + priority,
                                            p.Key, otherPriority
                                        ) : new DormActivityOption(
                                            activity, dormInfo.Dorm,
                                            p.Value + activityInfo.Priority + priority,
                                            p.Key, Array.BinarySearch(otherActivities.AvailableActivities, activity) < 0 ? 
                                                -1 : p.Value + activityInfo.Priority
                                        );

                                    CalculatedOptions.Add(hashcode, opt);
                                    return opt;
                                }
                            }
                        )
                    );

                if (activityInfo.MinDorms == 1)
                    options.Add(
                        new DormActivityOption(
                            activity, 
                            dormInfo.Dorm, 
                            priority + activityInfo.Priority
                        )
                    );
            }

            double prioritiesSum = options.Sum(o => o.Priority);
            var optionsPercentages = options.Select(
                o => new DormActivityOption(o, o.Priority / prioritiesSum)
            ).OrderByDescending(o => o.Priority).ToArray();

            return new DormActivityOptionsList(
                options.OrderByDescending(o => o.Priority).ToArray(), 
                optionsPercentages, 
                prioritiesSum
            );
        }
    }
}
