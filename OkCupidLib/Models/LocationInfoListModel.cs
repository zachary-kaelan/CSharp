using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace OkCupidLib.Models
{
    public class LocationInfoListModel : BaseLocationModel
    {
        public LocationFormatted formatted { get; protected set; }
    }

    public class LocationFormatted
    {
        public double distance { get; protected set; }
        public string distance_unit { get; protected set; }
        public object neighborhood { get; protected set; }
        [JilDirective("short")]
        public string formatted_short { get; protected set; }
        [JilDirective("standard")]
        public string formatted_standard { get; protected set; }
    }
}
