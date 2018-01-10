using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using RGX.Examine;
using RGX.HTML.HEADERS;
using RGX.Psychonauts;

namespace ZachLib
{
    public static class HtmlUtils
    {
        public static readonly Regex[] HTML_HEADERS = new Regex[]
        {
            null,
            new Header1(),
            new Header2(),
            new Header3(),
            new Header4(),
            new Header5(),
            new Header6()
        };

        public static readonly Dictionary<string, KeyValuePair<bool, Regex>> HTML_REGEXES = new Dictionary<string, KeyValuePair<bool, Regex>>()
        {
            { "Tooltips", new KeyValuePair<bool, Regex>(true, HtmlUtils.EXAMINE_TOOLTIPS) },
            { "Paragraphs", new KeyValuePair<bool, Regex>(false, ZachRGX.HTML_PARAGRAPHS )},
            { "Links", new KeyValuePair<bool, Regex>(true, ZachRGX.HTML_LINKS) },
            { "ListElements", new KeyValuePair<bool, Regex>(false, HtmlUtils.PSYCHONAUTS_EFFECTS_INDEX) }
        };

        public static readonly Tooltips EXAMINE_TOOLTIPS = new Tooltips();

        public static readonly SubjectiveEffectsIndex PSYCHONAUTS_EFFECTS_INDEX = new SubjectiveEffectsIndex();

