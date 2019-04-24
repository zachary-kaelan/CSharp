using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections;

namespace WargamingAPI.XMLModels
{
    public class XMLDictionary<TValue> : IXmlSerializable, IDictionary<string, TValue>
    {
        #region IDictionary Implementation
        public TValue this[string key] { get => _dictionary[key]; set => _dictionary[key] = value; }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => ((IDictionary<string, TValue>)_dictionary).IsReadOnly;

        private Dictionary<string, TValue> _dictionary { get; set; }

        public void Add(string key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            ((IDictionary<string, TValue>)_dictionary).Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<string, TValue>)_dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            return ((IDictionary<string, TValue>)_dictionary).Remove(item);
        }

        public bool TryGetValue(string key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, TValue>)_dictionary).GetEnumerator();
        }
        #endregion

        public XmlSchema GetSchema() => null;

        public void WriteXml(XmlWriter writer) { }

        public void ReadXml(XmlReader reader)
        {
            _dictionary = new Dictionary<string, TValue>();
            var t = typeof(TValue);
            if (t.IsPrimitive || t == typeof(string))
            {
                while (reader.Read())
                {
                    _dictionary.Add(reader.Name, (TValue)Convert.ChangeType(reader.ReadContentAsObject(), t));
                }
            }
            else
            {
                while (reader.Read())
                {
                    XmlSerializer serializer = new XmlSerializer(t, new XmlRootAttribute(reader.Name));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        StreamWriter sw = new StreamWriter(ms);
                        sw.Write(reader.ReadOuterXml());
                        ms.Position = 0;
                        _dictionary.Add(reader.Name, (TValue)serializer.Deserialize(ms));
                    }
                }
            }
        }
    }
}
