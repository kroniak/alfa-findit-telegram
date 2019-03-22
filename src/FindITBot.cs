using System.Net;
using System.Linq;
using FindAlfaITBot.Implementation.BotCommands;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using FindAlfaITBot.Models;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FindAlfaITBot
{
    public class FindITBot : ITelegramBot
    {
        private readonly TelegramBotClient _botClient;

        public string SecretKey { get; private set; }

        public FindITBot(string token, string secretKey, WebProxy proxy = null)
        {
            _botClient = new TelegramBotClient(token, proxy);
            SecretKey = secretKey;
            Build();
        }

        public FindITBot Build()
        {
            _botClient.OnMessage += OnMessageReceived;

            return this;
        }

        public ITelegramBot Start()
        {
            if (_botClient.IsReceiving) return this;

            _botClient.StartReceiving();
            return this;
        }

        public ITelegramBot Stop()
        {
            _botClient.StopReceiving();
            return this;
        }

        public bool Ping() => _botClient.IsReceiving;

        private void OnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            // Fix other message
            switch (message.Type)
            {
                case MessageType.Contact:
                case MessageType.Text:
                    {
                        CreateCommand(message).Execute();
                        break;
                    }
                default:
                    {
                        new WrongCommand(_botClient, message.Chat.Id);
                        break;
                    }
            }
        }


        private IMessageCommand CreateCommand(Message message)
        {
            var chatId = message.Chat.Id;
            var person = MongoDBHelperQuestion.GetPerson(chatId).Result;
            Result result = MongoDBHelperQuestion.GetResult(chatId).Result;

            if (person == null)
                return new CreateStudentCommand(_botClient, chatId);

            if (message.Type == MessageType.Contact && person.Phone is null)
                return new AddContactCommand(_botClient, chatId, message);

            if (message.Type != MessageType.Contact)
            {
                if (person.Phone == null)
                    return new AskContactCommand(_botClient, chatId);
                if (person.Name == null)
                    return new AddNameCommand(_botClient, chatId, message);
                if (person.EMail == null)
                    return new AddEMailCommand(_botClient, chatId, message);
                if (person.Profession == null)
                    return new AddProfessionCommand(_botClient, chatId, message);
                if (person.IsStudent == null)
                    return new AskIsStudentCommand(_botClient, chatId, message);
                if (person.IsAnswerAll.HasValue && person.IsAnswerAll.Value)
                    return new EndCommand(_botClient, chatId);
                if (person.University == null)
                    return new AddUniversityCommand(_botClient, chatId, message);
                if (person.Course == null)
                    return new AddCourceCommand(_botClient, chatId, message);

                if (result.Questions?.Count <= GetCountQuestion())
                {
                    return new AskQuestionCommand(_botClient, chatId, message);
                }
            }
            else return new WrongCommand(_botClient, chatId);

            return new EndCommand(_botClient, chatId);
        }

        private int GetCountQuestion()
        {
            var questions = MongoDBHelperQuestion.AllQuestion().Result;
            return questions.Count();
        }
    }
}