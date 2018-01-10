using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RedditSharp.Things;

namespace OptimalPostingTimes
{
    public struct FinalRatings<V>
    {
        public SortedDictionary<Season, V> Seasons { get; set; }
        public SortedDictionary<Month, V> Months { get; set; }
        public SortedDictionary<DayOfWeek, V> Days { get; set; }
        public SortedDictionary<int, V> Hours { get; set; }
        public DateTime OverallOptimal { get; set; }
        public SortedDictionary<Season, int> ActualBySeason { get; set; }
        public SortedDictionary<Month, int> ActualByMonth { get; set; }
        public SortedDictionary<DayOfWeek, int> ActualByDay { get; set; }
        public SortedDictionary<int, int> ActualByHour { get; set; }
    }

    // ----------------------------------------- //
    // ----------------- REDDIT ---------------- //
    // ----------------------------------------- //

    public struct RedditThing : IEnumerable<RedditThing>
    {
        public int Score { get; set; }
        public int NumComments { get; set; }
        public int Gold { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        //public string DayOfWeek { get; set; }
        //public string HourOfDay { get; set; }
        private IEnumerable<RedditThing> Comments { get; set; }
        public bool IsComment { get; set; }

        public RedditThing(Post post)
        {
            Score = post.Score;
            Gold = post.Gilded;
            NumComments = post.CommentCount;
            Date = post.Created.Date;
            Time = post.Created.TimeOfDay;
            //DayOfWeek = post.Created.DayOfWeek.ToString();
            //HourOfDay = (post.Created.Hour + (post.Created.Minute > 30 ? 1 : 0)).ToString();
            Comments = post.Comments.Select(c => new RedditThing(c)).ToArray();
            IsComment = false;
        }

        public RedditThing(Comment comment)
        {
            Score = comment.Score;
            Gold = comment.Gilded;
            Date = comment.Created.Date;
            Time = comment.Created.TimeOfDay;
            //DayOfWeek = comment.Created.DayOfWeek.ToString();
            //HourOfDay = (comment.Created.Hour + (comment.Created.Minute > 30 ? 1 : 0)).ToString();
            Comments = comment.Comments.Select(c => new RedditThing(c)).ToArray();
            NumComments = Comments.Count();
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

    public struct RedditThingRating
    {
        public double Gold { get; set; }
        public double Score { get; set; }
        public double Replies { get; set; }

        public RedditThingRating(RedditThing thing)
        {
            Gold = thing.Gold;
            Score = thing.Score;
            Replies = thing.NumComments;
        }

        public RedditThingRating(int gold, int replies, int score)
        {
            Gold = gold;
            Replies = replies;
            Score = score;
        }

        public RedditThingRating(double gold, double replies, double score)
        {
            Gold = gold;
            Replies = replies;
            Score = score;
        }
    }
}
