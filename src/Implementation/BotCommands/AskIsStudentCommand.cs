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
            bool? isAnsweredAllQuestions = null;
            
            if (string.Compare(answer, "да", StringComparison.InvariantCultureIgnoreCase) == 0)
                isStudent = true;

            if (string.Compare(answer, "нет", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                isStudent = false;
                isAnsweredAllQuestions = true;
            }

            await MongoDBHelper.SavePersonOrWorkerInfo(_chatId, isStudent, isAnsweredAllQuestions);

            if (!isStudent.HasValue)
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskIsStudentMessage,
                    replyMarkup: BotHelper.GetKeyboardYesOrNo());

            if (isStudent.HasValue && isStudent.Value)
            {
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskUniversityMessage, replyMarkup: BotHelper.GetRemoveKeyboard());
            }
            else
            {
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.EndOfAskingMessage, replyMarkup: BotHelper.GetRemoveKeyboard());
            }
        }
    }
}