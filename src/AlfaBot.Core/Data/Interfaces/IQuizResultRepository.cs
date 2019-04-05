using System.Collections.Generic;
using AlfaBot.Core.Models;

namespace AlfaBot.Core.Data.Interfaces
{
    public interface IQuizResultRepository
    {
        IEnumerable<QuizResult> All();

        IEnumerable<QuizResult> All(int limit);
        
        QuizResult AddUserQuiz(User user);

        QuizResult GetResult(long chatId);

        void UpdateQuestionsForUser(QuizResult result);

        void UpdateTimeForUser(QuizResult result);
    }
}