using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace HingeAPI.Models
{
    public enum ProfileOrigin
    {
        potential
    }

    public enum ProfileRating
    {
        skip,
        note,
        like
    }

    public class ProfileRate
    {
        public DateTime created { get; private set; }
        public bool hasPairing { get; private set; }
        public ProfileOrigin origin { get; private set; }
        public ProfileRating rating { get; private set; }
        public string subjectId { get; private set; }

        [JilDirective(Name = "content", IsUnion = true, IsUnionType = true)]
        public Type contentType { get; private set; }

        public ProfileRate(string subjectId)
        {
            this.rating = ProfileRating.skip;
            this.subjectId = subjectId;
            created = DateTime.UtcNow;
        }

        private ProfileRate(string subjectId, ProfileRating rating)
        {
            this.rating = rating;
            this.subjectId = subjectId;
            created = DateTime.UtcNow;
        }

        public sealed class Data
        {
            public bool fullProfileViewed { get; private set; }
            public byte numScrollTaps { get; private set; }
        }
    }
}