        /*Examine
        public static IEnumerable<string> EXAMINE_INTERACTIONS(string text)
        {
            return PPRGX.PARAGRAPHS.MatchesValues(text).Select(
                m => PPRGX.HTML_TAGS.Replace(m, "")
            );
        }

        public static Headers SplitByHeader(string text, int header)
        {
            return new Headers(
                HTML_HEADERS[header].MatchesValues(text, "Header").ZipToDictionary(
                    HTML_HEADERS[header].Split(text).Skip(1),
                    k => k.Trim(),
                    v => (IHeader)(new Header(v))
                )
            );
        }

        public static IHeader SplitByHeader(string text)
        {
            return SplitByHeader(text, 1, "Root");
        }

        private static IHeader SplitByHeader(string text, int curHeader, string thisName = null)
        {
            bool isMatch = false;
            do
            {
                isMatch = HTML_HEADERS[curHeader++].IsMatch(text);
            } while (!isMatch && curHeader <= 6);

            if (curHeader == 7 && !isMatch)
            {
                IHeader tempHeader = (IHeader)(new Header(text));
                return tempHeader.Paragraphs.Any() ||
                    tempHeader.Tooltips.Any() ||
                    tempHeader.Links.Any() ?
                    tempHeader : null;
            }

            return new Headers(
                HTML_HEADERS[curHeader - 1].SplitToDictionary(
                    text, k => thisName == "Root" ? k.Trim() : PPRGX.SYMBOLS.Replace(HttpUtility.HtmlDecode(k), "").ToLower(), thisName
                ).ToDictionary(
                    kv => kv.Key,
                    kv => SplitByHeader(kv.Value, curHeader, kv.Key)
                )
            );
        }

        public static DictType HeadersDictionary<DictType, NonDictType>(string text)
            where NonDictType : IRecursiveDictionaryValue, new()
            where DictType : IRecursiveDictionary<NonDictType>, new()
        {
            return (DictType)HeadersDictionary<DictType, NonDictType>(text, 1, "Root");
        }

        private static IRecursiveDictionaryValue HeadersDictionary<DictType, NonDictType>(string text, int curHeader, string thisName)
            where NonDictType : IRecursiveDictionaryValue, new()
            where DictType : IRecursiveDictionary<NonDictType>, new()
        {
            bool isMatch = false;
            do
            {
                isMatch = HTML_HEADERS[curHeader++].IsMatch(text);
            } while (!isMatch && curHeader <= 6);

            if (curHeader == 7 && !isMatch)
            {
                IRecursiveDictionaryValue tempHeader = (IRecursiveDictionaryValue)(new NonDictType());
                return tempHeader.IsNull() ? null : tempHeader;
            }

            DictType dict = new DictType();
            dict.SetDict(
                HTML_HEADERS[curHeader - 1].SplitToDictionary(
                    text, k => thisName == "Root" ? k.Trim() : PPRGX.SYMBOLS.Replace(HttpUtility.HtmlDecode(k), "").ToLower(), thisName
                ).ToDictionary(
                    kv => kv.Key,
                    kv => HeadersDictionary<DictType, NonDictType>(kv.Value, curHeader, kv.Key)
                )
            );
            return dict;
        }
        
        #region Examine
        public struct Header : IHeader
        {
            public IEnumerable<string> Paragraphs { get; private set; }
            public Dictionary<string, string> Tooltips { get; private set; }
            public IEnumerable<KeyValuePair<string, string>> Links { get; private set; }
            public bool HasSubheaders { get { return false; } }

            public Header(IEnumerable<string> paragraphs, Dictionary<string, string> tooltips, IEnumerable<KeyValuePair<string, string>> links)
            {
                Paragraphs = paragraphs == null ? Enumerable.Empty<string>() : paragraphs;
                Tooltips = tooltips == null ? new Dictionary<string, string>() : tooltips;
                Links = links == null ? Enumerable.Empty<KeyValuePair<string, string>>() : links;
                disposedValue = false;
            }

            public Header(string text)
            {
                Paragraphs = PPRGX.PARAGRAPHS.MatchesValues(text).Select(
                    p => HttpUtility.HtmlDecode(
                        PPRGX.HTML_TAGS.Replace(p, "")
                    ).Trim()
                );
                Tooltips = PPRGX.EXAMINE_TOOLIPS.Matches(text).ToDictionary();
                Links = PPRGX.HTML_LINKS.Matches(text).Cast<Match>().ToKeyValues();
                disposedValue = false;
            }

            public bool IsNull()
            {
                return !(Paragraphs.Any() ||
                    Tooltips.Any() ||
                    Links.Any());
            }

            private bool disposedValue { get; set; }

            public void Dispose()
            {
                this.Dispose(true);
            }

            public void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    disposedValue = true;
                    Tooltips.Clear();

                    if (disposing)
                    {
                        Tooltips = null;
                        Paragraphs = null;
                        Links = null;
                    }
                }
            }
        }

        public struct Headers : IDictionary<string, IHeader>, IHeader
        {
            public IHeader this[string key] { get => headers[PPRGX.SYMBOLS.Replace(key, "").ToLower()]; set => headers[key] = value; }
            public IHeader this[string key, bool removeSymbols] { get => headers[removeSymbols ? PPRGX.SYMBOLS.Replace(key, "").ToLower() : key]; }

            public bool HasSubheaders { get { return true; } }

            private SortedDictionary<string, IHeader> headers { get; set; }

            public IEnumerable<string> Paragraphs { get { return Values.Where(v => v != null).SelectMany(h => h.Paragraphs); } }
            public Dictionary<string, string> Tooltips { get { return Values.Where(v => v != null).SelectMany(h => h.Tooltips).Distinct(new KeyValuePairComparer<string, string>()).ToDictionary(t => t.Key, t => t.Value); } }
            public IEnumerable<KeyValuePair<string, string>> Links { get { return Values.Where(v => v != null).SelectMany(h => h.Links); } }

            public Headers(IDictionary<string, IHeader> dict)
            {
                headers = null;
                disposedValue = false;
                SetDictionary(dict);
            }

            public void SetDictionary(IDictionary<string, IHeader> dict)
            {
                headers = new SortedDictionary<string, IHeader>(dict);
            }

            #region IDictionary Support
            public ICollection<string> Keys => headers.Keys;

            public ICollection<IHeader> Values => headers.Values;

            public int Count => headers.Count;

            public bool IsReadOnly => ((IDictionary<string, IHeader>)headers).IsReadOnly;

            public void Add(string key, IHeader value)
            {
                headers.Add(key, value);
            }

            public void Add(KeyValuePair<string, IHeader> item)
            {
                ((IDictionary<string, IHeader>)headers).Add(item);
            }

            public void Clear()
            {
                headers.Clear();
            }

            public bool Contains(KeyValuePair<string, IHeader> item)
            {
                return headers.Contains(item);
            }

            public bool ContainsKey(string key)
            {
                return headers.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<string, IHeader>[] array, int arrayIndex)
            {
                ((IDictionary<string, IHeader>)headers).CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<string, IHeader>> GetEnumerator()
            {
                return headers.GetEnumerator();
            }

            public bool Remove(string key)
            {
                return headers.Remove(key);
            }

            public bool Remove(KeyValuePair<string, IHeader> item)
            {
                return ((IDictionary<string, IHeader>)headers).Remove(item);
            }

            public bool TryGetValue(string key, out IHeader value)
            {
                return headers.TryGetValue(PPRGX.SYMBOLS.Replace(key, "").ToLower(), out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return headers.GetEnumerator();
            }
            #endregion

            #region IDisposable Support
            private bool disposedValue { get; set; }

            public void Dispose()
            {
                this.Dispose(true);
            }

            public void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    disposedValue = true;

                    if (disposing)
                    {
                        IHeader[] headersTemp = Values.Where(v => v != null).ToArray();
                        foreach (IHeader header in headersTemp)
                            header.Dispose();
                        headersTemp = null;
                    }

                    headers.Clear();
                    headers = null;
                }
            }
            #endregion
        }
        #endregion

        public interface IHeader : IDisposable
        {
            IEnumerable<string> Paragraphs { get; }
            Dictionary<string, string> Tooltips { get; }
            IEnumerable<KeyValuePair<string, string>> Links { get; }
            bool HasSubheaders { get; }
        }*/
    }
}
