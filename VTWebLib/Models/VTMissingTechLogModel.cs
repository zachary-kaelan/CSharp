using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTWebLib.Models
{
    public struct VTMissingTechLogModel
    {
        public string ID { get; set; }
        public string URLID { get; set; }
        public string Name { get; set; }
        public string Office { get; set; }
        public string TechUsername { get; set; }

        public VTMissingTechLogModel(VTExportCust cust, string tech)
        {
            ID = cust.ID;
            URLID = cust.URLID;
            Name = cust.Name;
            Office = cust.Office;
            TechUsername = tech;
        }
    }
}
