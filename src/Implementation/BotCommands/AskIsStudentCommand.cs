using System;
using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class AskIsStudentCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;
        private Message _message;
        
        public AskIsStudentCommand(TelegramBotClient botClient, long chatId, Message message)
        {
            _botClient = botClient;
            _chatId = chatId;
            _message = message;
        }
        
        public async void Execute()
        {
            var answer = _message.Text;
            bool? isStudent = null;
            if (string.Compare(answer, "да", StringComparison.InvariantCultureIgnoreCase) == 0)
                isStudent = true;
            if (string.Compare(answer, "нет", StringComparison.InvariantCultureIgnoreCase) == 0)
                isStudent = false;

            await MongoDBHelper.SaveStudentOrWorkerInfo(_chatId, isStudent, null);

            if (isStudent.HasValue && isStudent.Value)
            {
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskCourseMessage,
                    replyMarkup: BotHelper.GetKeyboardForCourse());
            }
            else
            {
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskIsWorkerMessage,
                    replyMarkup: BotHelper.GetKeyboardYesOrNo());
            }
        }
    }
}