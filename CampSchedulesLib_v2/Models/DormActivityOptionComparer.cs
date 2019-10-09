using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Scheduling;
using ZachLib;

namespace CampSchedulesLib_v2.Models
{
    public class DormActivityOptionComparer : IComparer<DormActivityOption>
    {
        private Func<int, DormActivities> GetActivities { get; set; }
        private bool SingleDorm { get; set; }
        private Func<int, int> OtherDormOptions { get; set; }
        //private static SortedDictionary<CSVKeyValuePair<int, int>, int> _prevSorts = new SortedDictionary<CSVKeyValuePair<int, int>, int>();
        //private List<Tuple<int, int, int, int>> History = new List<Tuple<int, int, int, int>>();

        public DormActivityOptionComparer(Func<int, DormActivities> getActivitiesByID)
        {
            GetActivities = getActivitiesByID;
            SingleDorm = false;
        }

        public DormActivityOptionComparer(IDictionary<int, InterDormTracking> priorities)
        {
            OtherDormOptions = d => priorities[d].Options;
            SingleDorm = true;
        }

        public int Compare(DormActivityOption x, DormActivityOption y)
        {
            if (x.ID == y.ID)
                return 0;

            // result is x relative to y
            // lower means closer to the top of the list
            int result = 0;
            int selectorIndex = 0;
            if (x.IsRepeatedActivity == y.IsRepeatedActivity)
            {
                ++selectorIndex;
                if (x.TotalPriority == y.TotalPriority)
                {
                    ++selectorIndex;
                    if (x.HasExcess == y.HasExcess)
                    {
                        ++selectorIndex;
                        if (x.HasOther == y.HasOther || x.IsRepeatedDormToday != y.IsRepeatedDormToday)
                        {
                            ++selectorIndex;
                            if (x.Duration == y.Duration)
                            {
                                ++selectorIndex;
                                if (x.HasOther && y.HasOther)
                                {
                                    ++selectorIndex;
                                    if (SingleDorm)
                                    {
                                        x.SortIndex = -2;
                                        y.SortIndex = -2;
                                        result = OtherDormOptions(x.OtherDorm) - OtherDormOptions(y.OtherDorm);
                                    }
                                    else if (x.IsRepeatedDorm != y.IsRepeatedDorm)
                                    {
                                        ++selectorIndex;
                                        result = x.IsRepeatedDorm ? 1 : -1;
                                    }
                                    else
                                    {
                                        ++selectorIndex;
                                        result = x.OtherDormFunc().Options - y.OtherDormFunc().Options;
                                    }
                                }

                                if (result == 0)
                                {
                                    if (Schedule.RANDOMNESS_ENABLED)
                                    {
                                        return Schedule.GEN.Next(-2, 2);
                                        //_prevSorts.Add(kv, result);
                                        //return result;
                                    }
                                }
                                else
                                    return result;
                            }
                            else // sort descending
                                result = y.Duration - x.Duration;
                        }
                        else // sort descending
                            result = x.HasOther ? -1 : 1;
                    }
                    else // sort descending
                        result = x.HasExcess ? 1 : -1;
                }
                else // sorting descending
                    result = (y.TotalPriority - x.TotalPriority) < 0 ? -1 : 1;
            }
            else // sorting ascending
                result = x.IsRepeatedActivity ? 1 : -1;

            //History.Add(new Tuple<int, int, int, int>(x.ID, y.ID, result, selectorIndex));
            return result;
        }
    }
}
