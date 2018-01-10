using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZachLib
{
    public class KeyValuePairComparer<T, V> : EqualityComparer<KeyValuePair<T, V>>
    {
        public override bool Equals(KeyValuePair<T, V> x, KeyValuePair<T, V> y)
        {
            return x.Key.Equals(y.Key);
        }

        public override int GetHashCode(KeyValuePair<T, V> obj)
        {
            return obj.GetHashCode();
        }
    }

    public class RegexKeyValueComparer : IEqualityComparer<GroupCollection>
    {
        public bool Equals(GroupCollection x, GroupCollection y)
        {
            return x["Key"].Value == y["Key"].Value;
        }

        public int GetHashCode(GroupCollection obj)
        {
            return obj.GetHashCode();
        }
    }
}
