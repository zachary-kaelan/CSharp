using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HingeAPI.Models
{
    internal sealed class RateLimitResponse
    {
        public Limit limit { get; private set; }

        public sealed class Limit
        {
            public int likes { get; private set; }
        }
    }

    internal sealed class PromptListResponse
    {
        public ushort[] defaultPromptIndices { get; private set; }
        public Prompt[] prompts { get; private set; }
    }

    public sealed class PotentialCandidate
    {
        public object[] mutualFriends { get; private set; }
        //public string playerId { get; private set; }
        public string subjectId { get; private set; }
    }
}
