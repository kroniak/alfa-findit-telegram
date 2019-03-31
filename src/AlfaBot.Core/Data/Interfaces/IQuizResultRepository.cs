using System.Collections.Generic;
using AlfaBot.Core.Models;
using MongoDB.Driver;

namespace AlfaBot.Core.Data.Interfaces
{
    public interface IQuizResultRepository
    {
        IEnumerable<QuizResult> All();
        
        void AddUserQuiz(User user);

        QuizResult GetResult(long chatId);

        UpdateResult SaveQuestionForUser(long chatId, QuestionAnswer answer);

        UpdateResult CalcPoints(long chatId);

        UpdateResult UpdateEnd(long chatId);

        int GetAnsweredCount(long chatId);
    }
}