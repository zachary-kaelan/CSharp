namespace TaskBarApp.Objects
{
    using System;
    using System.Runtime.CompilerServices;

    public class TextMessageAttachment
    {
        public long fileSizeBytes { get; set; }

        public string mimeType { get; set; }

        public string storageKey { get; set; }
    }
}

