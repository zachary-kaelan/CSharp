using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;
using Newtonsoft.Json;
using ProtoBuf;

namespace Slack
{
    public struct Response
    {
        bool ok;
        string latest;
        string error;
        string warning;
    }

    public class Channel
    {
        string id { get; set; }
        string name { get; set; }
        int created { get; set; }
        string creator { get; set; }
        bool is_archived { get; set; }

        string[] members { get; set; }
        ChannelEntry topic { get; set; }
        ChannelEntry purpose { get; set; }

        string last_read { get; set; }
        object latest { get; set; }
        int unread_count { get; set; }
        int unread_count_display { get; set; }
    }

    public class Public : Channel
    {
        bool is_channel;
        bool is_general;
        bool is_member;
    }

    public class Private : Channel
    {
        bool is_group;
        bool is_mpim;
    }

    public struct ChannelEntry
    {
        string value;
        string creator;
        int last_set;
    }

    [ProtoContract]
    public struct SlackFile
    {
        public string id;
        public int created;
        public int timestamp;
        public string name;
        public string title;

        public string mimetype;
        public string filetype;
        public string pretty_type;

        public string user;
        public string mode;
        public bool editable;
        public bool is_external;

        public string external_type;
        public string username;
        public long size;
        public int lines;
        public int lines_more;

        public bool is_public;
        public bool public_url_shared;
        public bool display_as_bot;

        public string[] channels;
        public string[] groups;
        public string[] ims;

        public Comment initial_comment;
        public int num_stars;
        public bool is_starred;
        public string[] pinned_to;

        public Reaction[] reactions;
        public int comments_count;

        public string url_private;
        public string url_private_download;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendLine(this.name);
            sb.Append("   Created: \t\t");
            sb.AppendLine(Timestamp.ToDateTime(this.created).ToString("d"));
            sb.Append("   SlackFileType: \t");
            sb.AppendLine(this.filetype);
            sb.Append("   Size: \t\t\t");
            sb.AppendLine(this.size.ToString());
            sb.Append("   URL: \t\t\t");
            sb.AppendLine(this.url_private);

            return sb.ToString();
        }
    }

    public struct Comment
    {
        string id;
        int created;
        int timestamp;
        string user;
        string comment;
        string channel;
    }

    public struct Reaction
    {
        string name;
        int count;
        string[] users;
    }

    /*
    public struct Message
    {
        string type;
        string channel;
        string user;
        string text;
        string ts;
        
    }

    public struct Edited
    {
        string user;
        string ts;
    }
    */

    public struct User
    {
        public string id;
        public string team_id;
        public string name;
        public bool deleted;
        public string color;
        public string real_name;
        public string tz;
        public string tz_label;
        public int tz_offset;

        public Dictionary<string, object> profile;
        public bool is_admin;
        public bool is_owner;
        public long updated;
        public bool has_2fa;
    }
}
