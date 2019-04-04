using System.Collections.Generic;
using AlfaBot.Core.Models;

namespace AlfaBot.Core.Data.Interfaces
{
    public interface IQuizResultRepository
    {
        IEnumerable<QuizResult> All();
        
        QuizResult AddUserQuiz(User user);

        QuizResult GetResult(long chatId);

        void UpdateQuestionsForUser(QuizResult result);

        void SaveQuestionForUser(QuizResult result, QuestionAnswer answer);
        
        void UpdateTimeForUser(QuizResult result);
    }
}