using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace RedditSharpLib.Things
{
    public interface ICreated
    {
        DateTime CreatedAt { get; }
        DateTimeOffset CreatedAt_UTC { get; }
    }
}
