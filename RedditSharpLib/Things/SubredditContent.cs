using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace RedditSharpLib.Things
{
    public enum Sorting
    {
        Confidence,
        Top,
        New,
        Controversial,
        Old,
        Random,
        QA,
        Live
    }

    public abstract class SubredditContent : UserContent
    {
        [JilDirective("ups")]
        public int Upvotes { get; protected set; }
        [JilDirective("downs")]
        public int Downvotes { get; protected set; }
        [JilDirective("score")]
        public int Score { get; protected set; }
        [JilDirective("subreddit_id")]
        public string SubredditId { get; protected set; }
        [JilDirective("author_flair_css_class")]
        public string FlairCSSClass { get; protected set; }
        [JilDirective("author_flair_text")]
        public string Flair { get; protected set; }
        [JilDirective("saved")]
        public bool Saved { get; set; }

        [JilDirective("distinguished")]
        public string Distinguished { get; protected set; }
        [JilDirective("edited", IsUnion = true)]
        public bool Edited { get; protected set; }
        [JilDirective("edited", IsUnion = true)]
        public DateTime EditedAt { get; protected set; }

        [JilDirective("link_url")]
        public string LinkURL { get; protected set; }
        [JilDirective("num_comments")]
        public int CommentCount { get; protected set; }
        [JilDirective(true)]
        protected Sorting Sorting { get; set; }
    }
}
