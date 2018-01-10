using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalPostingTimes
{
    public enum Season
    {
        Winter,
        Spring,
        Summer,
        Fall
    }

    public enum Month
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public static class ExtensionMethods
    {
        public static SortedDictionary<T, SortedDictionary<int, RedditThingRating>> RankByHour<T>(this IEnumerable<IGrouping<T, RedditThing>> groups)
        {
            return new SortedDictionary<T, SortedDictionary<int, RedditThingRating>>(
                groups.OrderBy(g => g.Key).ToDictionary(
                    g => g.Key,
                    g => new SortedDictionary<int, RedditThingRating>(
                        g.GroupBy(
                            p => Convert.ToByte(
                                Math.Round(
                                    p.Time.TotalHours
                                )
                            ), p => new RedditThingRating(p)
                        ).OrderBy(h => h.Key).ToDictionary(
                            h => Convert.ToInt32(h.Key),
                            h => new RedditThingRating(
                                h.Sum(r => r.Gold),
                                h.Sum(r => r.Replies),
                                h.Sum(r => r.Score)
                            )
                        )
                    )
                )
            );
        }

        public static SortedDictionary<K, RedditThingRating> NormalizeRatings<K>(this IDictionary<K, RedditThingRating> hours)
        {
            RedditThingRating sums = hours.Values.Aggregate(
                new RedditThingRating(0, 0, 0),
                (old, curr) =>
                {
                    old.Gold += curr.Gold;
                    old.Replies += curr.Replies;
                    old.Score += curr.Score;
                    return old;
                }
            );

            return new SortedDictionary<K, RedditThingRating>(
                hours.ToDictionary(
                    h => h.Key,
                    h => new RedditThingRating(
                        h.Value.Gold / sums.Gold,
                        h.Value.Replies / sums.Replies,
                        h.Value.Score / sums.Score
                    )
                )
            );
        }

        private static readonly int DayOfWinter = new DateTime(1, 12, 21).DayOfYear;
        private static readonly int DayOfSpring = new DateTime(1, 3, 20).DayOfYear;
        private static readonly int DayOfSummer = new DateTime(1, 6, 21).DayOfYear;
        private static readonly int DayOfFall = new DateTime(1, 9, 22).DayOfYear;

        public static Season GetSeason(this DateTime date)
        {
            if (date.DayOfYear >= DayOfWinter || date.DayOfYear < DayOfSpring)
                return Season.Winter;
            else if (date.DayOfYear < DayOfSummer)
                return Season.Spring;
            else if (date.DayOfYear < DayOfFall)
                return Season.Summer;
            else
                return Season.Fall;
        }
    }
}
