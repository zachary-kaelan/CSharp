using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class APIProfile : BaseProfileModel
    {
        public bool blocked { get; protected set; }
        public bool bookmarked { get; protected set; }
        public ProfileDetail[] details { get; protected set; }
        public Essay[] essays { get; protected set; }
        public object first_message { get; protected set; }
        public bool hidden { get; protected set; }
        public LastContacts last_contacts { get; protected set; }
        public long last_login { get; protected set; }
        public Likes likes { get; protected set; }
        public object linked_account { get; protected set; }
        public LocationInfoListModel location { get; protected set; }
        public MatchGenre[] match_genres { get; protected set; }
        public object nudge { get; protected set; }
        public bool online { get; protected set; }
        public Profile.Percentages percentages { get; protected set; }
        public PersonalityTraitData[] personality_traits { get; protected set; }
        public SnapshotPhoto[] photos { get; protected set; }
    }
}
