using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class DamageAdjustments
    {
        public DamageType Type { get; set; }
        public DamageModifier Modifier { get; set; }
        public DamageAdjustment Adjustment { get; set; }
    }
}
