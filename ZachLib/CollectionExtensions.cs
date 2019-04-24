using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace ZachLib
{
    public static class CollectionExtensions
    {
        #region DictionariesExtensions
        #region Extract
        public static V Extract<K, V>(this IDictionary<K, V> dict, K key)
        {
            if (dict.TryGetValue(key, out V value))
            {
                dict.Remove(key);
                return value;
            }
            return default(V);
        }

        public static bool Extract<K, V>(this IDictionary<K, V> dict, K key, out V value)
        {
            if (dict.TryGetValue(key, out value))
            {
                dict.Remove(key);
                return true;
            }
            return false;
        }
#endregion

        #region TryAdd
        public static bool TryAdd<K, V>(this IDictionary<K, V> dict, K key, V value)
        {
            if (dict.ContainsKey(key))
                return false;
            else
            {
                dict.Add(key, value);
                return true;
            }
        }

        public static bool TryAdd<K, V>(this IDictionary<K, V> dict, K key, V value, out V existingValue)
        {
            if (!dict.TryGetValue(key, out existingValue))
            {
                dict.Add(key, value);
                return true;
            }
            return false;
        }

        public static bool TryAdd<K, V>(this IDictionary<K, V> dict, KeyValuePair<K, V> kv)
        {
            if (dict.ContainsKey(kv.Key))
                return false;
            else
            {
                dict.Add(kv.Key, kv.Value);
                return true;
            }
        }

        public static bool TryAdd<K, V>(this IDictionary<K, V> dict, KeyValuePair<K, V> kv, out V existingValue)
        {
            if (!dict.TryGetValue(kv.Key, out existingValue))
            {
                dict.Add(kv.Key, kv.Value);
                return true;
            }
            return false;
        }
        #endregion

        #region TryAddAll
        public static bool TryAddAll<K, V>(this IDictionary<K, V> dict, IEnumerable<KeyValuePair<K, V>> keyValues)
        {
            bool succeed = true;
            foreach (var keyValue in keyValues)
            {
                succeed &= dict.TryAdd(keyValue);
            }
            return succeed;
        }

        public static bool TryAddAll<K, V>(this IDictionary<K, V> dict, IEnumerable<V> values, Func<V, K> keySelector)
        {
            bool succeed = true;
            foreach (var value in values)
            {
                succeed &= dict.TryAdd(keySelector(value), value);
            }
            return succeed;
        }
        #endregion

        #region CompareTo
        public static DictionaryComparisonResults<K, V> CompareTo<K, V>(this IDictionary<K, V> dict, IDictionary<K, V> dict2)
            where V : IComparable<V>
        {
            DictionaryComparisonResults<K, V> results = new DictionaryComparisonResults<K, V>();
            var OnlyInDict1 = new List<K>();
            var ValueDifferences = new List<KeyValuePair<K, KeyValuePair<V, V>>>();

            List<K> dict2Keys = dict2.Keys.ToList();
            foreach (var kv in dict)
            {
                if (dict2.TryGetValue(kv.Key, out V value))
                {
                    dict2Keys.Remove(kv.Key);
                    if (((value == null) != (kv.Value == null)) || ((IComparable<V>)kv.Value).CompareTo(value) != 0)
                        ValueDifferences.Add(
                            new KeyValuePair<K, KeyValuePair<V, V>>(
                                kv.Key,
                                new KeyValuePair<V, V>(
                                    kv.Value,
                                    value
                                )
                            )
                        );
                }
                else
                    OnlyInDict1.Add(kv.Key);
            }

            results.OnlyInDict1 = OnlyInDict1.OrderBy();
            results.ValueDifferences = ValueDifferences.OrderBy(kv => kv.Key);
            results.OnlyInDict2 = dict2Keys.OrderBy();
            return results;
        }
        #endregion

        #region Append
        public static void Append<K, V>(this IDictionary<K, V> dict, K key, V value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }

        public static void Append<K, V>(this IDictionary<K, V> dict, KeyValuePair<K, V> kv)
        {
            if (dict.ContainsKey(kv.Key))
                dict[kv.Key] = kv.Value;
            else
                dict.Add(kv.Key, kv.Value);
        }
        #endregion

        #region Increment
        public static void Increment<K>(this IDictionary<K, sbyte> dict, K key)
        {
            if (dict.ContainsKey(key))
                ++dict[key];
            else
                dict.Add(key, 1);
        }

        public static void Increment<K>(this IDictionary<K, byte> dict, K key)
        {
            if (dict.ContainsKey(key))
                ++dict[key];
            else
                dict.Add(key, 1);
        }

        public static void Increment<K>(this IDictionary<K, short> dict, K key)
        {
            if (dict.ContainsKey(key))
                ++dict[key];
            else
                dict.Add(key, 1);
        }

        public static void Increment<K>(this IDictionary<K, ushort> dict, K key)
        {
            if (dict.ContainsKey(key))
                ++dict[key];
            else
                dict.Add(key, 1);
        }

        public static void Increment<K>(this IDictionary<K, int> dict, K key)
        {
            if (dict.ContainsKey(key))
                ++dict[key];
            else
                dict.Add(key, 1);
        }

        public static void Increment<K>(this IDictionary<K, uint> dict, K key)
        {
            if (dict.ContainsKey(key))
                ++dict[key];
            else
                dict.Add(key, 1);
        }

        public static void Increment<K>(this IDictionary<K, long> dict, K key)
        {
            if (dict.ContainsKey(key))
                ++dict[key];
            else
                dict.Add(key, 1);
        }

        public static void Increment<K>(this IDictionary<K, ulong> dict, K key)
        {
            if (dict.ContainsKey(key))
                ++dict[key];
            else
                dict.Add(key, 1);
        }
        #endregion

        public static void Merge<X, Y>(this IDictionary<X, Y> dict, IEnumerable<KeyValuePair<X, Y>> dict2)
        {
            foreach (KeyValuePair<X, Y> kv in dict2)
            {
                dict.Append(kv);
            }
        }
        #endregion

        #region GetRandomValue(s)
        public static T GetRandomValue<T>(this IEnumerable<T> ienum)
        {
            return ienum.ElementAt(Utils.RANDOM.Next(ienum.Count()));
        }

        public static IEnumerable<T> GetRandomValues<T>(this IEnumerable<T> ienum, int numberOfValues)
        {
            int count = ienum.Count();

            for (int i = 0; i < numberOfValues; ++i)
            {
                yield return ienum.ElementAt(Utils.RANDOM.Next(count));
            }

            yield break;
        }
        #endregion

        #region ZipToDictionary
        public static Dictionary<K, V> ZipToDictionary<K, V>(this IEnumerable<K> keys, IEnumerable<V> values)
        {
            return keys.Zip(
                values,
                (k, v) => new KeyValuePair<K, V>(k, v)
            ).Distinct(
                new KeyValuePairComparer<K, V>()
            ).ToDictionary(
                kv => kv.Key,
                kv => kv.Value
            );
        }

        public static Dictionary<K, V> ZipToDictionary<K, V, I>(this IEnumerable<I> keys, IEnumerable<V> values, Func<I, K> keySelector)
        {
            return keys.Zip(
                values,
                (k, v) => new KeyValuePair<I, V>(k, v)
            ).Distinct(
                new KeyValuePairComparer<I, V>()
            ).ToDictionary(
                kv => keySelector(kv.Key),
                kv => kv.Value
            );
        }

        public static Dictionary<K, V> ZipToDictionary<K, V, I, J>(this IEnumerable<I> keys, IEnumerable<J> values, Func<I, K> keySelector, Func<J, V> valueSelector)
        {
            return keys.Zip(
                values,
                (k, v) => new KeyValuePair<I, J>(k, v)
            ).Distinct(
                new KeyValuePairComparer<I, J>()
            ).ToDictionary(
                kv => keySelector(kv.Key),
                kv => valueSelector(kv.Value)
            );
        }

        public static IEnumerable<KeyValuePair<K, V>> ZipToKeyValues<K, V>(this IEnumerable<K> keys, IEnumerable<V> values)
        {
            return keys.Zip(
                values,
                (k, v) => new KeyValuePair<K, V>(k, v)
            );
        }
        #endregion

        public static Dictionary<string, int> DistinctCounts(this IEnumerable<KeyValuePair<string, string>> ienum)
        {
            return ienum.GroupBy(
                kv => kv.Key,
                kv => String.Empty
            ).ToDictionary(
                g => g.Key,
                g => g.Count()
            );
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> keyValuePairs)
        {
            return keyValuePairs.Distinct(new KeyValuePairComparer<K, V>()).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<CSVKeyValuePair<K, V>> keyValuePairs)
            where K : IComparable<K> where V : IComparable<V>
        {
            return keyValuePairs.GroupBy(
                kv => kv.Key, 
                (k, g) => g.First()
            ).ToDictionary(
                kv => kv.Key, 
                kv => kv.Value
            );
        }

        public static ILookup<K, V> ToLookup<K, V>(this IEnumerable<KeyValuePair<K, V>> keyValuePairs)
        {
            return keyValuePairs.ToLookup(kv => kv.Key, kv => kv.Value);
        }

        public static K[] GetKeys<K, V>(this ILookup<K, V> lookup)
        {
            return lookup.Select(l => l.Key).ToArray();
        }

        public static Dictionary<K, V> Inverse<K, V>(this IDictionary<V, K> dict)
        {
            return dict.ToDictionary(
                kv => kv.Value,
                kv => kv.Key
            );
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(k => k);
        }

        public static (TElement, TElement) GetMinAndMax<TElement>(this IEnumerable<TElement> list)
            where TElement : IComparable<TElement>
        {
            TElement min = default(TElement);
            TElement max = default(TElement);
            foreach(var element in list)
            {
                if (element.CompareTo(min) < 0)
                    min = element;
                else if (element.CompareTo(max) > 0)
                    max = element;
            }
            return (min, max);
        }

        public static (double, double, bool) GetAbsMinAndMax(this IEnumerable<double> list)
        {
            double min = double.MaxValue;
            double max = double.MinValue;
            bool hasNegatives = false;
            foreach (var element in list)
            {
                if (!hasNegatives && element < 0)
                    hasNegatives = true;
                var temp = Math.Abs(element);
                if (temp < min)
                    min = temp;
                else if (temp > max)
                    max = temp;
            }
            return (min, max, hasNegatives);
        }

        #region GetByMax
        public static T GetByMax<T>(this IEnumerable<T> list, Func<T, int> numericalSelector)
        {
            T maxValue = list.First();
            int maxNum = numericalSelector(maxValue);
            foreach(var element in list.Skip(1))
            {
                int num = numericalSelector(element);
                if (num > maxNum)
                {
                    maxNum = num;
                    maxValue = element;
                }
            }
            return maxValue;
        }

        public static T GetByMax<T>(this IEnumerable<T> list, Func<T, double> numericalSelector)
        {
            T maxValue = list.First();
            double maxNum = numericalSelector(maxValue);
            foreach (var element in list.Skip(1))
            {
                double num = numericalSelector(element);
                if (num > maxNum)
                {
                    maxNum = num;
                    maxValue = element;
                }
            }
            return maxValue;
        }

        public static T GetByMax<T>(this IEnumerable<T> list, Func<T, long> numericalSelector)
        {
            T maxValue = list.First();
            long maxNum = numericalSelector(maxValue);
            foreach (var element in list.Skip(1))
            {
                long num = numericalSelector(element);
                if (num > maxNum)
                {
                    maxNum = num;
                    maxValue = element;
                }
            }
            return maxValue;
        }
        #endregion

        #region GetByMin
        public static T GetByMin<T>(this IEnumerable<T> list, Func<T, int> numericalSelector)
        {
            T minValue = list.First();
            int minNum = numericalSelector(minValue);
            foreach (var element in list.Skip(1))
            {
                int num = numericalSelector(element);
                if (num < minNum)
                {
                    minNum = num;
                    minValue = element;
                }
            }
            return minValue;
        }

        public static T GetByMin<T>(this IEnumerable<T> list, Func<T, double> numericalSelector)
        {
            T minValue = list.First();
            double minNum = numericalSelector(minValue);
            foreach (var element in list.Skip(1))
            {
                double num = numericalSelector(element);
                if (num < minNum)
                {
                    minNum = num;
                    minValue = element;
                }
            }
            return minValue;
        }

        public static T GetByMin<T>(this IEnumerable<T> list, Func<T, long> numericalSelector)
        {
            T minValue = list.First();
            long minNum = numericalSelector(minValue);
            foreach (var element in list.Skip(1))
            {
                long num = numericalSelector(element);
                if (num < minNum)
                {
                    minNum = num;
                    minValue = element;
                }
            }
            return minValue;
        }
        #endregion

        public static bool ProgressiveFiltering<TKey>(this IEnumerable<TKey> elements, Predicate<TKey>[] filters, out List<TKey> list)
        {
            list = elements.ToList();
            if (list.Count == 0)
            {
                list = null;
                return false;
            }
            List<TKey> prevList = null;
            foreach(var filter in filters)
            {
                if (list.Count == 1)
                    return true;
                else if (list.Count == 0)
                {
                    if (prevList == null)
                        list = elements.ToList();
                    else
                        list = new List<TKey>(prevList);
                }
                else
                    prevList = list;
                list = list.FindAll(filter);
            }
            if (list.Count == 1)
                return true;
            else if (list.Count == 0)
                list = new List<TKey>(prevList);
            return false;
        }

        public static int CustomBinarySearch<TElement>(this TElement[] array, TElement element, Func<TElement, TElement, bool> greaterThan, Func<TElement, TElement, bool> lessThan, Func<TElement, TElement, bool> equalTo) =>
            array.CustomBinarySearch<TElement>(element, greaterThan, lessThan, equalTo, 0, array.Length);

        public static int CustomBinarySearch<TElement>(this TElement[] array, TElement element, Func<TElement, TElement, bool> greaterThan, Func<TElement, TElement, bool> lessThan, Func<TElement, TElement, bool> equalTo, int min, int max)
        {
            int currIndex = (byte)((max + min) / 2);
            TElement current = array[currIndex];

            bool failed = false;
            while (!equalTo(current, element))
            {
                if (greaterThan(current, element))
                {
                    if (max == currIndex)
                    {
                        failed = true;
                        break;
                    }
                    max = currIndex;
                }
                else if (lessThan(current, element))
                {
                    if (min == currIndex)
                    {
                        failed = true;
                        break;
                    }
                    min = currIndex;
                }
                currIndex = (byte)((max + min) / 2);
            }

            return failed ? -1 : currIndex;
        }

        public static void Shuffle<T>(this List<T> elements, Random gen)
        {
            for (int i = elements.Count; i > 0; --i)
            {
                var j = gen.Next(i);
                var temp = elements[i];
                elements[i] = elements[j];
                elements[j] = temp;
            }
        }
    }

    public struct DictionaryComparisonResults<K, V>
    {
        public IEnumerable<K> OnlyInDict1 { get; set; }
        public IEnumerable<K> OnlyInDict2 { get; set; }
        public IEnumerable<KeyValuePair<K, KeyValuePair<V, V>>> ValueDifferences { get; set; }

        private const string TOSTRING_FORMAT = "Only In Dictionary 1:\r\n\t{0}\r\n\r\nOnly In Dictionary 2:\r\n\t{1}\r\n\r\nValue Differences:\r\n\t{2}";
        public override string ToString()
        {
            return String.Format(
                TOSTRING_FORMAT,
                String.Join(
                    "\r\n\t",
                    OnlyInDict1
                ),
                String.Join(
                    "\r\n\t",
                    OnlyInDict2
                ),
                String.Join(
                    "\r\n\t",
                    ValueDifferences.Select(
                        kv => (kv.Key == null ? "null" : kv.Key.ToString()) + ": [" +
                              (kv.Value.Key == null ? "null" : kv.Value.Key.ToString()) + ", " +
                              (kv.Value.Value == null ? "null" : kv.Value.Value.ToString()) + "]"
                    )
                )
            );
        }
    }

    public class DictionaryListComparisonResults
    {
        static DictionaryListComparisonResults()
        {
            RGX_ALPHA_NUMERIC = new Regex(@"[^0-9.:\/]", RegexOptions.Compiled);
        }

        public SortedList<string, string> Constants { get; private set; }
        public SortedList<string, string[]> Variations { get; private set; }
        public SortedList<string, int> NullCounts { get; private set; }
        public int Count { get; private set; }
        public int KeysCount { get; private set; }
        private static Regex RGX_ALPHA_NUMERIC = null;

        public DictionaryListComparisonResults(IEnumerable<IEnumerable<KeyValuePair<string, string>>> dictionaries)
        {
            Count = dictionaries.Count();
            Constants = new SortedList<string, string>();
            Variations = new SortedList<string, string[]>();
            NullCounts = new SortedList<string, int>();
            var groups = dictionaries.SelectMany(d => d).GroupBy(
                kv => kv.Key,
                kv => kv.Value,
                (k, g) => new KeyValuePair<string, string[]>(k, g.ToArray())
            );
            KeysCount = groups.Count();
            int maxVarsCount = Count / 4;
            foreach(var group in groups)
            {
                var distinct = group.Value.Distinct();

                int count = group.Value.Length;
                int distinctCount = distinct.Count();
                int nulls = Count - count;

                bool tooVariable = false;
                bool nullable = nulls != 0;
                bool single = distinctCount == 1;
                if (nullable)
                {
                    NullCounts.Add(group.Key, nulls);
                    single = false;
                    if (nulls + distinctCount == count)
                        tooVariable = true;
                    else
                        distinct = distinct.Append(null);
                }
                else if (distinctCount == count)
                    tooVariable = true;

                if (single)
                    Constants.Add(group.Key, distinct.First());
                else
                {
                    bool possibleTooVariable = distinctCount >= maxVarsCount;
                    if (tooVariable || possibleTooVariable)
                    {
                        if (distinct.Any(v => RGX_ALPHA_NUMERIC.IsMatch(v) && !String.IsNullOrWhiteSpace(v)))
                            distinct = nullable ?
                                Enumerable.Empty<string>().Append(distinctCount.ToString()) :
                                Enumerable.Empty<string>().Append((distinctCount + 1).ToString()).Append(null);
                        else
                        {
                            distinct = distinct.OrderBy(v => v.Length).ThenBy(v => v);
                            distinct = nullable ? distinct.Take(1).Append(distinct.Last()).Append(null) : distinct.Take(1).Append(distinct.Last());
                        }
                    }
                    Variations.Add(group.Key, distinct.ToArray());
                }
            }
        }

        private const string TOSTRING_FORMAT = "Constants: {0}\r\n\t{1}\r\n\r\nVariations: {2}\r\n\t{3}\r\n\r\nNull Counts: {4}\r\n\t{5}";
        public override string ToString()
        {
            return String.Format(
                TOSTRING_FORMAT,
                Constants.Count,
                String.Join(
                    "\r\n\t",
                    Constants
                ),
                Variations.Count,
                String.Join(
                    "\r\n\t",
                    Variations.Select(
                        v =>
                        {
                            int count = v.Value.Length;
                            string str = v.Key.ToString() + ": ";
                            if (count == 1)
                                str += v.Value[0];
                            else if (count == 2 && v.Value[1] == null)
                                str += v.Value[0] + ", nullable";
                            else
                            {
                                str += "\r\n\t\t\"";
                                if (v.Value[count - 1] == null)
                                {
                                    var vars = v.Value;
                                    Array.Resize(ref vars, count - 1);
                                    str += String.Join("\"\r\n\t\t\"", vars);
                                    str += "\"\r\n\t\t(null)";
                                }
                                else
                                    str += String.Join("\"\r\n\t\t\"", v.Value) + "\"";
                            }
                            return str;
                        }
                    )
                ),
                NullCounts.Count,
                String.Join(
                    "\r\n\t",
                    NullCounts
                )
            );
        }

        /*public void SaveTo(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (!folder.EndsWith("\\"))
                folder += "\\";
            Constants.SaveDictAs(folder + "Constants.txt", " - ");
            Variations
        }*/
    }
}
