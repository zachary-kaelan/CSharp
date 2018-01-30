using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

using RedditSharp.Things;

namespace RedditLib
{
    /*public struct FinalRatings
    {
        public SortedDictionary<Season, double> Seasons { get; set; }
        public SortedDictionary<Month, double> Months { get; set; }
        public SortedDictionary<DayOfWeek, double> Days { get; set; }
        public SortedDictionary<int, double> Hours { get; set; }
        
        public SortedDictionary<Season, int> ActualBySeason { get; set; }
        public SortedDictionary<Month, int> ActualByMonth { get; set; }
        public SortedDictionary<DayOfWeek, int> ActualByDay { get; set; }
        public SortedDictionary<int, int> ActualByHour { get; set; }

        public IRedditThingRating ImportanceRatioScale { get; set; }

        public FinalRatings(IEnumerable<RedditThing> posts) : this()
        {
            var ImportanceRatioScale = posts.Select(p => new RedditThingRating(p)).SumRatings();
            double multiplier = Math.Pow(ImportanceRatioScale.Gold * ImportanceRatioScale.Replies * ImportanceRatioScale.Score, 1.0 / 3.0);
            ImportanceRatioScale = (IRedditThingRating)new RedditThingRating()
            {
                Gold = multiplier / ImportanceRatioScale.Gold,
                Score = multiplier / ImportanceRatioScale.Score,
                Replies = multiplier / ImportanceRatioScale.Replies
            };
            TotalsByTimePeriod(posts);
            FinalizeTimePeriods(posts);
        }

        public void TotalsByTimePeriod(IEnumerable<RedditThing> posts)
        {
            var byDayByHour = new SortedDictionary<DayOfWeek, SortedDictionary<int, int>>(
                posts.GroupBy(
                    p => p.DateTime.DayOfWeek
                ).ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(
                        p => Math.Round(p.DateTime.TimeOfDay.TotalHours),
                        p => p
                    )
                )
            );

            ActualByHour = new SortedDictionary<int, int>(
                posts.GroupBy(
                    p => Math.Round(p.DateTime.TimeOfDay.TotalHours),
                    p => 0
                ).ToDictionary(
                    g => Convert.ToInt32(g.Key),
                    g => g.Count()
                )
            );
            
            ActualByDay = new SortedDictionary<DayOfWeek, int>(
                posts.GroupBy(
                    p => p.DateTime.DayOfWeek,
                    p => 0
                ).ToDictionary(
                    g => g.Key,
                    g => g.Count()
                )
            );
            
            ActualByMonth = new SortedDictionary<Month, int>(
                posts.GroupBy(
                    p => p.Date.Month,
                    p => 0
                ).ToDictionary(
                    g => (Month)g.Key,
                    g => g.Count()
                )
            );
            
            ActualBySeason = new SortedDictionary<Season, int>(
                posts.GroupBy(
                    p => p.Date.GetSeason(),
                    (k, values) => new KeyValuePair<Season, int>(k, values.Count())
                ).ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value
                )
            );
        }

        public void FinalizeTimePeriods(IEnumerable<RedditThing> posts)
        {
            IGrouping<string, RedditThing>[] postGroups = posts.GroupBy(p => p.Date.ToString()).ToArray();
            posts = null;

            var postsRankedByHour = postGroups.RankByHour();
            postsRankedByHour.NormalizeAll();
            Hours = postsRankedByHour.SelectMany(d => d.Value).ReNormalize();
            postsRankedByHour = null;

            var postsRankedByDay = postGroups.RankByDay();
            postsRankedByDay.NormalizeAll();
            Days = postsRankedByDay.SelectMany(d => d.Value).ReNormalize();
            postsRankedByDay = null;

            var postsRankedByMonth = postGroups.RankByMonth();
            postsRankedByMonth.NormalizeAll();
            Months = postsRankedByMonth.SelectMany(d => d.Value).ReNormalize();
            postsRankedByMonth = null;

            var postsRankedBySeason = postGroups.RankBySeason();
            postsRankedBySeason.NormalizeAll();
            Seasons = postsRankedBySeason.SelectMany(d => d.Value).ReNormalize();
            postsRankedBySeason = null;
        }
    }*/

    public struct Ratings
    {
        //public SortedDictionary<int, int> ScoreByHour { get; set; }
        public SortedDictionary<DayOfWeek, SortedDictionary<int, double>> ScoreByHourByDay { get; set; }

        public Ratings(IEnumerable<RedditThing> posts)
        {
            var ratingsTemp = posts.Select(
                p => new KeyValuePair<DateTime, RedditThingRating>(
                    p.DateTime,
                    new RedditThingRating(p)
                )
            );
            posts = null;

            var ImportanceRatioScale = ratingsTemp.Select(kv => kv.Value).SumRatings();
            double multiplier = Math.Pow(ImportanceRatioScale.Gold * ImportanceRatioScale.Replies * ImportanceRatioScale.Score, 1.0 / 3.0);
            ImportanceRatioScale = (IRedditThingRating)new RedditThingRating()
            {
                Gold = multiplier / ImportanceRatioScale.Gold,
                Score = multiplier / ImportanceRatioScale.Score,
                Replies = multiplier / ImportanceRatioScale.Replies
            };

            var ratings = ratingsTemp.Select(
                r => new KeyValuePair<DateTime, double>(
                    r.Key, r.Value.GetWeightedTotal()
                )
            ).GroupBy(
                r => r.Key.Date
            ).SelectMany(
                g =>
                {
                    double sum = g.Sum(r => r.Value);
                    return g.Select(
                        r => new KeyValuePair<DateTime, double>(
                            r.Key, r.Value / sum
                        )
                    );
                }
            );
            ratingsTemp = null;

            ScoreByHourByDay = new SortedDictionary<DayOfWeek, SortedDictionary<int, double>>(
                ratings.GroupBy(
                    r => r.Key.DayOfWeek
                ).ToDictionary(
                    g => g.Key,
                    g => new SortedDictionary<int, double>(
                        g.GroupBy(
                            r => Math.Round(r.Key.TimeOfDay.TotalHours),
                            r => r.Value
                        ).ToDictionary(
                            h => Convert.ToInt32(h.Key),
                            h => h.Average()
                        )
                    )
                )
            );
            ratings = null;
        }

        public override string ToString()
        {
            return String.Join(
                "\r\n",
                ScoreByHourByDay.Select(
                    d => " ~ " + d.Key.ToString() + " ~ \r\n" + String.Join(
                        "\r\n",
                        d.Value.OrderByDescending(h => h.Value).Select(
                            h => "\t" + h.Key.ToString() + " - " + h.Value.ToString()
                        )
                    )
                )
            );
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
                Comments = post.Comments.Select(c => new RedditThing(c, discardSubreplies));

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
