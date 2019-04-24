using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Extensions;
using RedditSharp.Things;
using ZachLib;

namespace RedditSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var webagent = new BotWebAgent("Zephandrypus", "meeko011", "KjwX25olhbEI4w", "vVmP-rTXj6EX43iGZkzywi-X4xo", "https://example.com");
            var reddit = new Reddit(webagent, true);
            var user = reddit.User;
            var saved = user.GetSaved(Sort.Top, 100, FromTime.Year).Where(p => p.Kind == "t1");
            saved.SaveAs(@"E:\RedditBots\SavedComments.txt");
            var savedFull = new SortedDictionary<string, Comment[]>(
                saved.Select(
                    p => reddit.GetComment(
                        new Uri(p.Shortlink)
                    )
                ).GroupBy(
                    c => c.Subreddit.ToLower(),
                    c => c
                ).ToDictionary(
                    s => s.Key,
                    s => s.ToArray()
                )
            );
            savedFull.SaveAs(@"E:\RedditBots\SavedCommentsFull.txt");
            var savedAskreddit = savedFull["askreddit"];
            savedAskreddit.SaveAs(@"E:\RedditBots\SavedCommentsAskreddit.txt");
            savedAskreddit = savedAskreddit.Where(
                c => (
                    c.Body.Contains("lottery") ||
                    c.Body.Contains("money")
                ) && (
                    c.Body.Contains("lawyer") ||
                    c.Body.Contains("broker")
                )
            ).OrderByDescending(c => c.Body.Length).ToArray();
            savedAskreddit.SaveAs(@"E:\RedditBots\SavedAskredditLottery.txt");
            foreach(var comment in savedAskreddit)
            {
                Console.WriteLine(comment.LinkTitle + " - " + comment.Shortlink + " - " + comment.Score);
            }
            Console.ReadLine();
        }
    }
}
