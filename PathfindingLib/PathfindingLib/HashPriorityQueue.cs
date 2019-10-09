using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingLib
{
    public class HashPriorityQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection
        where T : IPriorityItem<P>, IEquatable<T>
        where P : IComparable<P>
    {
        private bool[] _containsHash;
        private List<T> _list;
        private int _head;
        private int _tail;
        private int _size;

        static List<T> _emptyList = new List<T>(0);

        public HashPriorityQueue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            _list = new List<T>(capacity);
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        public HashPriorityQueue() =>
            _list = _emptyList;

        public HashPriorityQueue(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _list = new List<T>(_DefaultCapacity);
            _size = 0;

            using (IEnumerator<T> en = collection.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    Enqueue(en.Current);
                }
            }
        }

        public bool Enqueue(T item)
        {
            int insertIndex = -1;
            int prevIndex = -1;
            bool lastGreaterThan = true;
            for (int i = 0; i < _list.Count; ++i)
            {
                var element = _list[i];
                var comparison = element.Priority.CompareTo(item.Priority);
                if (lastGreaterThan && comparison < 0)
                {
                    insertIndex = i;
                    lastGreaterThan = false;
                    if (prevIndex >= 0)
                        break;
                    else if (element.Equals(item))
                    {
                        prevIndex = i;
                        break;
                    }
                }
                else if (prevIndex < 0 && element.Equals(item))
                {
                    prevIndex = i;
                    if (!lastGreaterThan)

                }
            }
        }

        #region Interface Implementations
        public int Count => _list.Count;

        public object SyncRoot => ((ICollection)_list).SyncRoot;

        public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

        public IEnumerator<T> GetEnumerator() =>
            ((IEnumerable<T>)_list).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _list.GetEnumerator();

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }
        #endregion
    }
}
