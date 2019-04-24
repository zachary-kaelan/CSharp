using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v1.Models
{
    public class DormActivityOptionsList
    {
        public double PrioritiesSum { get; private set; }
        public double FairShare { get; private set; }
        public Dictionary<string, DormActivityOption> Options { get; private set; }
        public Dictionary<string, DormActivityOption> OptionsPercentages { get; private set; }

        public DormActivityOptionsList(DormActivityOption[] opts, DormActivityOption[] optsPercentages, double prioritiesSum)
        {
            Options = opts.ToDictionary(o => o.ActivityAbbrv);
            OptionsPercentages = optsPercentages.ToDictionary(o => o.ActivityAbbrv);
            PrioritiesSum = prioritiesSum;
            FairShare = PrioritiesSum / 13;
        }
    }
}
