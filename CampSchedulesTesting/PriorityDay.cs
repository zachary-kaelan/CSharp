using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CampSchedulesTesting
{
    public class PriorityDay
    {
        private static readonly Regex RGX_PRIORITIES = new Regex(@"^ - (?<Activity>[A-Z1-9]{2}).{0,6}: (?<Dorms>.+)", RegexOptions.Compiled);
        public string Name { get; private set; }
        public PriorityActivity[] Activities { get; private set; }
        public string[] FullyBooked { get; private set; }
        public string[] MostlyBooked { get; private set; }

        public PriorityDay() { }

        public PriorityDay(string path)
        {
            Name = Path.GetFileNameWithoutExtension(path);
            var lines = File.ReadAllLines(path);
            Activities = new PriorityActivity[lines.Length - 2];
            for (int l = 0; l < Activities.Length; ++l)
            {
                Activities[l] = new PriorityActivity(RGX_PRIORITIES.Match(lines[l]));
            }

            var line = lines[Activities.Length];
            line = line.Substring(1, line.Length - 2);
            FullyBooked = line.Split(new string[] { ", " }, StringSplitOptions.None);

            line = lines[Activities.Length + 1];
            line = line.Substring(1, line.Length - 2);
            MostlyBooked = line.Split(new string[] { ", " }, StringSplitOptions.None);
        }
    }
}
