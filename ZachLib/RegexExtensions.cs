using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZachLib
{
    public static class RegexExtensions
    {
        #region ToDictionary(s)
        public static Dictionary<string, string> ToDictionary(this Regex rgx, string input)
        {
            return rgx.Match(input).Groups.Cast<Group>().ToList()
                .FindAll(g => g.Success).Zip(
                    rgx.GetGroupNames(), (v, k) => new { k, v = v.Value }
                ).Skip(1).ToDictionary(
                    kv => kv.k,
                    kv => kv.v
                );
        }

        public static Dictionary<string, string> ToDictionary(this MatchCollection matches)
        {
            return matches.Cast<Match>().ToDictionary();
        }

        public static Dictionary<string, string> ToDictionary(this IEnumerable<Match> matches)
        {
            return matches.Select(
                m => new KeyValuePair<string, string>(m.Groups["Key"].Value, m.Groups["Value"].Value)
            ).Distinct(
                new KeyValuePairComparer<string, string>()
            ).ToDictionary();
        }

        public static Dictionary<string, string> ToDictionary(this Match match)
        {
            return match.Groups.ToDictionary();
        }

        public static Dictionary<string, string> ToDictionary(this GroupCollection groups)
        {
            return groups.Cast<Group>().Skip(1).ToDictionary(g => g.Name, g => g.Value);
        }

        public static IEnumerable<Dictionary<string, string>> ToDictionaries(this Regex rgx, string input)
        {
            return rgx.Matches(input).Cast<Match>().Select(
                m => m.Groups.Cast<Group>().ToList().FindAll(g => g.Success).Zip(
                        rgx.GetGroupNames(), (v, k) => new { k, v = v.Value }
                    ).Skip(1).ToDictionary(
                        kv => kv.k,
                        kv => kv.v
                    )
            );
        }
        #endregion

        #region ToKeyValue(s)
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValues(this Regex rgx, string input)
        {
            return rgx.Match(input).Groups.Cast<Group>().ToList()
                .FindAll(g => g.Success).Zip(
                    rgx.GetGroupNames(), (v, k) => new KeyValuePair<string, string>(k, v.Value)
                ).Skip(1);
        }

        public static IEnumerable<KeyValuePair<string, string>> ToKeyValues(this IEnumerable<Match> matches)
        {
            return matches.Select(
                m => m.ToKeyValue()
            ).Distinct(new KeyValuePairComparer<string, string>());
        }

        public static KeyValuePair<string, string> ToKeyValue(this Match match)
        {
            return new KeyValuePair<string, string>(
                match.Groups["Key"].Value,
                match.Groups["Value"].Value
            );
        }
        #endregion

        #region SplitToDictionary
        public static IDictionary<string, string> SplitToDictionary(this Regex rgx, string input, string root = "Root")
        {
            return rgx.SplitToDictionary(input, k => k, root);
        }

        public static IDictionary<K, string> SplitToDictionary<K>(this Regex rgx, string input, Func<string, K> keySelector, string root = "Root")
        {
            return rgx.SplitToDictionary(input, keySelector, v => v, root);
        }

        public static IDictionary<K, V> SplitToDictionary<K, V>(this Regex rgx, string input, Func<string, K> keySelector, Func<string, V> valueSelector, string root = "Root")
        {
            var strings = rgx.Split(input);
            return Enumerable.Range(
                0, (strings.Length + 1) / 2
            ).Select(n => n * 2).Where(n => !String.IsNullOrWhiteSpace(strings[n])).ToDictionary(
                n => keySelector(n == 0 ? root : strings[n - 1]),
                n => valueSelector(strings[n])
            );
        }
        #endregion

        #region SplitByIndex
        public static ILookup<bool, Dictionary<string, string>> SplitByIndex(this Regex rgx, string input, int index)
        {
            return rgx.Matches(input).SplitByIndex(index);
        }

        public static ILookup<bool, Dictionary<string, string>> SplitByIndex(this MatchCollection matches, int index)
        {
            return matches.Cast<Match>().SplitByIndex(index);
        }

        public static ILookup<bool, Dictionary<string, string>> SplitByIndex(this IEnumerable<Match> matches, int index)
        {
            return matches.ToLookup(m => m.Index > index, m => m.ToDictionary());
        }

        public static ILookup<bool, string> SplitByIndex(this Regex rgx, string input, int index, string group)
        {
            return rgx.Matches(input).SplitByIndex(index, group);
        }

        public static ILookup<bool, string> SplitByIndex(this MatchCollection matches, int index, string group)
        {
            return matches.Cast<Match>().SplitByIndex(index, group);
        }

        public static ILookup<bool, string> SplitByIndex(this IEnumerable<Match> matches, int index, string group)
        {
            return matches.ToLookup(m => m.Index > index, m => m.Groups[group].Value);
        }
        #endregion

        #region SplitByIndices
        public static ILookup<int, Dictionary<string, string>> SplitByIndices(this Regex rgx, string text, params int[] indicies)
        {
            return rgx.Matches(text).SplitByIndices(indicies);
        }

        public static ILookup<int, Dictionary<string, string>> SplitByIndices(this MatchCollection matches, params int[] indicies)
        {
            return matches.Cast<Match>().SplitByIndices(indicies);
        }

        public static ILookup<int, Dictionary<string, string>> SplitByIndices(this IEnumerable<Match> matches, params int[] indicies)
        {
            List<int> indiciesTemp = indicies.ToList();
            indiciesTemp.Add(int.MaxValue);
            return matches.ToLookup(m => indiciesTemp.FindIndex(i => m.Index < i), m => m.ToDictionary());
        }

        public static ILookup<int, string> SplitByIndices(this Regex rgx, string text, string group, params int[] indicies)
        {
            return rgx.Matches(text).SplitByIndices(group, indicies);
        }

        public static ILookup<int, string> SplitByIndices(this MatchCollection matches, string group, params int[] indicies)
        {
            return matches.Cast<Match>().SplitByIndices(group, indicies);
        }

        public static ILookup<int, string> SplitByIndices(this IEnumerable<Match> matches, string group, params int[] indicies)
        {
            List<int> indiciesTemp = indicies.ToList();
            indiciesTemp.Add(int.MaxValue);
            return matches.ToLookup(m => indiciesTemp.FindIndex(i => m.Index < i), m => m.Groups[group].Value);
        }
        #endregion

        #region ToObject(s)
        public static T ToObject<T>(this Regex rgx, string input) where T : new()
        {
            T obj = new T();
            Type objType = obj.GetType();
            string[] names = rgx.GetGroupNames();
            var groups = rgx.Match(input).Groups;
            string grpValue = null;
            PropertyInfo prop = null;

            foreach (string name in names)
            {
                if (groups.TryGetGroup(name, out grpValue) && objType.TryGetProperty(name, out prop))
                    prop.SetValue(obj, grpValue);
            }

            names = null;
            groups = null;

            return obj;
        }

        public static IEnumerable<T> ToObjects<T>(this Regex rgx, string input) where T : new()
        {
            var matches = rgx.GroupCollections(input);
            string[] names = rgx.Keys();
            string grpValue = null;
            PropertyInfo prop = null;

            foreach (var match in matches)
            {
                T obj = new T();
                Type objType = obj.GetType();

                foreach (string name in names)
                {
                    if (match.TryGetGroup(name, out grpValue) && objType.TryGetProperty(name, out prop))
                    {
                        object boxed = obj;
                        prop.SetValue(boxed, grpValue, null);
                        obj = (T)boxed;
                    }
                }

                yield return obj;
            }

            matches = null;
            names = null;

            yield break;
        }
        #endregion

        #region MatchesValues
        public static IEnumerable<string> MatchesValues(this Regex rgx, string input)
        {
            if (rgx.GetGroupNames().Length > 1)
                return rgx.GroupsMatrix(input).Select(m => m.ElementAt(1).Value);
            return rgx.Matches(input).Cast<Match>().Select(m => m.Value);
        }

        public static IEnumerable<string> MatchesValues(this Regex rgx, string input, string group)
        {
            return rgx.Matches(input).Cast<Match>().Select(m => m.Groups[group].Value);
        }
        #endregion

        #region GroupsManagement
        public static bool TryGetGroup(this GroupCollection grps, string key, out string grpValue)
        {
            Group output = grps[key];
            grpValue = output.Value;
            return output.Success && !String.IsNullOrWhiteSpace(grpValue);
        }

        public static GroupCollection[] GroupCollections(this Regex rgx, string input)
        {
            return rgx.Matches(input).Cast<Match>().Select(m => m.Groups).ToArray();
        }

        public static IEnumerable<IEnumerable<Group>> GroupsMatrix(this Regex rgx, string input)
        {
            return rgx.Matches(input).Cast<Match>().Select(m => m.Groups.Cast<Group>());
        }
        #endregion

        public static IEnumerable<KeyValuePair<string, string>> FromKeyValues(this Regex rgx, string input)
        {
            return rgx.Matches(input).Cast<Match>().ToKeyValues();
        }

        public static string[] Keys(this Regex rgx)
        {
            return rgx.GetGroupNames().Skip(1).ToArray();
        }
    }
}
