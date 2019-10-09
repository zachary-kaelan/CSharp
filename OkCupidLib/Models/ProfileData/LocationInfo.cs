using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace OkCupidLib.Models.ProfileData
{
    public class BaseLocationModel
    {
        public string country_code { get; protected set; }
        public int locid { get; protected set; }
        public string state_code { get; protected set; }
    }

    public class LocationInfo : BaseLocationModel
    {
        internal static readonly Lazy<LocationInfo> DEFAULT = new Lazy<LocationInfo>(
            () => Utils.LoadJSON<LocationInfo>(
                API.PATH_MAIN + "LocationInfo.txt"
            )
        );

        public string city_name { get; protected set; }
        public string country_name { get; protected set; }
        public int default_radius { get; protected set; }
        public int density { get; protected set; }
        public int display_state { get; protected set; }
        public int latitude { get; protected set; }
        public int longitude { get; protected set; }
        public int metro_area { get; protected set; }
        public int nameid { get; protected set; }
        public int popularity { get; protected set; }
        public string postal_code { get; protected set; }
        public string state_name { get; protected set; }
    }
}
