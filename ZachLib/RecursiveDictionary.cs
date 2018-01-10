using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using Jil;

namespace ZachLib
{
    [Flags]
    public enum HTMLContent
    {
        Paragraphs,
        Tooltips,
        Links,
        ListElements
    };

    public interface IRecursiveDictionary<T> :
        IDictionary<string, T>, IRecursiveDictionaryValue
        where T : IRecursiveDictionaryValue
    {
        void SetDict(IDictionary<string, T> dict);
        void SetDict(IEnumerable<KeyValuePair<string, IRecursiveDictionaryValue>> keyValues);
        T AsValue();
    }

    public interface IRecursiveDictionaryValue : IDisposable, IEquatable<IRecursiveDictionaryValue>
    {
        string Name { get; }
        bool hasSubvalues { get; }
        Dictionary<string, IEnumerable<KeyValuePair<string, string>>> Dictionaries { get; }
        Dictionary<string, IEnumerable<string>> Collections { get; }

        string Serialize();
        void ParseText(string text);
        void ParseText(string text, HTMLContent content);
        void SetValues(IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, string>>>> dicts, IEnumerable<KeyValuePair<string, IEnumerable<string>>> lists);
        bool IsNull();
    }

    public struct RecursiveDictionary<V> : IRecursiveDictionary<V>
        where V : IRecursiveDictionaryValue, new()
    {
        #region Properties
        private const string SERIALIZATION_FORMAT =
            "\"{0}\":{{1}}";

        public bool hasSubvalues => true;
        public string Name { get; private set; }
        private bool disposedValue { get; set; }
        private static readonly IRecursiveDictionaryValue nullInterface = null;

        private KeyValuePair<bool, bool> isNull { get; set; }

        public Dictionary<string, IEnumerable<KeyValuePair<string, string>>> Dictionaries
        {
            get => Values.SelectMany(v => v.Dictionaries).GroupBy(
                kv => kv.Key,
                kv => kv.Value,
                (key, values) => new KeyValuePair<string, IEnumerable<KeyValuePair<string, string>>>(
                    key,
                    values.SelectMany(v => v).Distinct(
                        new KeyValuePairComparer<string, string>()
                    )
                )
            ).ToDictionary();
        }

        public Dictionary<string, IEnumerable<string>> Collections
        {
            get => Values.SelectMany(v => v.Collections).GroupBy(
                kv => kv.Key,
                kv => kv.Value,
                (key, values) => new KeyValuePair<string, IEnumerable<string>>(
                    key,
                    values.SelectMany(e => e)
                )
            ).ToDictionary();
        }
#endregion

        #region Constructors
        public RecursiveDictionary(IDictionary<string, V> dict) : this()
        {
            disposedValue = false;
            dictionary = new SortedDictionary<string, V>(dict);
        }

        public RecursiveDictionary(string text, string thisName = "Root") : this()
        {
            disposedValue = false;
            Name = thisName;
            ParseText(text);
        }

        public RecursiveDictionary(string text, HTMLContent content, string thisName = "Root") : this()
        {
            disposedValue = false;
            Name = thisName;
            ParseText(text, content);
        }
#endregion

        #region IRecursiveDictionaryValue Support
        public string Serialize()
        {
            return String.Format(
                SERIALIZATION_FORMAT,
                Name,
                String.Join(
                    ",",
                    Values.Select(
                        v => v.Serialize()
                    )
                )
            );
        }

        public void SetValues(IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, string>>>> dicts, IEnumerable<KeyValuePair<string, IEnumerable<string>>> lists)
        {
            V value = new V();
            value.SetValues(dicts, lists);
            this.SetDict(new Dictionary<string, V>() { { Name, value } });
        }

        public bool IsNull()
        {
            bool check = false;
            if (isNull.Key)
                return isNull.Value;
            else if (dictionary == null || !dictionary.Any() || Keys == null || Values == null)
                check = true;
            else
            {
                string[] keys = Keys.ToArray();
                foreach (string key in keys)
                {
                    if (
                        !String.IsNullOrWhiteSpace(key) &&
                        dictionary[key] != null &&
                        !dictionary[key].IsNull()
                    ) continue;

                    dictionary[key].Dispose();
                    dictionary.Remove(key);
                }
                keys = null;
                check = !Values.Any();
            }

            isNull = new KeyValuePair<bool, bool>(true, check);
            return check;
        }
#endregion

        #region ParseText
        public void ParseText(string text)
        {
            this.ParseText(text, HTMLContent.Links | HTMLContent.ListElements | HTMLContent.Paragraphs | HTMLContent.Tooltips);
        }

        public void ParseText(string text, HTMLContent content)
        {
            var dict = ParseText(text, 1, Name, content);

            dictionary = new SortedDictionary<string, V>(
                dict.hasSubvalues ?
                    (IDictionary<string, V>)((IRecursiveDictionary<V>)dict) :
                    new Dictionary<string, V>() { { Name, (V)dict } }
            );
            dict = null;
        }
#endregion

        #region ParseText Static
        private static IRecursiveDictionaryValue ParseText(string text, int curHeader, string thisName)
        {
            return ParseText(text, curHeader, thisName, HTMLContent.Links | HTMLContent.ListElements | HTMLContent.Paragraphs | HTMLContent.Tooltips);
        }

        private static IRecursiveDictionaryValue ParseText(string text, int curHeader, string thisName, HTMLContent content)
        {
            bool isMatch = false;
            do
            {
                isMatch = HtmlUtils.HTML_HEADERS[curHeader++].IsMatch(text);
            } while (!isMatch && curHeader <= 6);

            if (curHeader == 7 && !isMatch)
            {
                IRecursiveDictionaryValue tempValue = (IRecursiveDictionaryValue)(new V());
                tempValue.ParseText(text, content);
                return tempValue.IsNull() ? null : tempValue;
            }

            var dict = new RecursiveDictionary<V>(
                HtmlUtils.HTML_HEADERS[curHeader - 1].SplitToDictionary(
                    text, k => thisName == "Root" ? k.Trim() : ZachRGX.SYMBOLS.Replace(HttpUtility.HtmlDecode(k), "").ToLower(), thisName
                ).ToDictionary(
                    kv => kv.Key,
                    kv => (V)ParseText(kv.Value, curHeader, kv.Key)
                )
            );
            return dict.IsNull() ? nullInterface : dict;
        }
#endregion

        #region IRecursiveDictionary Support
        public void SetDict(IDictionary<string, V> dict)
        {
            dictionary = new SortedDictionary<string, V>(dict);
        }

        public void SetDict(IEnumerable<KeyValuePair<string, IRecursiveDictionaryValue>> keyValues)
        {
            dictionary = new SortedDictionary<string, V>(
                keyValues.Where(
                    kv => !(String.IsNullOrWhiteSpace(kv.Key) || kv.Value.IsNull())
                ).Distinct(
                    new KeyValuePairComparer<string, IRecursiveDictionaryValue>()
                ).ToDictionary(
                    kv => kv.Key,
                    kv => (V)kv.Value
                )
            );
        }

        public V AsValue()
        {
            V value = new V();
            value.SetValues(Dictionaries, Collections);
            return value;
        }
        #endregion

        #region IEquatable Support
        public bool Equals(IRecursiveDictionaryValue obj)
        {
            if (!obj.hasSubvalues)
                return this.AsValue().Equals(obj);
            else
            {
                var dict = (IRecursiveDictionary<IRecursiveDictionaryValue>)obj;
                return dict.Count == this.Count &&
                    dict.Keys.OrderBy().SequenceEqual(this.Keys.OrderBy()) &&
                    this.AsValue().Equals(dict.AsValue());
            }
        }
#endregion

        #region IDictionary Support
        public V this[string key] { get => dictionary[key]; set => dictionary[key] = value; }

        public ICollection<string> Keys => dictionary.Keys;

        public ICollection<V> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly => ((IDictionary<string, V>)dictionary).IsReadOnly;

        private SortedDictionary<string, V> dictionary { get; set; }

        public void Add(string key, V value)
        {
            dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, V> item)
        {
            dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, V> item)
        {
            return dictionary.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, V>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, V>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, V> item)
        {
            return ((IDictionary<string, V>)dictionary).Remove(item);
        }

        public bool TryGetValue(string key, out V value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
        #endregion

        #region IDisposable Support
        private void Dispose(bool disposing)
        {
            if (!disposedValue && !this.IsNull())
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    var values = Values.ToArray();
                    foreach (var value in Values)
                    {
                        value.Dispose();
                    }
                    values = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                dictionary.Clear();
                dictionary = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RecursiveDictionary() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }

    public struct RecursiveDictionaryValue : IRecursiveDictionaryValue
    {
        #region Properties
        private const string SERIALIZATION_FORMAT =
            "\"{0}\":{" +
                "\"Collections\":{1}," +
                "\"Dictionaries\":{2}" +
            "}";

        public bool hasSubvalues { get; set; }
        public string Name { get; private set; }

        // Key is whether IsNull has already been ran. Value is the result.
        private KeyValuePair<bool, bool> isNull { get; set; }

        public Dictionary<string, IEnumerable<KeyValuePair<string, string>>> Dictionaries { get; private set; }

        public Dictionary<string, IEnumerable<string>> Collections { get; private set; }
#endregion

        public RecursiveDictionaryValue(string text, string thisName = "Root") : this()
        {
            this.ParseText(text);
            Name = thisName;
        }

        #region IRecursiveDictionaryValue Support
        public string Serialize()
        {
            return String.Format(
                SERIALIZATION_FORMAT,
                Name,
                Collections == null ? "{}" : JSON.Serialize(Collections),
                Dictionaries == null ? "{}" : JSON.Serialize(Dictionaries)
            );
        }

        public bool IsNull()
        {
            if (isNull.Key)
                return isNull.Value;

            if (Collections != null)
            {
                string[] collections = Collections.Keys.ToArray();
                foreach (string key in collections)
                {
                    if (!String.IsNullOrWhiteSpace(key))
                    {
                        var collection = Collections[key];
                        if (collection != null)
                        {
                            collection = collection.Where(c => !String.IsNullOrWhiteSpace(c));
                            if (collection.Any())
                            {
                                Collections[key] = collection;
                                continue;
                            }
                        }
                    }

                    Collections.Remove(key);
                }
                collections = null;
                if (!Collections.Any())
                    Collections = null;
            }

            if (Dictionaries != null)
            {
                string[] dictionaries = Dictionaries.Keys.ToArray();
                foreach (string key in dictionaries)
                {
                    if (!String.IsNullOrWhiteSpace(key))
                    {
                        var dictionary = Dictionaries[key];
                        if (dictionary != null)
                        {
                            dictionary = dictionary.Where(
                                kv => !String.IsNullOrWhiteSpace(kv.Key) &&
                                !String.IsNullOrWhiteSpace(kv.Value)
                            );

                            if (dictionary.Any())
                            {
                                Dictionaries[key] = dictionary;
                                continue;
                            }
                        }
                    }

                    Dictionaries.Remove(key);
                }

                dictionaries = null;
                if (!Dictionaries.Any())
                    Dictionaries = null;
            }

            isNull = new KeyValuePair<bool, bool>(true, Dictionaries != null || Collections != null);
            return isNull.Value;
        }

        public void SetValues(IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, string>>>> dicts, IEnumerable<KeyValuePair<string, IEnumerable<string>>> lists)
        {
            Dictionaries = dicts.ToDictionary();
            Collections = lists.ToDictionary();
        }

        #region ParseText
        public void ParseText(string text)
        {
            ParseText(text, HTMLContent.Links | HTMLContent.ListElements | HTMLContent.Paragraphs | HTMLContent.Tooltips);
        }

        public void ParseText(string text, HTMLContent content)
        {
            string[] array = content.GetFlags().Select(f => f.ToString()).ToArray();
            foreach (string flag in array)
            {
                if (HtmlUtils.HTML_REGEXES[flag].Key)
                {
                    var keyValues = HtmlUtils.HTML_REGEXES[flag].Value.FromKeyValues(text).Distinct(new KeyValuePairComparer<string, string>());
                    if (keyValues.Any())
                    {
                        if (Dictionaries == null)
                            Dictionaries = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>() { { flag, keyValues } };
                        else if (!Dictionaries.TryAdd(flag, keyValues, out IEnumerable<KeyValuePair<string, string>> existingValues))
                            Dictionaries[flag] = keyValues.Concat(existingValues).Distinct(new KeyValuePairComparer<string, string>());
                    }
                }
                else
                {
                    var values = HtmlUtils.HTML_REGEXES[flag].Value.MatchesValues(text).Distinct();
                    if (values.Any())
                    {
                        if (Collections == null)
                            Collections = new Dictionary<string, IEnumerable<string>>() { { flag, values } };
                        else if (!Collections.TryAdd(flag, values, out IEnumerable<string> existingValues))
                            Collections[flag] = values.Concat(existingValues).Distinct();
                    }
                }
            }
            array = null;
        }
        #endregion
        #endregion

        #region IEquatable Support
        public bool Equals(IRecursiveDictionaryValue obj)
        {
            if (obj.hasSubvalues)
                return this.Equals(((IRecursiveDictionary<IRecursiveDictionaryValue>)obj).AsValue());

            if (obj.Dictionaries.Count == this.Dictionaries.Count &&
                obj.Collections.Count == this.Collections.Count)
            {
                var thisDicts = this.Dictionaries.Keys.AsEnumerable().OrderBy();
                var thisLists = this.Collections.Keys.AsEnumerable().OrderBy();

                if (obj.Dictionaries.Keys.OrderBy().SequenceEqual(thisDicts) &&
                    obj.Collections.Keys.OrderBy().SequenceEqual(thisLists)
                )
                {
                    var collections = this.Collections;
                    var dictionaries = this.Dictionaries;
                    return thisLists.All(
                        l => collections[l].Count() == obj.Collections[l].Count() &&
                            collections[l].SequenceEqual(obj.Collections[l])
                    ) && thisDicts.All(
                        d => dictionaries[d].Count() == obj.Dictionaries[d].Count() &&
                            dictionaries[d].SequenceEqual(obj.Dictionaries[d])
                    );
                }
            }

            return false;
        }
        #endregion

        #region IDisposable Support
        public void Dispose()
        {
            if (Dictionaries != null)
            {
                Dictionaries.Clear();
                Dictionaries = null;
            }

            if (Collections != null)
            {
                Collections.Clear();
                Collections = null;
            }
        }
#endregion
    }
}
