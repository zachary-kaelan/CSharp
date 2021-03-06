﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditSharpPlus.Extensions;
using System;

namespace RedditSharpPlus.Things
{
    /// <summary>
    /// A user that is banned in a subreddit.
    /// </summary>
    public class BannedUser : RedditUser
    {
        /// <inheritdoc />
        public BannedUser(IWebAgent agent, JToken json) : base(agent, json) {
            var data = json["name"] == null ? json["data"] : json;
            base.Name = data["name"].ValueOrDefault<string>();
            var id = data["id"].ValueOrDefault<string>();
            if (id.Contains("_"))
            {
                base.Kind = "t2";
                base.Id = id.Split('_')[1];
                base.FullName = id;
            }
        }

        /// <summary>
        /// Date the user was banned.
        /// </summary>
        [JsonProperty("date")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? BanDate { get; private set; }

        /// <summary>
        /// Ban note.
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; private set; }

        /// <summary>
        /// This will always return 0 for BannedUsers
        /// </summary>
        [JsonIgnore]
        public new int CommentKarma => 0;

        /// <summary>
        /// This will always return 0 for BannedUsers
        /// </summary>
        [JsonIgnore]
        public new int LinkKarma => 0;
    }
}
