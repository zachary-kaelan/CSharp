using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace RedditSharpLib.Things
{
    /*public enum Kind
    {
        Comment = 1,
        RedditUser,
        Post,
        PrivateMessage,
        Subreddit,
        ModAction,
        More,
        LiveUpdate,
        LiveUpdateEvent
    }*/

    public interface IRedditThing
    {
        [JilDirective("id")]
        string Id { get; }
        [JilDirective("name")]
        string FullName { get; }
        [JilDirective("kind", Ignore = false)]
        string Kind { get; }
    }

    internal struct RedditThingData<TData> : IRedditThing
    {
        [JilDirective(Ignore = false, Name = "data")]
        public TData Data { get; set; }

        [JilDirective("id")]
        public string Id { get; private set; }
        [JilDirective("name")]
        public string FullName { get; private set; }
        [JilDirective("kind", Ignore = false)]
        public string Kind { get; private set; }
    }

    internal struct RedditThingData : IRedditThing
    {
        [JilDirective(Ignore = false, Name = "data")]
        public object Data { get; set; }

        [JilDirective("id")]
        public string Id { get; private set; }
        [JilDirective("name")]
        public string FullName { get; private set; }
        [JilDirective("kind", Ignore = false)]
        public string Kind { get; private set; }
    }
}
