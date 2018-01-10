namespace TaskBarApp.Objects
{
    using System;
    using System.Runtime.CompilerServices;

    public class ConversationMetaData
    {
        public string fingerprint { get; set; }

        public long lastContactId { get; set; }

        public DateTime? lastMessageDate { get; set; }

        public string lastMessageDirection { get; set; }

        public bool lastMessageIsError { get; set; }
    }
}

