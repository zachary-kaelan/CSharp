using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using RedditSharp;
using RedditSharp.Extensions;
using RedditSharp.Multi;
using RedditSharp.Things;
using RedditLib;
using ZachLib;

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
            /*var allowed_subs = reddit.GetPopularSubreddits().Where(
                s => s.Subscribers.HasValue &&
                     s.Subscribers.Value >= 6000 &&
                     Utils.COMPARE_INFO.IndexOf(
                        s.Name + s.Description, "suicide", Utils.IGNORE_CASE_AND_SYMBOLS
                     ) == -1
            );*/

            var allowed_subs = File.ReadAllLines(@"E:\Temp\AllowedSubreddits.txt"); 
                
                /*.Select(s => s.ToLower()).Distinct().Except(
                File.ReadAllLines(@"E:\Temp\DisallowedSubreddits.txt").Select(
                    d => ZachRGX.SYMBOLS.Replace(d, "").ToLower()
                )
            );
            File.WriteAllLines(@"E:\Temp\AllowedSubreddits.txt", allowed_subs);*/

            while(true)
            {
                Subreddit all = reddit.RSlashAll;
                DateTime now = DateTime.Now;
                DateTime last30Minutes = now.AddHours(0.5);
                var posts = all.New.Where(p => (p.Created.DateTime - now).TotalHours <= 8 && allowed_subs.Contains(p.SubredditName));
                var comments = posts.SelectMany(
                    p => p.Comments.SelectMany(
                        c => c.Comments.SelectMany(
                            c2 => c2.Comments.SelectMany(
                                c3 => c3.Comments.SelectMany(
                                    c4 => c4.Comments.SelectMany(
                                        c5 => c5.Comments
                                    )
                                )
                            )
                        )
                    )
                ).Where(
                    c => (c.Created - now).TotalMinutes <= 30
                ).Where(
                    c => compInf.IndexOf(c.Body, "/r/thathappened", opts) != -1 &&
                         !ZachRGX.LFS_CheckQuote1.IsMatch(c.Body) &&
                         !ZachRGX.LFS_CheckQuote2.IsMatch(c.Body) &&
                         !ZachRGX.CheckSarcasm.IsMatch(c.Body)
                ).DeDupe();

                Console.WriteLine(comments.Count());

                File.AppendAllLines(
                    @"E:\RedditBots\LeFakeStoryBot.txt",
                    comments.Select(
                        c => c.Reply(
                            "> Le fake story...\r\n\r\n\"*The stories and information posted here are artistic works of fiction and falsehood. Only a fool would take anything posted here as fact.*\"\r\n\r\n -Someone forgotten"
                        ).FullName
                    )
                );



                /*posts.Where(
                    p => p.Comments.Any(
                        c => compInf.IndexOf(
                            c.Body, 
                            "/r/thathappened", 
                            opts
                        ) != -1
                    )
                );*/

                comments = null;
                posts = null;
                all = null;
                GC.Collect();

                Thread.Sleep(TimeSpan.FromMinutes(30));
                
            }
        }
    }
}
