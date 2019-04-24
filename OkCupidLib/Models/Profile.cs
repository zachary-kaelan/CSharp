using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class Profile : BaseProfileModel
    {
        public PersonalityTraitData[] personality_traits { get; protected set; }
        public bool online { get; protected set; }
        public Percentages percentages { get; protected set; }
        public string[] detail_tags { get; protected set; }

        public struct Percentages
        {
            public int match { get; protected set; }
            public int enemy { get; protected set; }
        }
    }
}
