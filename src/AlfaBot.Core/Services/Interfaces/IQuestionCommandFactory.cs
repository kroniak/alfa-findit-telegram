using System;
using Telegram.Bot.Types;
using User = AlfaBot.Core.Models.User;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IQuestionCommandFactory
    {
        Action EndQuestionCommand(long chatId, int messageId);
        
//        Action QuestionCommand(QuizResult result);
        
        Action AddQuizCommand(User user, Message message);
    }
}