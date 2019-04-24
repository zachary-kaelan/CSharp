using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditSharpLib.Things
{
    internal interface IListing
    {
        string before { get; }
        string after { get; }
        string modhash { get; }
        IEnumerable<IRedditThing> children { get; set; }
    }

    internal interface IListing<TElement>
    {
        string before { get; }
        string after { get; }
        string modhash { get; }
        IEnumerable<TElement> children { get; set; }
    }
}
