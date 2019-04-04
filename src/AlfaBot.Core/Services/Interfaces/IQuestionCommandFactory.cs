using System;
using AlfaBot.Core.Models;
using Telegram.Bot.Types;
using User = AlfaBot.Core.Models.User;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IQuestionCommandFactory
    {
        Action EndQuestionCommand(Message message);

        Action QuestionCommand(QuizResult result, Message message, QueueMessage nextMessage);
        
        Action AddQuizCommand(User user, Message message, QueueMessage nextMessage);
    }
}