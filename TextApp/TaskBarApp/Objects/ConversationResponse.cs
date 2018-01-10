namespace TaskBarApp.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ConversationResponse
    {
        public Conversation conversation { get; set; }

        public List<TextMessage> messages { get; set; }
    }
}

