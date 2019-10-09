using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models.Questions
{
    public class Question
    {
        public string[] answers { get; private set; }
        public QuestionGenre genre { get; private set; }
        public int id { get; private set; }
        public bool noskip { get; private set; }
        public string text { get; private set; }
    }
}
