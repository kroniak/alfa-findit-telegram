using System;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IGeneralCommandsFactory
    {
        Action CreateStartCommand(long chatId, int messageId);

        Action AddContactCommand(long chatId, Message message);

        Action ContactCommand(long chatId, int messageId);

        Action AddNameCommand(long chatId, Message message);

        Action AddEMailCommand(long chatId, Message message);

        Action AddProfessionCommand(long chatId, Message message);

        Action IsStudentCommand(long chatId, Message message);

        Action EndCommand(long chatId, int messageId);

        Action AddUniversityCommand(long chatId, Message message);

        Action AddCourseCommand(long chatId, Message message);

        Action WrongCommand(long chatId, int messageId);
    }
}