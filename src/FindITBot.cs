using System.Net;
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

        public FindITBot(string token, string secretKey)
        {
            _botClient = new TelegramBotClient(token, new WebProxy("173.212.204.122",3128));
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

        public bool Ping()
        {
            return _botClient.IsReceiving;
        }

        private void OnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;

            CreateCommand(message).Execute();
        }

        private IMessageCommand CreateCommand(Message message)
        {
            var chatId = message.Chat.Id;
            Student student = MongoDBHelper.GetStudent(chatId).Result;

            if (student == null)
                return new CreateStudentCommand(_botClient, chatId);
            if (message.Type == MessageType.ContactMessage)
                return new AddContactCommand(_botClient, chatId, message);
            if (student.Phone == null)
                return new AskContactCommand(_botClient, chatId);
            if (student.Name == null)
                return new AddNameCommand(_botClient, chatId, message);
            if (student.EMail == null)
                return new AddEMailCommand(_botClient, chatId, message);
            if (student.Profession == null)
                return new AddProfessionCommand(_botClient, chatId, message);
            if (student.IsStudent == null)
                return  new  AskIsStudentCommand(_botClient, chatId, message);
            if (student.IsAnswerAll.HasValue && student.IsAnswerAll.Value)
                return new EndCommand(_botClient, chatId);
            if (student.University == null)
                return new AddUniversityCommand(_botClient, chatId, message);
            if (student.Course == null)
                return new AddCourceCommand(_botClient, chatId, message);

            return new EndCommand(_botClient, chatId);
        }
    }
}