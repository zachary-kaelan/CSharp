using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WargamingAPI.Models;

namespace WargamingAPI.XMLModels
{
    public sealed class XMLShell : XMLBase
    {
        public ShellType kind { get; set; }
        public double caliber { get; set; }
        public bool isTracer { get; set; }
        public XMLShellDamage damage { get; set; }
        public double explosionRadius { get; set; }
        public double piercingPowerLossFactorByDistance { get; set; }

        public struct XMLShellDamage
        {
            public int armor { get; set; }
            public int devices { get; set; }
        }
    }
}
