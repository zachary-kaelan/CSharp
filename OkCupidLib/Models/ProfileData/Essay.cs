using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models.ProfileData
{
    public class Essay
    {
        public int essayid { get; protected set; }
        public string raw_content { get; protected set; }
        public string rawtext { get; protected set; }
        public string contents { get; protected set; }
        public int group_type { get; protected set; }
        public int id { get; protected set; }
        public string placeholder { get; protected set; }
        public object[] essay_pics { get; protected set; }
        public string content { get; protected set; }
        public string title { get; protected set; }
        public int essay_groupid { get; protected set; }
        public string clean_content { get; protected set; }
    }
}
