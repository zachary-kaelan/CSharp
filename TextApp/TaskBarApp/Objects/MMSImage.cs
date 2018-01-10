namespace TaskBarApp.Objects
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    public class MMSImage
    {
        public string ext { get; set; }

        public Image image { get; set; }

        public long messageID { get; set; }

        public string mimeType { get; set; }

        public string storageKey { get; set; }
    }
}

