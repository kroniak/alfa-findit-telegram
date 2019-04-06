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

        public QueueMessage AskStartQuizMessage => new QueueMessage(ChatId, MessageId)
        {
            Text = QuizMessageDictionary.StartMessage,
            ReplyMarkup = BotHelper.GetKeyboardQuizOrNot()
        };

        public QueueMessage AskStartQuizMessageAgain => new QueueMessage(ChatId, MessageId)
        {
            Text = QuizMessageDictionary.StartMessageAgain,
            ReplyMarkup = BotHelper.GetKeyboardYesOrNo()
        };

        public QueueMessage AskIsStudentMessage => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.IsStudentMessage,
            ReplyMarkup = BotHelper.GetKeyboardYesOrNo()
        };

        public QueueMessage AskProfessionMessage => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.ProfessionMessage,
            ReplyMarkup = BotHelper.GetKeyboardForProfession()
        };

        public QueueMessage AskUniversityMessage => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.UniversityMessage,
            ReplyMarkup = BotHelper.GetRemoveKeyboard()
        };

        public QueueMessage EndMessage => new QueueMessage(ChatId, MessageId, false)
        {
            Text = GeneralMessageDictionary.EndMessage,
            ReplyMarkup = BotHelper.GetRemoveKeyboard()
        };

        public QueueMessage AskCourse => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.CourseMessage,
            ReplyMarkup = BotHelper.GetKeyboardForCourse()
        };

        public QueueMessage AskEmail => new QueueMessage(ChatId, MessageId)
        {
            Text = GeneralMessageDictionary.EmailMessage,
            ReplyMarkup = BotHelper.GetRemoveKeyboard()
        };
    }
}