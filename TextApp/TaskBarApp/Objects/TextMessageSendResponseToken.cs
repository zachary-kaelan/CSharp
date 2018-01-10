namespace TaskBarApp.Objects
{
    using System;
    using System.Runtime.CompilerServices;

    public class TextMessageSendResponseToken
    {
        public string @class { get; set; }

        public long contact { get; set; }

        public long device { get; set; }

        public string fingerprint { get; set; }

        public string message { get; set; }
    }
}

