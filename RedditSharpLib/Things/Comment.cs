using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace RedditSharpLib.Things
{
    public sealed class Comment : SubredditContent
    {
        [JilDirective(false)]
        private RedditThingData<IListing<RedditThingData<Comment>>> replies { get; set; }
        private Comment[] comments { get; set; }
        public 
    }
}
