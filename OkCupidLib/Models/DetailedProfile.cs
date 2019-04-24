using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class DetailedProfile : BaseProfileModel
    {
        public string[] detail_tags { get; protected set; }
        public bool online { get; protected set; }
        public Likes likes { get; protected set; }
        public LastContacts last_contacts { get; protected set; }
        public long last_login { get; protected set; }
        public ProfileDetail[] details { get; protected set; }
        public WhatIWant wiw { get; protected set; }
        public object first_message { get; protected set; }
        public Profile.Percentages percentages { get; protected set; }
        public Essay[] essays { get; protected set; }
    }
}
