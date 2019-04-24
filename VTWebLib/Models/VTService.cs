using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTWebLib.Models
{
    public struct VTService
    {
        public string serviceId { get; set; }
        public string serviceType { get; set; }
        public string serviceSetupType { get; set; }
        public string serviceSetupColor { get; set; }
        public string serviceOrderType { get; set; }
        public string serviceOrderColor { get; set; }
        public string serviceSchedule { get; set; }
    }
}
