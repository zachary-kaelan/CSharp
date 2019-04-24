using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace RedditSharpLib.Things
{
    public class Listing<T> : IAsyncEnumerable<T> where T : Thing
    {
        /// <summary>
        /// Number of records to return for each request.
        /// </summary>
        public int LimitPerRequest { get; set; }

        /// <summary>
        /// Maximum number of records to return.
        /// </summary>
        public int MaximumLimit { get; set; }

        /// <summary>
        /// Returns true is this a ListingStream.
        /// </summary>
        internal bool IsStream { get; set; }

        private class ListingEnumerator : IAsyncEnumerator<T>
        {
            private Listing<T> Listing { get; set; }
            private string After { get; set; }
            private string Before { get; set; }

            public ListingEnumerator(Listing<T> listing)
            {
                Listing = listing;

            }
        }
    }
}
