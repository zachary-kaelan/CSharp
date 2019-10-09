using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CataclysmTesting.Models
{
    public class Gun
    {
        public string Category { get; set; }
        public string Skill { get; set; }
        public string Ammo { get; set; }

        public string ID { get; set; }
        public string Name { get; set; }
        public float Volume { get; set; }
        public float Weight { get; set; }
        public byte Barrel_Length { get; set; }

        public byte Reload_Noise_Volume { get; set; }

        public Faults Faults { get; set; }
        public GunFlags Flags { get; set; }

        public Gun(GunTemp gun, Gun template = null)
        {
            if (template != null)
            {
                if (gun.override_template)
                {

                }
                else
                {
                    Category = template.Name;

                    if (template.Reload_Noise_Volume != 0)
                        Reload_Noise_Volume = template.Reload_Noise_Volume;

                    // Flags

                    if (template.Faults != Faults.NONE)
                        Faults = template.Faults;

                    if (template.Flags != GunFlags.NONE)
                        Flags = template.Flags;

                    // Strings

                    if (!String.IsNullOrWhiteSpace(template.Skill))
                        Skill = template.Skill;

                    if (!String.IsNullOrWhiteSpace(template.Ammo))
                        Ammo = template.Ammo;


                }
            }
            else
            {
                
            }
        }
    }
}
