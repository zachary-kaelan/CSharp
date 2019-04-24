using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.Models
{
    public class SequenceReader
    {
        private CodonSequence[] ReadingFrames { get; set; }
        private bool IncludeAntiSense { get; set; }

        public SequenceReader(UnambiguousSequence sequence, bool includeAntiSense)
        {
            IncludeAntiSense = includeAntiSense;
            ReadingFrames = new CodonSequence[IncludeAntiSense ? 6 : 3];
            
        }
    }
}
