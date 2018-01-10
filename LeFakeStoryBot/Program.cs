using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RedditSharp;
using RedditSharp.Extensions;
using RedditSharp.Multi;
using RedditSharp.Things;

namespace LeFakeStoryBot
{
    class Program
    {
        static void Main(string[] args)
        {
            CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
            CompareOptions opts = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols;

            BotWebAgent webAgent = new BotWebAgent("LeFakeStoryBot", "meeko011", "qd1dXOmrqt1UWg", "YaD0bvQYc48DO-sNiCYnvSYCQy0", "http://127.0.0.1");
            Reddit reddit = new Reddit(webAgent);

            while(true)
            {
                Subreddit all = reddit.RSlashAll;
                DateTime now = DateTime.Now;
                DateTime last30Minutes = now.AddHours(0.5);
                var posts = all.New.Where(p => (p.Created.DateTime - now).TotalHours <= 8);
                var comments = posts.SelectMany(
                    p => p.Comments
                ).Where(
                    c => (c.Created - now).TotalMinutes <= 30
                ).Where(
                    c => compInf.IndexOf(c.Body, "/r/thathappened", opts) != -1
                );

                posts.Where(
                    p => p.Comments.Any(
                        c => compInf.IndexOf(
                            c.Body, 
                            "/r/thathappened", 
                            opts
                        ) != -1
                    )
                );
                

                Thread.Sleep(TimeSpan.FromMinutes(30));
                
            }
        }
    }
}
