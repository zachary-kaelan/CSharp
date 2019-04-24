using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace RedditSharpLib.Things
{
    public abstract class UserContent : ICreated, IRedditThing
    {
        [JilDirective("body")]
        public string Body { get; protected set; }
        [JilDirective("body_html")]
        public string BodyHTML { get; protected set; }
        [JilDirective("title")]
        public string Title { get; protected set; }

        [JilDirective(false, Name = "selftext")]
        protected string _selftext { get; set; }
        [JilDirective(false, Name = "selftext_html")]
        protected string _selftext_html { get; set; }
        [JilDirective(false, Name = "title")]
        protected string _title { get; set; }
        [JilDirective(false, Name = "subject")]
        protected string _subject { get; set; }
        [JilDirective(false, Name = "link_title")]
        protected string _link_title { get; set; }

        [JilDirective("likes")]
        public bool? LikedByUser { get; set; }
        [JilDirective("subreddit")]
        public string Subreddit { get; protected set; }
        [JilDirective("author")]
        public string Author { get; protected set; }

        [JilDirective("created")]
        public DateTime CreatedAt { get; protected set; }
        [JilDirective("created_utc")]
        public DateTimeOffset CreatedAt_UTC { get; protected set; }

        [JilDirective("id")]
        public string Id { get; protected set; }
        [JilDirective("name")]
        public string FullName { get; protected set; }
        [JilDirective("kind", Ignore = false)]
        public string Kind { get; protected set; }

        public virtual void Initialize()
        {
            if (Kind == "t1")
                Title = _link_title;
            else if (Kind == "t3")
            {
                Title = _link_title;
                Body = _selftext;
                BodyHTML = _selftext_html;
            }
            else if (Kind == "t4")
                Title = String.IsNullOrWhiteSpace(_subject) ?
                    _link_title :
                    _subject;
        }
    }
}
