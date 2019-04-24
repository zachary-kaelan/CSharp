using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class Message
    {
        public string body { get; protected set; }
        public string receiverid { get; protected set; }
        public bool reply { get; protected set; }
        public string service { get; protected set; }
        public string source { get; protected set; }
    }

    public class MessageData
    {
        public string correspondentid { get; protected set; }
        public string ownerid { get; protected set; }
        public byte properties { get; protected set; }
        public byte received_anchor { get; protected set; }
        public byte? received_snippet { get; protected set; }
        public byte received_time { get; protected set; }
        public string sent_anchor { get; protected set; }
        public string sent_snippet { get; protected set; }
        public long sent_time { get; protected set; }
        public string subject { get; protected set; }
        public string threatid { get; protected set; }
        public bool unread { get; protected set; }
    }

    public class MessageResponse
    {
        public string msgid { get; protected set; }
        public byte nway { get; protected set; }
        public byte pending { get; protected set; }
        public byte status { get; protected set; }
        public byte success { get; protected set; }
        public string threadid { get; protected set; }
    }
}
