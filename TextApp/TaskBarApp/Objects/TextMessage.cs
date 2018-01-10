namespace TaskBarApp.Objects
{
    using System;
    using System.Runtime.CompilerServices;

    public class TextMessage
    {
        public string address { get; set; }

        public string body { get; set; }

        public long bodySize { get; set; }

        public string carrier { get; set; }

        public long contactId { get; set; }

        public long creatorId { get; set; }

        public string dateCreated { get; set; }

        public string dateDelivered { get; set; }

        public bool deleted { get; set; }

        public string destAddress { get; set; }

        public string fingerprint { get; set; }

        public bool hasAttachment { get; set; }

        public long id { get; set; }

        public bool isRead { get; set; }

        public string scheduledDate { get; set; }

        public string statusDesc { get; set; }

        public TransmissionState transmissionState { get; set; }

        public string type { get; set; }
    }
}

