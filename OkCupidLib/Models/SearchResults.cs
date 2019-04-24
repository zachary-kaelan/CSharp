using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class SearchResults<T>
    {
        public T[] data { get; protected set; }
        public PagingData paging { get; protected set; }
        public int total_matches { get; protected set; }
        public string search_key { get; protected set; }
    }

    public class PagingData
    {
        public Cursors cursors { get; protected set; }
        public bool end { get; protected set; }
        public int total { get; protected set; }
    }

    public class Cursors
    {
        public string after { get; protected set; }
        public string before { get; protected set; }
        public string current { get; protected set; }
    }

    public class SearchResultsEnumerator<T> : IEnumerator<T>
    {
        private SearchResults<T> CurResults { get; set; }
        private int Index { get; set; }
        public T Current => throw new NotImplementedException();
        object IEnumerator.Current => throw new NotImplementedException();

        public SearchResultsEnumerator(SearchResults<T> results)
        {
            CurResults = results;
            Index = -1;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            ++Index;

        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
