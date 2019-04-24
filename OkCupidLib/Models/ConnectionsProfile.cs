using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class ConnectionsProfile : BaseProfileModel
    {
        internal static Fields FIELDS_DEFAULT = new Fields(
            SearchFields.likes | 
            SearchFields.thumbs | 
            SearchFields.location | 
            SearchFields.userinfo | 
            SearchFields.online | 
            SearchFields.percentages | 
            SearchFields.last_contacts, 
            new KeyValuePair<SearchFields, FieldParam>(
                SearchFields.thumbs, 
                new FieldParam(1, "225x225")
            )
        );

        public bool online { get; protected set; }
        public Likes likes { get; protected set; }
        public LastContacts last_contacts { get; protected set; }
        public Profile.Percentages percentages { get; protected set; }
        public LocationInfoListModel location { get; protected set; }
    }

    public class Connection<T> where T : BaseUserModel
    {
        public string section { get; protected set; }
        public long time { get; protected set; }
        public T user { get; protected set; }
    }
}
