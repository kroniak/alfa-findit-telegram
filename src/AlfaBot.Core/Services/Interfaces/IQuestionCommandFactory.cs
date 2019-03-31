using System;
using AlfaBot.Core.Models;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IQuestionCommandFactory
    {
        Action EndQuestionCommand(long chatId);
        
        Action QuestionCommand(QuizResult result);
    }
}