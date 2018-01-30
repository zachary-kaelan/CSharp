using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChanLib
{
    public class Post
    {
        public string ID { get; private set; }
        public DateTime LastModified { get; private set; }
        public 
    }

    public struct PostFile
    {
        public bool IsDeleted { get; private set; }
        public string Filename { get; private set; }
        public string FileExtension { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ThumbnailWidth { get; private set; }
        public int ThumbnailHeight { get; private set; }
        public long Time { get; private set; }
        public string MD5 { get; private set; }
        public long Filesize { get; private set; }
    }
}
