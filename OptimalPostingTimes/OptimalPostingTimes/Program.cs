using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZachLib;
using RedditSharp;
using RedditSharp.Extensions;
using RedditSharp.Multi;
using RedditSharp.Things;
using RedditLib;
using FChan;
using FChan.Library;

namespace OptimalPostingTimes
{
    static class Program
    {
        public const string MAIN_PATH = @"E:\OptimalPostingTimes\";
        public const string REDDIT_PATH = MAIN_PATH + @"Reddit\";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Stopwatch timer = new Stopwatch();
            timer.Start();
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            /*AuthProvider auth = new AuthProvider();
            AuthProvider.OAuthToken = "iKCshSoRsRgQIDF0k5tbLtXBd-I";
            AuthProvider.RefreshToken = "32120013-v7HGCrSOM55Q7FvbmxgHMcH6Svw";*/
            
            BotWebAgent webAgent = new BotWebAgent("Zephandrypus", "meeko011", "KjwX25olhbEI4w", "vVmP-rTXj6EX43iGZkzywi-X4xo", "https://example.com");
            Reddit reddit = new Reddit(webAgent, true);
            
            timer.Stop();
            long setupTime = timer.ElapsedMilliseconds;

            /*var user = reddit.GetUser("not_queen_BHVR");
            var days = user.GetComments(Sort.New, 100, FromTime.Year).GroupBy(
                c => c.Created.DayOfWeek,
                c => ""
            ).Concat(
                user.GetPosts(Sort.New, 100, FromTime.Year).GroupBy(
                    p => p.Created.DayOfWeek,
                    p => ""
                )
            ).GroupBy(
                t => t.Key,
                g => g.Count(),
                (k, g) => new KeyValuePair<DayOfWeek, int>(k, g.Sum())
            ).OrderBy(kv => kv.Key);
            Console.WriteLine(
                String.Join(
                    "\r\n",
                    days
                )
            );
            Console.ReadLine();*/

            /*File.WriteAllLines(
                @"E:\Temp\AllowedSubreddits.txt",
                reddit.GetPopularSubreddits().Where(
                    s => s.Subscribers.HasValue &&
                         s.Subscribers.Value >= 6000 &&
                         Utils.COMPARE_INFO.IndexOf(
                            s.Name + s.Description, "suicide", Utils.IGNORE_CASE_AND_SYMBOLS
                         ) == -1
                ).Select(s => s.Name).Distinct()
            );*/

            /*var postTemp = reddit.GetPost(new Uri("https://www.reddit.com/r/deadbydaylight/comments/7xdudl/shrine/?st=jdux4vr8&sh=b3702b7a"));

            string[] allowedFlairs = new string[]
            {
                "BHVR Official",
                "News",
                "Salt / Rant",
                "User Video",
                "Gameplay",
                "Suggestion",
                "Discussion",
                "Question",
                "Shitpost",
                "Guide"
            };
            string[] searchTerms = new string[]
            {
                "freddy",
                "nightmare",
                "sweater"
            };

            var matchingPosts = reddit.GetSubreddit("deadbydaylight").Search(
                new DateTime(2017, 10, 26),
                new DateTime(2017, 11, 9),
                Sorting.Top
            ).Where(
                p => 
                     p.Score > 1 && p.CommentCount > 1 && 
                     allowedFlairs.Contains(p.LinkFlairText) &&
                     searchTerms.Any(
                         s => Utils.COMPARE_INFO.IndexOf(
                                p.Title + p.SelfText, s,
                                Utils.IGNORE_CASE_AND_SYMBOLS
                            ) >= 0
                    )
            ).ToDictionary(
                p => "[" + p.LinkFlairText + "] " + p.Title,
                p => "https://www.reddit.com" + p.Permalink.ToString()
            );

            foreach(var post in matchingPosts)
            {
                Console.WriteLine(post.Key);
                Console.WriteLine("\t" + post.Value);
            }
            Console.ReadLine();*/

            timer.Restart();
            DateTime now = DateTime.Now;
            DateTime oneYearAgo = now.AddYears(-1);

            //var threads = reddit.RSlashAll.Search(oneYearAgo, now).GroupBy(p => new KeyValuePair<bool, int>());

            /*var tempSave = reddit.GetSubreddit("nofap").PostsNew.TakeWhile(
                p => p.Created.DateTime >= oneYearAgo
            ).Select(
                p => new RedditThing(p, true)
            );
            tempSave.SaveAs(@"E:\OptimalPostingTimes\Reddit\DeadByDaylightPosts.txt");*/
            var ratings = new Ratings(
                reddit.GetSubreddit("imgoingtohellforthis").Posts.Select(
                    p => new RedditThing(p, true)
                )
            );
            //tempSave = null;
            Console.WriteLine(ratings.ToString());

            timer.Stop();
            ratings.Timings.Add(new KeyValuePair<string, long>("TotalTime", timer.ElapsedMilliseconds));
            Console.WriteLine(
                "\r\n\r\n" + 
                String.Join(
                    "\r\n",
                    ratings.Timings.Select(
                        kv => kv.Key + ": " + kv.Value.ToString() + "ms"
                    )
                )
            );
            Console.ReadLine();


            timer.Restart();

             Ratings dbdRatings = new Ratings(
                reddit.GetSubreddit(
                    "deadbydaylight"
                ).Search(
                    oneYearAgo, now
                ).Where(
                    p => p.Score > 1 && p.CommentCount > 1 && p.Comments.Any(c => c.AuthorName.ToLower().EndsWith("bhvr"))
                ).Select(
                    p => new RedditThing(p, true)
                )
            );
            timer.Stop();
            long processTime = timer.ElapsedMilliseconds;
            //dbdRatings.SaveAs(REDDIT_PATH + "DBDRatings.txt");

            /*Console.WriteLine("Best Season: " + dbdRatings.Seasons.OrderByDescending(s => s.Value).First().Key.ToString());
            Console.WriteLine("Best Month: " + dbdRatings.Months.OrderByDescending(m => m.Value).First().Key.ToString());
            Console.WriteLine("Best Day: " + dbdRatings.Days.OrderByDescending(d => d.Value).First().Key.ToString());
            Console.WriteLine("Best Hour: " + dbdRatings.Hours.OrderByDescending(h => h.Value).First().Key.ToString());*/
            Console.WriteLine("Elapsed Milliseconds: " + processTime.ToString());
            Console.WriteLine(dbdRatings.ToString());
            Console.ReadLine();

            Subreddit all = reddit.RSlashAll;

            //FinalRatings RedditRatings = new FinalRatings(all.Posts.Select(p => new RedditThing(p)));
            //posts.SaveAs(REDDIT_PATH + "AllPosts.txt");
            all = null;
            reddit = null;
            webAgent = null;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


    }
}
