using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

using RedditSharp.Things;
using ZachLib;


namespace RedditLib
{
    public struct Ratings
    {
        //public SortedDictionary<int, int> ScoreByHour { get; set; }
        public SortedDictionary<DayOfWeek, SortedDictionary<int, double>> ScoreByHourByDay { get; set; }
        public SortedDictionary<DayOfWeek, double> ScoreByDay { get; set; }
        public Dictionary<KeyValuePair<DayOfWeek, int>, double> ScoreByHourAndDay { get; set; }

        public List<KeyValuePair<string, long>> Timings { get; set; }
        private Stopwatch timer { get; set; }

        public Ratings(IEnumerable<RedditThing> posts)
        {
            //var pllPosts = posts.AsParallel();

            double sum = 0;
            Timings = new List<KeyValuePair<string, long>>();
            timer = new Stopwatch();
            timer.Start();

            var ratingsTemp = posts.Select(
                p => new KeyValuePair<DateTime, RedditThingRating>(
                    p.DateTime,
                    new RedditThingRating(p)
                )
            );
            posts = null;

            timer.Stop();
            Timings.Add(new KeyValuePair<string, long>("CreateRedditThingRatings", timer.ElapsedMilliseconds));
            timer.Restart();

            var ImportanceRatioScale = ratingsTemp.Select(kv => kv.Value).SumRatings();
            double multiplier = Math.Pow(ImportanceRatioScale.Gold * ImportanceRatioScale.Replies * ImportanceRatioScale.Score, 1.0 / 3.0);
            RedditThingRating.Multipliers = new RedditThingRating()
            {
                Gold = multiplier / ImportanceRatioScale.Gold,
                Score = multiplier / ImportanceRatioScale.Score,
                Replies = multiplier / ImportanceRatioScale.Replies
            };

            timer.Stop();
            Timings.Add(new KeyValuePair<string, long>("SumRatingsGetMultiplier", timer.ElapsedMilliseconds));
            timer.Restart();

            /*var ratings = ratingsTemp.Select(
                r => new KeyValuePair<DateTime, double>(
                    r.Key, r.Value.GetWeightedTotal()
                )
            ).GroupBy(
                r => r.Key.DayOfYear
            ).SelectMany(
                g =>
                {
                    double temp = g.Sum(r => r.Value);
                    return g.Select(
                        r => new KeyValuePair<DateTime, double>(
                            r.Key, r.Value / temp
                        )
                    );
                }
            );*/
            var ratings = ratingsTemp.Select(
                r => new KeyValuePair<KeyValuePair<DayOfWeek, int>, double>(
                    new KeyValuePair<DayOfWeek, int>(
                        r.Key.DayOfWeek, 
                        Convert.ToInt32(
                            Math.Round(
                                r.Key.TimeOfDay.TotalHours
                            )
                        )
                    ), r.Value.GetWeightedTotal()
                )
            );

            timer.Stop();
            ratingsTemp = null;
            Timings.Add(new KeyValuePair<string, long>("NormalizeByDate", timer.ElapsedMilliseconds));
            timer.Restart();

            sum = ratings.Sum(kv => kv.Value);

            timer.Stop();
            Timings.Add(new KeyValuePair<string, long>("GetNormalizedSum", timer.ElapsedMilliseconds));
            timer.Restart();

            ScoreByHourAndDay = ratings.GroupBy(
                r => r.Key, 
                r => r.Value,
                (k, g) => new KeyValuePair<KeyValuePair<DayOfWeek, int>, double>(k, g.Sum() / sum)
            ).ToDictionary();

            timer.Stop();
            Timings.Add(new KeyValuePair<string, long>("ScoreByHourAndDay", timer.ElapsedMilliseconds));
            timer.Restart();

            var byDay = ratings.GroupBy(
                r => r.Key.Key,
                r => new KeyValuePair<int, double>(r.Key.Value, r.Value)
            );
            ratings = null;
            ScoreByDay = new SortedDictionary<DayOfWeek, double>(
                byDay.GroupBy(
                    g => g.Key,
                    r => r.Sum(p => p.Value),
                    (k, g) => new KeyValuePair<DayOfWeek, double>(k, g.Sum())
                ).ToDictionary()
            );

            timer.Stop();
            Timings.Add(new KeyValuePair<string, long>("ScoreByDay", timer.ElapsedMilliseconds));
            timer.Restart();

            ScoreByHourByDay = new SortedDictionary<DayOfWeek, SortedDictionary<int, double>>(
                byDay.GroupBy(
                    g => g.Key,
                    g => g.AsEnumerable(), 
                    (k, g) => new KeyValuePair<DayOfWeek, SortedDictionary<int, double>>(
                        k, new SortedDictionary<int, double>(
                            g.SelectMany(r => r).GroupBy(
                                r => r.Key,
                                r => r.Value,
                                (k2, g2) => new KeyValuePair<int, double>(
                                    k2, g2.Sum() / sum
                                )
                            ).ToDictionary()
                        )
                    )
                ).ToDictionary()
            );

            timer.Stop();
            Timings.Add(new KeyValuePair<string, long>("ScoreByHourByDay", timer.ElapsedMilliseconds));

            ratings = null;
        }

