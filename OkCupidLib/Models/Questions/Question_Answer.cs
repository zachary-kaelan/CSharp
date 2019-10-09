using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models.Questions
{
    public class Question_Answer
    {
        public Question question { get; private set; }
        public Answer target { get; private set; }
        public Answer viewer { get; private set; }
    }
}
