using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressingMatters
{
    [Flags]
    public enum Tags
    {
        None,
        Sexy,
        Tits,
        Lips = 4,
        Legs = 8,
        Ass = 16,
        Eyes = 32,
        Hair = 64,
        Cute = 128,
        HighQuality = 256,
        WhoreLook = 512
    }

    class Matter
    {
    }
}
