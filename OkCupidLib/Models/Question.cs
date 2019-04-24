using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace OkCupidLib.Models
{
    public enum QuestionCategory
    {
        dating,
        lifestyle,
        religion,
        sex,
        other
    }

    public class BaseQuestionModel
    {
        public int id { get; protected set; }
        public string text { get; protected set; }
    }

    public class Question : BaseQuestionModel
    {
        public string answer { get; protected set; }
        public QuestionCategory category { get; protected set; }
        public string explanation { get; protected set; }
    }

    public class QuestionInfo : BaseQuestionModel
    {
        public string[] answers { get; protected set; }
        public QuestionCategory genre { get; protected set; }
        public byte noskip { get; protected set; }

        public KeyValuePair<int, int> ToIDs(int[] answerIndices)
        {
            int answersID = 0;
            foreach(var answer in answerIndices)
            {
                answersID += ZachMath.IntPow(2, answer + 1);
            }
            return new KeyValuePair<int, int>(id, answersID);
        }
    }

    public class QuestionListModel
    {
        public int questionid { get; protected set; }
        public string question_text { get; protected set; }
    }

}
