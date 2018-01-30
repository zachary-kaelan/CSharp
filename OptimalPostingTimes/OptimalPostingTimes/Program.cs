using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZachLib;
using RedditSharp;
using RedditSharp.Extensions;
using RedditSharp.Multi;
using RedditSharp.Things;
using RedditLib;
using FourChanLib;

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
            BotWebAgent webAgent = new BotWebAgent("ZephandrypusBot", "meeko011", "HXlqOyR76U3u-A", "4AD5uZkMwAXkOn6FS-_CJM26h5U", "https://www.reddit.com/u/ZephandrypusBot");
            Reddit reddit = new Reddit(webAgent, false);
            
            timer.Stop();
            long setupTime = timer.ElapsedMilliseconds;

            timer.Restart();
             Ratings dbdRatings = new Ratings(
                reddit.GetSubreddit(
                    "nootropics"
                ).Posts.Select(
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
