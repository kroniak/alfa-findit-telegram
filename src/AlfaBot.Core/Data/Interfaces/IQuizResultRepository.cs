using System.Collections.Generic;
using AlfaBot.Core.Models;
using MongoDB.Driver;

namespace AlfaBot.Core.Data.Interfaces
{
    public interface IQuizResultRepository
    {
        IEnumerable<QuizResult> All();

        IEnumerable<QuizResult> All(int limit);

        QuizResult Get(long chatId);

        DeleteResult Delete(long chatId);

        QuizResult AddUserQuiz(User user);

        bool IsQuizMember(long chatId);

        QuizResult GetResult(long chatId);

        void UpdateQuestionsForUser(QuizResult result);

        void UpdateTimeForUser(QuizResult result);
    }
}