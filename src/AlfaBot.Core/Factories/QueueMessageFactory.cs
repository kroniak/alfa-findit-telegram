using AlfaBot.Core.Factories.Dictionaries;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services.Helpers;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Factories
{
    public class QueueMessageFactory
    {
        private readonly Message _message;

        private long ChatId => _message.Chat.Id;

        private int MessageId => _message.MessageId;

        public QueueMessageFactory(Message message)
        {
            _message = message;
        }

        public QueueMessage WelcomeMessage =>
            new QueueMessage(ChatId, MessageId)
            {
                Text = GeneralMessageDictionary.WelcomeMessage,
                ReplyMarkup = BotHelper.GetKeyBoardForContact()
            };

        public QueueMessage AskNameMessage(string telegramName) => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.NameMessage +
                   (string.IsNullOrWhiteSpace(telegramName) ? "" : "\nВыбери или укажи свой вариант"),
            ReplyMarkup = string.IsNullOrWhiteSpace(telegramName)
                ? BotHelper.GetRemoveKeyboard()
                : BotHelper.GetKeyboardForName(telegramName)
        };

        public QueueMessage WrongMessage => new QueueMessage(ChatId, MessageId, false)
        {
            Text = GeneralMessageDictionary.WrongMessage
        };

        public QueueMessage AskContactMessage => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.ContactMessage,
            ReplyMarkup = BotHelper.GetKeyBoardForContact()
        };

        public QueueMessage AskBetMessage => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.BetMessage,
            ReplyMarkup = BotHelper.GetKeyboardForBet()
        };

        public QueueMessage EndMessage => new QueueMessage(ChatId, MessageId, false)
        {
            Text = GeneralMessageDictionary.EndMessage,
            ReplyMarkup = BotHelper.GetRemoveKeyboard()
        };

        public QueueMessage AskEmail => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.EmailMessage,
            ReplyMarkup = BotHelper.GetRemoveKeyboard()
        };
    }
}