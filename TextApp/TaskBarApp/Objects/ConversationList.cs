namespace TaskBarApp.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ConversationList
    {
        public List<Conversation> response { get; set; }

        public object sessions { get; set; }

        public long size { get; set; }

        public bool success { get; set; }

        public int total { get; set; }
    }
}

