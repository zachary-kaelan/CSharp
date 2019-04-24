using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public enum Comparative
    {
        Less,
        More
    }

    public enum PersonalityTrait
    {
        adventuresome = 0,
        artsiness = 3,
        cockiness = 7,
        freedom_social = 16,
        indie = 22,
        old_fashionedness = 33,
        planning = 35,
        republican = 38,
        self_confidence = 40,
        sprirituality = 45,
        trust_in_others = 49,
        political = 52
    }

    public enum PersonalityTraitType
    {
        positive,
        negative
    }

    public class PersonalityTraitData
    {
        public PersonalityTraitType type { get; protected set; }
        public bool hidden { get; protected set; }
        public int idx { get; protected set; }
        public string name { get; protected set; }
        public Comparative comparative { get; protected set; }
        public PersonalityTrait id { get; protected set; }
        public int percentile { get; protected set; }
        /// <summary>
        /// Description based on comparative and trait.
        /// </summary>
        public string description { get; protected set; }
    }
}
