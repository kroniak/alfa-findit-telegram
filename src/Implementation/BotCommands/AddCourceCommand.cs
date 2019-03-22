using System;
using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class AddCourceCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;
        private Message _message;

        public AddCourceCommand(TelegramBotClient botClient, long chatId, Message message)
        {
            _botClient = botClient;
            _chatId = chatId;
            _message = message;
        }

        public async void Execute()
        {
            var course = _message.Text;

            bool? isYoung = null;
            bool? isAnsweredAllQuestions = null;

            if (string.Compare(course, "1-3", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                isYoung = true;
                isAnsweredAllQuestions = true;
            }

            if (string.Compare(course, "4 и старше", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                isYoung = false;
                isAnsweredAllQuestions = true;
            }

            await MongoDBHelperUser.SaveCourse(_chatId, isYoung.HasValue ? course : null, isAnsweredAllQuestions);
            if (isYoung.HasValue)
            {
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.EndOfAskingMessage, replyMarkup: BotHelper.GetRemoveKeyboard());
                if (!isYoung.Value)
                {
                    await _botClient.SendTextMessageAsync(_chatId, MessageFactory.OpenDoorsInvitationMessage);
                }
            }
            else
            {
            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskCourseMessage,
                replyMarkup: BotHelper.GetKeyboardForCourse());
            }
        }
    }
}