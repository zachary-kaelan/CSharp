namespace TaskBarApp.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class AccountItem
    {
        public string connectionStatus { get; set; }

        public string countryCode { get; set; }

        public DateTime? lastSyncDate { get; set; }

        public string messagePeek { get; set; }

        public string number { get; set; }

        public string password { get; set; }

        public string session { get; set; }

        public string title { get; set; }

        public List<TextMessage> unReadMessageList { get; set; }
    }
}

