using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HingeAPI.Models
{
    public sealed class ProfileListModel
    {
        public string identityId { get; private set; }
        public Profile profile { get; private set; }
    }

    public class Profile
    {
        public byte age { get; protected set; }
        public byte height { get; protected set; }
        public Answer[] answers { get; protected set; }
        public string jobTitle { get; protected set; }
        public DisplayableThing gender { get; protected set; }
        public string firstName { get; protected set; }
        public string lastName { get; protected set; }
        public string userId { get; protected set; }

        public Photo mainPhoto { get; protected set; }
        public Photo[] photos { get; protected set; }
        public bool instafeedVisible { get; protected set; }

        public DisplayableThing children { get; protected set; }
        public DisplayableThing drinking { get; protected set; }
        public DisplayableThing drugs { get; protected set; }
        public DisplayableThing familyPlans { get; protected set; }
        public DisplayableThing politics { get; protected set; }
        public DisplayableThing marijuana { get; protected set; }
        public DisplayableThing smoking { get; protected set; }

        public Location location { get; protected set; }
        public DisplayableThing hometown { get; protected set; }

        public DisplayableThing[] ethnicities { get; protected set; }
        public DisplayableThing[] religions { get; protected set; }
        public DisplayableThing[] works { get; protected set; }
        public DisplayableThing[] educations { get; protected set; }
    }
}
