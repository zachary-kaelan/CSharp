using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationTheory
{
    class Methods
    {
        public static float InformationContent(float messageProbability) =>
            (float)Math.Log(1 / messageProbability, 2);
    }
}
