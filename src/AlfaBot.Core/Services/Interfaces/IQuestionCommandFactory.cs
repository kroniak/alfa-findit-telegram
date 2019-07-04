using System;
using AlfaBot.Core.Models;
using Telegram.Bot.Types;
using User = AlfaBot.Core.Models.User;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IQuestionCommandFactory
    {
//        Action EndQuestionCommand(Message message);

        Action QuestionCommand(Message message, QueueMessage nextMessage,
            bool isAnsweredAll);

        Action AddQuizCommand(User user, Message message, QueueMessage nextFalseMessage, QueueMessage wrongMessage);
    }
}