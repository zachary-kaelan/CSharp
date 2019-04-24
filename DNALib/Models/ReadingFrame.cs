using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.Models
{
    internal class ReadingFrame : AbstractReadingFrame
    {
        public int FramePosition => base._startPosition;

        internal ReadingFrame(int position) : base(position)
        {
        }
    }
}
