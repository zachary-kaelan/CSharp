using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChanLib
{
    public class Board
    {
        public string URLName { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public bool IsWorkSafe { get; private set; }
        public bool IsArchived { get; private set; }
        public int NumThreadsPerPage { get; private set; }
        public int NumPages { get; private set; }
        public Limits Limits { get; private set; }
        public Cooldowns Cooldowns { get; private set; }
    }

    public struct Limits
    {
        public long Filesize { get; private set; }
        public long WebmFilesize { get; private set; }
        public int CommentLength { get; private set; }
        public int WebmDuration { get; private set; }
        public int BumpsPerThread { get; private set; }
        public int ImagesPerThread { get; private set; }
    }

    public struct Cooldowns
    {
        // In seconds
        public int Threads { get; private set; }
        public int Replies { get; private set; }
        public int Images { get; private set; }
    }
}
