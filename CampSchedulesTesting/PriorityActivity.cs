using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CampSchedulesTesting
{
    public class PriorityActivity
    {
        public string Name { get; private set; }
        public string[] Dorms { get; private set; }
        public DormPair[] DormPairs { get; private set; }
        public int Count { get; private set; }

        public PriorityActivity() { }

        public PriorityActivity(Match match)
        {
            Name = match.Groups["Activity"].Value;
            var dormsString = match.Groups["Dorms"].Value;
            var split = dormsString.Split(new string[] { ", " }, StringSplitOptions.None);
            Count = split.Length;

            if (dormsString.Contains('/'))
            {
                Dorms = split.SelectMany(s => s.Split('/')).ToArray();
                DormPairs = split.Where(
                    s => s.Contains('/')
                ).Select(
                    s => s.Split('/')
                ).Select(
                    s => new DormPair(s[0], s[1])
                ).ToArray();
            }
            else
            {
                Dorms = split.ToArray();
                DormPairs = new DormPair[0];
            }
        }
    }
}
