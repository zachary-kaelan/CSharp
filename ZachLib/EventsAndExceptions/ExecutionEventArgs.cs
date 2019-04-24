using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.EventsAndExceptions
{
    public class ExecutionEventArgs : EventArgs
    {
        public System.Net.HttpStatusCode ResponseCode { get; set; }
        public string URL { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public bool HasResponse { get; set; }
        public string LogName { get; set; }
    }
}
