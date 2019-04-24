using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3.SimpleGeneticAlgorithm
{
    public class SimpleFullGeneCollection : IReadOnlyCollection<SimpleFullGene>
    {
        private SimpleFullGene[] _genes;

        public SimpleFullGeneCollection(IEnumerable<SimpleFullGene> genes)
        {
            _genes = genes.OrderBy(g => g.Day).ThenBy(g => g.Time).ThenBy(g => g.Dorm).ToArray();
        }

        public int Count => _genes.Length;

        public IEnumerator<SimpleFullGene> GetEnumerator() =>
            ((IEnumerable<SimpleFullGene>)_genes).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _genes.GetEnumerator();

        public SortedSet<byte> DormsReserved(byte day, byte time)
        {
            int min = 0;
            int max = Count;
            int oldMidpoint = 0;
            int midpoint = 0;
            sbyte lastResult = 0;
            var current = new SimpleFullGene();
            do
            {
                oldMidpoint = midpoint;
                midpoint = (min + max) / 2;
                if (midpoint == oldMidpoint)
                    return null;
                current = _genes[midpoint];
                lastResult = current.CompareTo(day, time);
                if (lastResult < 0)
                    min = midpoint;
                else if (lastResult > 0)
                    max = midpoint;
            } while (lastResult != 0);

            var midsectionLength = max - min;
            return new SortedSet<byte>(_genes.Skip(min).Take(midsectionLength).Select(g => g.Dorm));
        }

        public byte ContainsMatch(byte day, byte time, byte dorm, byte otherDorm)
        {
            int min = 0;
            int max = Count;
            int oldMidpoint = 0;
            int midpoint = 0;
            sbyte lastResult = 0;
            var current = new SimpleFullGene();
            do
            {
                oldMidpoint = midpoint;
                midpoint = (min + max) / 2;
                if (midpoint == oldMidpoint)
                    return 0;
                current = _genes[midpoint];
                lastResult = current.CompareTo(day, time);
                if (lastResult < 0)
                    min = midpoint;
                else if (lastResult > 0)
                    max = midpoint;
            } while (lastResult != 0);

            if (otherDorm == 255)
            {
                int veryOldMidpoint = 0;
                do
                {
                    veryOldMidpoint = midpoint;
                    if (current.Dorm > dorm)
                    {
                        max = midpoint;
                        midpoint = (min + max) / 2;
                        if (midpoint == oldMidpoint)
                            return 0;
                        current = _genes[midpoint];

                        while (current.Day != day || current.Time != time)
                        {
                            oldMidpoint = midpoint;
                            min = midpoint;
                            midpoint = (min + max) / 2;
                            if (midpoint == oldMidpoint)
                                return 0;
                            current = _genes[midpoint];
                        }

                    }
                    else if (current.Dorm < dorm)
                    {
                        min = midpoint;
                        midpoint = (min + max) / 2;
                        if (midpoint == oldMidpoint)
                            return 0;
                        current = _genes[midpoint];

                        while (current.Day != day || current.Time != time)
                        {
                            oldMidpoint = midpoint;
                            max = midpoint;
                            midpoint = (min + max) / 2;
                            if (midpoint == oldMidpoint)
                                return 0;
                            current = _genes[midpoint];
                        }
                    }

                    if (midpoint == veryOldMidpoint)
                        return 0;
                } while (current.Dorm != dorm);

                return 1;
            }
            else
            {
                int veryOldMidpoint = 0;
                do
                {
                    veryOldMidpoint = midpoint;
                    if (current.Dorm > dorm && current.Dorm > otherDorm)
                    {
                        max = midpoint;
                        midpoint = (min + max) / 2;
                        if (midpoint == oldMidpoint)
                            return 0;
                        current = _genes[midpoint];

                        while (current.Day != day || current.Time != time)
                        {
                            oldMidpoint = midpoint;
                            min = midpoint;
                            midpoint = (min + max) / 2;
                            if (midpoint == oldMidpoint)
                                return 0;
                            current = _genes[midpoint];
                        }

                    }
                    else if (current.Dorm < dorm && current.Dorm < otherDorm)
                    {
                        min = midpoint;
                        midpoint = (min + max) / 2;
                        if (midpoint == oldMidpoint)
                            return 0;
                        current = _genes[midpoint];

                        while (current.Day != day || current.Time != time)
                        {
                            oldMidpoint = midpoint;
                            max = midpoint;
                            midpoint = (min + max) / 2;
                            if (midpoint == oldMidpoint)
                                return 0;
                            current = _genes[midpoint];
                        }
                    }
                } while (midpoint != veryOldMidpoint);

                bool matchDorm = false;
                bool matchOther = false;

                for (int i = min; i < max; ++i)
                {
                    current = _genes[midpoint];
                    if (current.Dorm == dorm)
                    {
                        matchDorm = true;
                        if (matchOther)
                            return 3;
                    }
                    else if (current.Dorm == otherDorm)
                    {
                        matchOther = true;
                        if (matchDorm)
                            return 3;
                    }
                }

                if (matchDorm)
                    return 1;
                else if (matchOther)
                    return 2;
                else
                    return 0;
            }
        }
    }
}
