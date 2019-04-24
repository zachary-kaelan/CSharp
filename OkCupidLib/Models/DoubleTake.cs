using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class DoubleTake
    {
        public string upgradeLink { get; protected set; }
        public bool hasWhoLikesYou { get; protected set; }
        public bool isEmpty { get; protected set; }
        public YMLData[] yml { get; protected set; }
        public bool showFlavorModal { get; protected set; }
        public string browseLink { get; protected set; }
        public int numLikesYou { get; protected set; }
        public bool isStaff { get; protected set; }
        public object interestsType { get; protected set; }
        public BaseUserModel me { get; protected set; }
        public bool newUser { get; protected set; }
        public bool hasFlavors { get; protected set; }
        public object flavors { get; protected set; }
        public bool hasAds { get; protected set; }
    }

    public class YMLData
    {
        public string thumbnail { get; protected set; }
        public string userid { get; protected set; }
        public string username { get; protected set; }
    }
}
