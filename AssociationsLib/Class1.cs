using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociationsLib
{
    public class Association<TKey, TValues>
    {
        public TKey Key { get; private set; }
        public SortedDictionary<TValues, double> Values { get; private set; }
        
        public Association(TKey key)
        {
            Key = key;
            Values = new SortedDictionary<TValues, double>();
        }

        public Association(TKey key, IDictionary<TValues, double> values)
        {
            Key = key;
            Values = new SortedDictionary<TValues, double>(values);
        }
    }
}