        private const string DAY_FORMAT = " ~ {0} ~ : {1}\r\n{2}";
        public override string ToString()
        {
            timer.Restart();
            var temp = ScoreByDay;
            string str = 
                "~ ------------------- ~\r\n" +
                "~ --- BY HOUR/DAY --- ~\r\n" +
                "~ ------------------- ~\r\n" +
                String.Join(
                    "\r\n",
                    ScoreByHourByDay.Select(
                        d => new KeyValuePair<KeyValuePair<DayOfWeek, double>, SortedDictionary<int, double>>(
                            new KeyValuePair<DayOfWeek, double>(
                                d.Key, temp[d.Key]
                            ), d.Value
                        )
                    ).OrderByDescending(d => d.Key.Value).Take(5).Select(
                        d => String.Format(
                            DAY_FORMAT,
                            d.Key.Key,
                            d.Key.Value,
                            String.Join(
                                "\r\n",
                                d.Value.OrderByDescending(h => h.Value).Take(3).Select(
                                    h => "\t" + h.Key.ToString() + " - " + h.Value.ToString("#.000")
                                )
                            )
                        )
                    )
                ) +
                "\r\n\r\n" +
                "~ ----------------------- ~\r\n" +
                "~ --- BY HOUR AND DAY --- ~\r\n" +
                "~ ----------------------- ~\r\n" +
                String.Join(
                    "\r\n",
                    ScoreByHourAndDay.OrderByDescending(kv => kv.Value).Take(15).Select(
                        kv => kv.Key.Key.ToString() + "\t - \t" + kv.Key.Value.ToString() + ": \t" + kv.Value.ToString("#.000")
                    )
                );
            timer.Stop();
            temp = null;
            Timings.Add(new KeyValuePair<string, long>("ToString", timer.ElapsedMilliseconds));
            return str;
        }
    }

    // ----------------------------------------- //
    // ----------------- REDDIT ---------------- //
    // ----------------------------------------- //

    public struct RedditThing : IEnumerable<RedditThing>
    {
        public int Score { get; set; }
        public int NumComments { get; set; }
        public int Gold { get; set; }
        public DateTime DateTime { get; set; }
        //public TimeSpan Time { get; set; }
        //public string DayOfWeek { get; set; }
        //public string HourOfDay { get; set; }
        private IEnumerable<RedditThing> Comments { get; set; }
        public bool IsComment { get; set; }
        //private long MemorySaved { get; set; }

        public RedditThing(Post post, bool discardReplies, bool discardSubreplies) : this()
        {
            LoadPost(post);
            NumComments = post.CommentCount;
            if (!discardReplies)
                Comments = post.ListComments(100).Select(c => new RedditThing(c, discardSubreplies));
            /*if (!discardReplies || !discardSubreplies)
            {
                long baseMemory = GC.GetTotalMemory(false);
                GCSettings.LatencyMode = GCLatencyMode.Batch;
                int generation = GC.GetGeneration(post);
                post = null;
                GC.Collect(generation, GCCollectionMode.Optimized, true, true);
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
                MemorySaved = baseMemory - GC.GetTotalMemory(false);
            }
            else*/
                post = null;
        }

        public RedditThing(Post post, bool discardReplies) : this(post, discardReplies, true) { }

        public RedditThing(Post post) : this(post, false, true) { }

        public RedditThing(Comment comment, bool discardReplies) : this()
        {
            LoadComment(comment);
            if (discardReplies)
                NumComments = comment.Comments.Count();
            else
            {
                Comments = comment.Comments.Select(c => new RedditThing(c, true));
                NumComments = Comments.Count();
            }
            comment = null;
        }

        public RedditThing(Comment comment) : this(comment, true) { }

        public void LoadPost(Post post)
        {
            Score = post.Score;
            Gold = post.Gilded;
            NumComments = post.CommentCount;
            //Date = post.Created.Date;
            //Time = post.Created.TimeOfDay;
            DateTime = post.Created.DateTime;
            //DayOfWeek = post.Created.DayOfWeek.ToString();
            //HourOfDay = (post.Created.Hour + (post.Created.Minute > 30 ? 1 : 0)).ToString();
            IsComment = false;
        }

        public void LoadComment(Comment comment)
        {
            Score = comment.Score;
            Gold = comment.Gilded;
            //Date = comment.Created.Date;
            //Time = comment.Created.TimeOfDay;
            DateTime = comment.Created.DateTime;
            //DayOfWeek = comment.Created.DayOfWeek.ToString();
            //HourOfDay = (comment.Created.Hour + (comment.Created.Minute > 30 ? 1 : 0)).ToString();
            IsComment = true;
        }

        public IEnumerator<RedditThing> GetEnumerator()
        {
            return Comments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Comments.GetEnumerator();
        }
    }

    public interface IRedditThingRating
    {
        double Gold { get; set; }
        double Score { get; set; }
        double Replies { get; set; }

        double GetWeightedTotal();
        //IRedditThingRating FixNaNs();
    }

    public struct RedditThingRating : IRedditThingRating
    {
        public static RedditThingRating Multipliers = new RedditThingRating() { Score = 1, Gold = 1, Replies = 1 };
        public double Gold { get; set; }
        public double Score { get; set; }
        public double Replies { get; set; }

        public RedditThingRating(RedditThing thing) : this(thing.Gold, thing.Score, thing.NumComments) { }

        public RedditThingRating(int gold, int replies, int score) 
        {
            Gold = gold;
            Replies = replies;
            Score = score;
        }

        public RedditThingRating(double gold, double replies, double score)
        {
            Gold = (Double.IsNaN(gold) || Double.IsInfinity(gold) ? 0.0 : gold);
            Replies = (Double.IsNaN(replies) || Double.IsInfinity(replies) ? 0.0 : replies);
            Score = (Double.IsNaN(score) || Double.IsInfinity(score) ? 0.0 : score);
        }

        public double GetWeightedTotal()
        {
            return (Gold * Multipliers.Gold) + (Score * Multipliers.Score) + (Replies * Multipliers.Replies);

        }
    }
}
