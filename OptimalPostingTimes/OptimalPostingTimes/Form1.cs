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

namespace OptimalPostingTimes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public const string MAIN_PATH = @"E:\OptimalPostingTimes\";
        public const string REDDIT_PATH = MAIN_PATH + @"Reddit\";
        public static readonly string[] Days = new string[]
        {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"
        };
        public static readonly string[] Months = new string[]
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };
        public static readonly string[] Seasons = new string[]
        {
            "Winter",
            "Spring",
            "Summer",
            "Fall"
        };

        private void Form1_Load(object sender, EventArgs e)
        {
            

            //var boards = FourChan.GetBoards();

            Stopwatch timer = new Stopwatch();
            timer.Start();
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            BotWebAgent webAgent = new BotWebAgent("ZephandrypusBot", "meeko011", "HXlqOyR76U3u-A", "4AD5uZkMwAXkOn6FS-_CJM26h5U", "https://www.reddit.com/u/ZephandrypusBot");
            Reddit reddit = new Reddit(webAgent, false);
            timer.Stop();
            long setupTime = timer.ElapsedMilliseconds;

            timer.Restart();
            /*FinalRatings dbdRatings = new FinalRatings(
                reddit.GetSubreddit(
                    "deadbydaylight"
                ).Posts.Select(
                    p => new RedditThing(p, true)
                )
            );
            timer.Stop();
            long processTime = timer.ElapsedMilliseconds;
            dbdRatings.SaveAs(REDDIT_PATH + "DBDRatings.txt");

            FinalRatings RedditRatings = new FinalRatings(all.Posts.Select(p => new RedditThing(p)));*/
            //posts.SaveAs(REDDIT_PATH + "AllPosts.txt");

            /*Subreddit all = reddit.RSlashAll;
            var post = all.Posts.First();
            post.
            all = null;
            reddit = null;
            webAgent = null;*/

            /*RedditRatings.ActualByHour = new SortedDictionary<int, int>(
                posts.GroupBy(
                    p => Math.Round(p.Time.TotalHours),
                    (k, values) => new KeyValuePair<int, int>(Convert.ToInt32(k), values.Count())
                ).OrderBy(h => h.Key).ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value
                )
            );
            RedditRatings.ActualByDay = new SortedDictionary<DayOfWeek, int>(
                posts.GroupBy(
                    p => p.Date.DayOfWeek, 
                    (k, values) => new KeyValuePair<DayOfWeek, int>(k, values.Count())
                ).OrderBy(d => d.Key).ToDictionary(
                    kv => kv.Key, 
                    kv => kv.Value
                )
            );
            RedditRatings.ActualByMonth = new SortedDictionary<Month, int>(
                posts.GroupBy(
                    p => p.Date.Month,
                    (k, values) => new KeyValuePair<int, int>(k, values.Count())
                ).OrderBy(m => m.Key).ToDictionary(
                    kv => (Month)kv.Key,
                    kv => kv.Value
                )
            );
            RedditRatings.ActualBySeason = new SortedDictionary<Season, int>(
                posts.GroupBy(
                    p => p.Date.GetSeason(),
                    (k, values) => new KeyValuePair<Season, int>(k, values.Count())
                ).OrderBy(s => (int)s.Key).ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value
                )
            );
            RedditRatings.SaveAs(REDDIT_PATH + "Ratings");

            IGrouping<string, RedditThing>[] postGroups = posts.GroupBy(p => p.Date.ToString()).ToArray();
            posts = null;
            var postsByHour = postGroups.RankByHour();
            postsByHour.Keys.ToList().ForEach(k => postsByHour[k] = postsByHour[k].NormalizeRatings());
            RedditRatings.Hours = postsByHour.SelectMany(d => d.Value).GroupBy(
                r => r.Key, 
                r => r.Value
            ).OrderBy(g => g.Key).ToDictionary(
                g => g.Key,
                g => new RedditThingRating(
                    g.Sum(d => d.Gold),
                    g.Sum(d => d.Replies),
                    g.Sum(d => d.Score)
                )
            ).NormalizeRatings();
            RedditRatings.Hours.SaveAs(REDDIT_PATH + "Hours.txt");*/


        }
    }
}
