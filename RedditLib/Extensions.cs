using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;

namespace RedditLib
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
        public static SortedDictionary<T, SortedDictionary<int, IRedditThingRating>> RankByHour<T>(this IEnumerable<IGrouping<T, RedditThing>> groups)
        {
            return new SortedDictionary<T, SortedDictionary<int, IRedditThingRating>>(
                groups.OrderBy(g => g.Key).ToDictionary(
                    g => g.Key,
                    g => new SortedDictionary<int, IRedditThingRating>(
                        g.GroupBy(
                            p => Convert.ToByte(
                                Math.Round(
                                    p.DateTime.TimeOfDay.TotalHours
                                )
                            ), p => new RedditThingRating(p)
                        ).OrderBy(h => h.Key).ToDictionary(
                            h => Convert.ToInt32(h.Key),
                            h => (IRedditThingRating)new RedditThingRating(
                                h.Sum(r => r.Gold),
                                h.Sum(r => r.Replies),
                                h.Sum(r => r.Score)
                            )
                        )
                    )
                )
            );
        }

        public static SortedDictionary<T, SortedDictionary<DayOfWeek, IRedditThingRating>> RankByDay<T>(this IEnumerable<IGrouping<T, RedditThing>> groups)
        {
            return new SortedDictionary<T, SortedDictionary<DayOfWeek, IRedditThingRating>>(
                groups.OrderBy(g => g.Key).ToDictionary(
                    g => g.Key,
                    g => new SortedDictionary<DayOfWeek, IRedditThingRating>(
                        g.GroupBy(
                            p => p.DateTime.DayOfWeek,
                            p => new RedditThingRating(p)
                        ).OrderBy(d => d.Key).ToDictionary(
                            d => d.Key,
                            d => (IRedditThingRating)new RedditThingRating(
                                d.Sum(r => r.Gold),
                                d.Sum(r => r.Replies),
                                d.Sum(r => r.Score)
                            )
                        )
                    )
                )
            );
        }

        public static SortedDictionary<T, SortedDictionary<Month, IRedditThingRating>> RankByMonth<T>(this IEnumerable<IGrouping<T, RedditThing>> groups)
        {
            return new SortedDictionary<T, SortedDictionary<Month, IRedditThingRating>>(
                groups.OrderBy(g => g.Key).ToDictionary(
                    g => g.Key,
                    g => new SortedDictionary<Month, IRedditThingRating>(
                        g.GroupBy(
                            p => p.DateTime.Month,
                            p => new RedditThingRating(p)
                        ).OrderBy(m => m.Key).ToDictionary(
                            m => (Month)m.Key,
                            m => (IRedditThingRating)new RedditThingRating(
                                m.Sum(r => r.Gold),
                                m.Sum(r => r.Replies),
                                m.Sum(r => r.Score)
                            )
                        )
                    )
                )
            );
        }

        public static SortedDictionary<T, SortedDictionary<Season, IRedditThingRating>> RankBySeason<T>(this IEnumerable<IGrouping<T, RedditThing>> groups)
        {
            return new SortedDictionary<T, SortedDictionary<Season, IRedditThingRating>>(
                groups.OrderBy(g => g.Key).ToDictionary(
                    g => g.Key,
                    g => new SortedDictionary<Season, IRedditThingRating>(
                        g.GroupBy(
                            p => p.DateTime.GetSeason(),
                            p => new RedditThingRating(p)
                        ).OrderBy(s => s.Key).ToDictionary(
                            s => s.Key,
                            s => (IRedditThingRating)new RedditThingRating(
                                s.Sum(r => r.Gold),
                                s.Sum(r => r.Replies),
                                s.Sum(r => r.Score)
                            )
                        )
                    )
                )
            );
        }

        public static SortedDictionary<K, double> ReNormalize<K>(this IEnumerable<KeyValuePair<K, IRedditThingRating>> ratings)
        {
            return new SortedDictionary<K, double>(
                ratings.GroupBy(
                    r => r.Key,
                    r => r.Value
                ).OrderBy(g => g.Key).ToDictionary(
                    g => g.Key,
                    g => (IRedditThingRating)new RedditThingRating(
                        g.Sum(d => d.Gold),
                        g.Sum(d => d.Replies),
                        g.Sum(d => d.Score)
                    )
                ).NormalizeRatings().ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value.GetWeightedTotal()
                )
            );
        }

        public static IRedditThingRating SumRatings<R>(this IEnumerable<R> ratings) where R : IRedditThingRating
        {
            return ratings.Aggregate(
                (IRedditThingRating)new RedditThingRating(0, 0, 0),
                (old, curr) =>
                {
                    old.Gold += curr.Gold;
                    old.Replies += curr.Replies;
                    old.Score += curr.Score;
                    return old;
                }
            );
        }

        public static SortedDictionary<K, IRedditThingRating> NormalizeRatings<K>(this IDictionary<K, IRedditThingRating> ratings)
        {
            IRedditThingRating sums = (IRedditThingRating)ratings.Values.SumRatings();

            return new SortedDictionary<K, IRedditThingRating>(
                ratings.ToDictionary(
                    r => r.Key,
                    r => (IRedditThingRating)new RedditThingRating(
                        r.Value.Gold / sums.Gold,
                        r.Value.Replies / sums.Replies,
                        r.Value.Score / sums.Score
                    )
                )
            );
        }

        public static void NormalizeAll<K, InnerKey>(this SortedDictionary<K, SortedDictionary<InnerKey, IRedditThingRating>> dict)
        {
            dict.Keys.ToList().ForEach(k => dict[k] = dict[k].NormalizeRatings());
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

        public static IEnumerable<Comment> DeDupe(this IEnumerable<Comment> comments, int minutesCheckInterval = 30)
        {
            comments = comments.Where(
                c => !c.AuthorName.EndsWith("bot", StringComparison.CurrentCultureIgnoreCase)
            ).GroupBy(
                c => c.ParentId,
                c => c,
                (k, g) => g.OrderByDescending(c => c.Score).First()
            ).GroupBy(
                c => c.LinkId,
                c => c,
                (k, g) => g.OrderByDescending(c => c.Score).Take(10)
            ).SelectMany(c => c).OrderByDescending(
                c => c.Score
            );

            return comments.Take(minutesCheckInterval * 21).Concat(comments.Reverse().Take(minutesCheckInterval * 6));
        }
    }
}
