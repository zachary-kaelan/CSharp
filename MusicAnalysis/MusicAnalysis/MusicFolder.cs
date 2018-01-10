using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MusicAnalysis
{
    public class MusicFolder : IDictionary<string, StorageItemContentProperties>
    {
        public StorageItemContentProperties this[string key] { get => dictionary[key]; set => dictionary[key] = value; }

        public ICollection<string> Keys => dictionary.Keys;

        public ICollection<StorageItemContentProperties> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly => ((IDictionary<string, StorageItemContentProperties>)dictionary).IsReadOnly;

        private SortedDictionary<string, StorageItemContentProperties> dictionary { get; set; }

        public MusicFolder()
        {
            dictionary = new SortedDictionary<string, StorageItemContentProperties>();
        }

        public MusicFolder(IDictionary<string, StorageItemContentProperties> dict)
        {
            dictionary = new SortedDictionary<string, StorageItemContentProperties>(dict);
        }

        public MusicFolder(string dirName) : this(
            Directory.GetFiles(dirName).ToDictionary(
                f => f,
                f => StorageFile.GetFileFromPathAsync(f).GetResults().Properties
            )
        ) { }

        #region IDictionary Support
        public void Add(string key, StorageItemContentProperties value)
        {
            dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, StorageItemContentProperties> item)
        {
            ((IDictionary<string, StorageItemContentProperties>)dictionary).Add(item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, StorageItemContentProperties> item)
        {
            return dictionary.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, StorageItemContentProperties>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, StorageItemContentProperties>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, StorageItemContentProperties> item)
        {
            return ((IDictionary<string, StorageItemContentProperties>)dictionary).Remove(item);
        }

        public bool TryGetValue(string key, out StorageItemContentProperties value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
#endregion
    }
}
