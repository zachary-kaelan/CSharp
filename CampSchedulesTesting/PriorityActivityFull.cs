using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesTesting
{
    public class PriorityActivityFull
    {
        public string Name { get; private set; }
        public string[] Dorms { get; private set; }
        public DormPair[] DormPairs { get; private set; }
        public int Count { get; private set; }
        public KeyValuePair<string, string[]>[] Days { get; private set; }

        public PriorityActivityFull() { }

        public PriorityActivityFull(string name, IEnumerable<KeyValuePair<string, PriorityActivity>> group)
        {
            Name = name;
            Days = group.Select(
                d => new KeyValuePair<string, string[]>(
                    d.Key, d.Value.Dorms
                )
            ).ToArray();

            var activities = group.Select(a => a.Value);
            Count = activities.Sum(a => a.Count);
            DormPairs = activities.SelectMany(a => a.DormPairs).ToArray();
            Dorms = activities.SelectMany(a => a.Dorms).ToArray();
        }
    }
}
