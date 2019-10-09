using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HingeAPI.Models
{
    public sealed class Answer
    {
        public string answerId { get; private set; }
        public string id { get; private set; }
        public byte position { get; private set; }
        public string questionId { get; private set; }
        public string response { get; private set; }
        public string type { get; private set; }
    }
}
