using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models.Questions
{
    public enum QuestionGenre
    {
        dating,
        ethics,
        lifestyle,
        other,
        religion,
        sex
    }

    public enum AnswerFilters
    {
        None,
        Public = 1,
        Private = 2,
        Skipped = 3,
        Important = 4,
        SomewhatImportant = 5,
        LittleSomewhatImportant = 6,
        LittleImportant = 7,
        Explained = 8,
        Agree = 9,
        Disagree = 10,
        Find_Out = 11
    }
}
