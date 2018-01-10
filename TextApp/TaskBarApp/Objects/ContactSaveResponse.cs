namespace TaskBarApp.Objects
{
    using System;
    using System.Runtime.CompilerServices;

    public class ContactSaveResponse
    {
        public Contact response { get; set; }

        public object sessions { get; set; }

        public bool success { get; set; }
    }
}

