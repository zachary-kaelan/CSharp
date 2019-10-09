using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models.ProfileData
{
    public class ProfileDetail
    {
        public DetailText text { get; protected set; }
        public DetailInfo info { get; protected set; }
        public DetailIcon icon { get; protected set; }
    }

    public class DetailText
    {
        public string text { get; protected set; }
        public DetailTextColor text_color { get; protected set; }
    }

    public struct DetailTextColor
    {
        public byte blue { get; protected set; }
        public byte green { get; protected set; }
        public byte red { get; protected set; }
        public byte alpha { get; protected set; }
    }

    public struct DetailInfo
    {
        public string name { get; protected set; }
    }

    public class DetailIcon
    {
        public object icon_color { get; protected set; }
        public string url { get; protected set; }
    }
}
