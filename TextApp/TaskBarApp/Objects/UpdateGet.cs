namespace TaskBarApp.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class UpdateGet
    {
        public object response { get; set; }

        public List<Session> sessions { get; set; }

        public bool success { get; set; }
    }
}

