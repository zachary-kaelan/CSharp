using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    public class Day : Thing//, IList<Block>
    {
        public DayOfWeek DayOfWeek { get; private set; }
        internal List<int> Blocks { get; set; }
        internal int Pointer { get; set; }
        internal List<int> Backtracking { get; private set; }
        internal bool FullyBacktracked { get; set; }
        public bool ExcessPreceeding { get; set; }

        private static int ID_COUNTER = 0;

        public Day(DayOfWeek day) : base(ID_COUNTER, day.ToString().Substring(0, 3))
        {
            DayOfWeek = day;
            ++ID_COUNTER;
            Blocks = new List<int>();
            Pointer = 0;
            Backtracking = new List<int>();
        }

        /*
        #region IList<Block> Implementation
        public int Count => Blocks.Count;

        public bool IsReadOnly => ((IList<Block>)Blocks).IsReadOnly;

        public Block this[int index] { get => Blocks[index]; set => Blocks[index] = value; }

        public int IndexOf(Block item)
        {
            return Blocks.IndexOf(item);
        }

        public void Insert(int index, Block item)
        {
            Blocks.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Blocks.RemoveAt(index);
        }

        public void Add(Block item)
        {
            Blocks.Add(item);
        }

        public void Clear()
        {
            Blocks.Clear();
        }

        public bool Contains(Block item)
        {
            return Blocks.Contains(item);
        }

        public void CopyTo(Block[] array, int arrayIndex)
        {
            Blocks.CopyTo(array, arrayIndex);
        }

        public bool Remove(Block item)
        {
            return Blocks.Remove(item);
        }

        public IEnumerator<Block> GetEnumerator()
        {
            return Blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Blocks.GetEnumerator();
        }
        #endregion
        */
    }
}
