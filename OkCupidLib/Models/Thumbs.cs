using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace OkCupidLib.Models
{
    public class Thumbs
    {
        [JilDirective("225x225")]
        public string _225x225 { get; protected set; }

        [JilDirective("100x100")]
        public string _100x100 { get; protected set; }

        [JilDirective("400x400")]
        public string _400x400 { get; protected set; }

        [JilDirective("60x60")]
        public string _60x60 { get; protected set; }

        [JilDirective("82x82")]
        public string _82x82 { get; protected set; }

        [JilDirective("383x230")]
        public string _383x230 { get; protected set; }

        [JilDirective("800x800")]
        public string _800x800 { get; protected set; }

        [JilDirective("160x160")]
        public string _160x160 { get; protected set; }

        [JilDirective("120x120")]
        public string _120x120 { get; protected set; }

        public ThumbnailInfo info { get; protected set; }
    }
}
