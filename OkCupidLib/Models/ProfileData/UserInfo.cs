using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public enum Gender
    {
        Woman
    }

    public enum SexualOrientation
    {
        Straight,
        Bisexual
    }

    public class UserInfo
    {
        public string realname { get; protected set; }
        public char gender_letter { get; protected set; }
        public Gender gender { get; protected set; }
        public int age { get; protected set; }
        public string rel_status { get; protected set; }
        public string location { get; protected set; }
        public SexualOrientation orientation { get; protected set; }
        public string displayname { get; protected set; }
    }
}
