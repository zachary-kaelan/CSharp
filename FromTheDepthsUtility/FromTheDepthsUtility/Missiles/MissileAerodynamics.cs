using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.Missiles
{
    public class MissileAerodynamics
    {
        public float Mass { get; private set; }
        public float ThrustKN { get; private set; }
        public float Acceleration { get; private set; }
        private float DragCoefficient { get; set; }
        public float InitialSpeed { get; private set; }
        
        public MissileAerodynamics(byte size, float drag, float initialSpeed)
        {
            DragCoefficient = (1 + (10 * drag)) / 1000;
            InitialSpeed = initialSpeed;
        }
    }
}
