using System;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IGeneralCommandsFactory
    {
        Action CreateStudentCommand(long chatId);

        Action AddContactCommand(long chatId, Message message);

        Action ContactCommand(long chatId);

        Action AddNameCommand(long chatId, Message message);

        Action AddEMailCommand(long chatId, Message message);

        Action AddProfessionCommand(long chatId, Message message);

        Action IsStudentCommand(long chatId, Message message);

        Action EndCommand(long chatId);

        Action AddUniversityCommand(long chatId, Message message);

        Action AddCourseCommand(long chatId, Message message);

        Action WrongCommand(long chatId);
    }
}