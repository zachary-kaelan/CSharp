using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CataclysmTesting.Models
{
    public class ExplosiveMaterial
    {
        public string ID { get; set; }
        public int PortionWeight { get; set; }
        public float Density { get; set; }
        public float DetonationVelocity { get; set; }
        public float GurneyConstant { get; set; }
        public float RE_Factor { get; set; }
        public bool ShockSensitive { get; set; }
        public bool FireSensitive { get; set; }
    }
}
