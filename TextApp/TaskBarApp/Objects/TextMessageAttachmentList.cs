namespace TaskBarApp.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class TextMessageAttachmentList
    {
        public List<TextMessageAttachment> response { get; set; }

        public object sessions { get; set; }

        public int size { get; set; }

        public bool success { get; set; }

        public int total { get; set; }
    }
}

