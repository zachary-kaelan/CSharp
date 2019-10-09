using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingLib
{
    public class PriorityQueue<T, P> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection 
        where T : IPriorityItem<P>, IEquatable<T>
        where P : IComparable<P>
    {
        private List<T> _list;
        private int _tail;
        static List<T> _emptyList = new List<T>(0);

        public PriorityQueue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            _list = new List<T>(capacity);
            _tail = 0;
        }

        public PriorityQueue() =>
            _list = _emptyList;

        public PriorityQueue(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _list = new List<T>(collection.OrderByDescending(e => e.Priority));
            _tail = _list.Count;
        }

        public T Peek() =>
            _list.Count > 0 ?
                _list[_tail] :
                default(T);

        public T Dequeue()
        {
            if (_list.Count == 0)
                return default(T);
            var element = _list[0];
        }

        public bool Enqueue(T item)
        {
            int insertIndex = -1;
            int prevIndex = -1;
            bool lastHigherPriority = true;

            for (int i = _tail; i >= 0; --i)
            {
                var element = _list[i];
                var comparison = element.Priority.CompareTo(item.Priority);
                if (lastHigherPriority && comparison > 0)
                {
                    insertIndex = i;
                    lastHigherPriority = false;
                    if (prevIndex >= 0)
                        return false;
                    else if (element.Equals(item))
                    {
                        prevIndex = i;
                        break;
                    }
                }
                else if (prevIndex < 0 && element.Equals(item))
                {
                    prevIndex = i;
                    if (!lastHigherPriority)
                    {

                    }
                    else
                        break;
                }
            }

            if (prevIndex >= 0)
                _list.RemoveAt(prevIndex);
            _list.Insert(insertIndex, item);
            return true;
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
