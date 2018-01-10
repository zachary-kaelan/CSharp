namespace TaskBarApp.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Session
    {
        public List<Contact> contact { get; set; }

        public List<Conversation> conversation { get; set; }

        public List<TextMessage> message { get; set; }

        public string session { get; set; }
    }
}

