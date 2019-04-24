using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    public struct CSVKeyValuePair<K, V> : IEquatable<CSVKeyValuePair<K, V>>, IComparable<CSVKeyValuePair<K, V>> 
        where K : IComparable<K> where V : IComparable<V>
    {
        public K Key { get; set; }
        public V Value { get; set; }

        public CSVKeyValuePair(K key, V value)
        {
            Key = key;
            Value = value;
        }

        private const string TOSTRING_FORMAT = "[{0},{1}]";
        public override string ToString()
        {
            return String.Format(TOSTRING_FORMAT, Key.ToString(), Value.ToString());
        }

        public bool Equals(CSVKeyValuePair<K, V> other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return Key.Equals(other.Key) && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return (Key == null ? 0 : Key.GetHashCode()) ^ (Value == null ? 0 : Value.GetHashCode());
        }

        public int CompareTo(CSVKeyValuePair<K, V> other)
        {
            int result = (Key.CompareTo(other.Key));
            if (result != 0)
                return Value.CompareTo(other.Value);
            return result;
        }
    }
}
