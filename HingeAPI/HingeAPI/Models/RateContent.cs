using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HingeAPI.Models
{
    public abstract class BaseRateContent
    {
        public string comment { get; protected set; }

        public BaseRateContent(string comment)
        {
            this.comment = comment;
        }
    }

    public sealed class PromptContent : BaseRateContent
    {
        public Prompt prompt { get; private set; }

        public PromptContent(string comment, Prompt prompt) : base(comment)
        {
            this.prompt = prompt;
        }

        public sealed class Prompt
        {
            public string answer { get; private set; }
            public string question { get; private set; }
        }
    }
}
