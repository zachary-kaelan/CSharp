using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingAPI.XMLModels
{
    public class XMLGun : XMLBaseTechTree
    {
        public double impulse { get; set; }
        public XMLGunRecoil recoil { get; set; }
        public double reloadTime { get; set; }
        public double aimingTime { get; set; }
        public XMLGunClip? clip { get; set; }
        public XMLGunClip? burst { get; set; }
        public double shotDispersionRadius { get; set; }
        
        public struct XMLGunRecoil
        {
            public LODDistance lodDist { get; set; }
            public double amplitude { get; set; }
            public double backoffTime { get; set; }
            public double returnTime { get; set; }
        }

        public struct XMLGunClip
        {
            public int count { get; set; }
            public int rate { get; set; }
        }

        public struct XMLGunDispersionFactors
        {
            public double turretRotation { get; set; }
            public double afterShot { get; set; }
            public double whileGunDamaged { get; set; }
        }

        public struct XMLGunShots
        {
            public int speed { get; set; }
            public double gravity { get; set; }
            public int maxDistance { get; set; }
            public string piercingPower { get; set; }
        }
    }
}
