using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    public static class CollectionExtensions
    {
        #region DictionariesExtensions
        #region Extract
        public static V Extract<K, V>(this Dictionary<K, V> dict, K key)
        {
            if (dict.TryGetValue(key, out V value))
            {
                dict.Remove(key);
                return value;
            }
            return default(V);
        }

        public static bool Extract<K, V>(this Dictionary<K, V> dict, K key, out V value)
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

        public static void Merge<X, Y>(this Dictionary<X, Y> dict, IEnumerable<KeyValuePair<X, Y>> dict2)
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
            return keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(k => k);
        }
    }
}
