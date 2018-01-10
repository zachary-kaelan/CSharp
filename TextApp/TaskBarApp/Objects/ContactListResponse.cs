namespace TaskBarApp.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ContactListResponse
    {
        public int page { get; set; }

        public int pages { get; set; }

        public List<Contact> response { get; set; }

        public object sessions { get; set; }

        public bool success { get; set; }

        public int total { get; set; }
    }
}

