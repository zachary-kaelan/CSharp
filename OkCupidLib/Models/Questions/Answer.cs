using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models.Questions
{
    public class Answer
    {
        public byte[] accepts { get; private set; }
        public byte answer { get; private set; }
        public string date_answered { get; private set; }
        public ushort id { get; private set; }
        public byte importance { get; private set; }
        public QuestionNote note { get; private set; }
        [Jil.JilDirective("public")]
        public bool is_public { get; private set; }
        public bool skipped { get; private set; }
        public long timestamp_answered { get; private set; }
        public bool viewer_accepts { get; private set; }
        public bool target_accepts { get; private set; }

        public class QuestionNote
        {
            public ulong date_modified { get; private set; }
            public string note { get; private set; }
            public ushort qid { get; private set; }
            public string userid { get; private set; }
        }
    }
}
