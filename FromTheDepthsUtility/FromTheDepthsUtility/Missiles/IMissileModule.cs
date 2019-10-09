using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.Missiles
{
    public enum MissileModuleType
    {
        Propulsion
    }

    public interface IMissileModule
    {
        byte Health { get; }
        byte Drag { get; }
        MissileModuleType Type { get; }
    }
}
