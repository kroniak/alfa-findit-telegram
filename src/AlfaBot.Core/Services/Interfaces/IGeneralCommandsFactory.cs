using System;
using AlfaBot.Core.Models;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IGeneralCommandsFactory
    {
        Action StartCommand(QueueMessage nextMessage);

        Action AddContactCommand(Message message, QueueMessage nextMessage);

        Action AddNameCommand(Message message, QueueMessage nextMessage);

        Action AddEMailCommand(Message message, QueueMessage nextMessage);

        Action AddProfessionCommand(Message message, QueueMessage nextMessage);

        Action AddIsStudentCommand(Message message, QueueMessage nextTrueMessage, QueueMessage nextFalseMessage);

        Action AddUniversityCommand(Message message, QueueMessage nextMessage);

        Action AddCourseCommand(Message message, QueueMessage nextTrueMessage, QueueMessage nextFalseMessage);

        Action Command(QueueMessage queueMessage);
    }
}