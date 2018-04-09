using System;
using System.IO;
using FindAlfaITBot.Implementation.Commands;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using FindAlfaITBot.Models;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FindAlfaITBot
{
    public class FindITBot
    {
        private const bool Debug = false;
        private readonly TelegramBotClient _botClient;

        public FindITBot(string token)
        {
            _botClient = new TelegramBotClient(token);
            Build().Start();
        }

        public FindITBot Build()
        {
            _botClient.OnMessage += OnMessageReceived;

            return this;
        }

        public void Start()
        {
            _botClient.StartReceiving();
        }

        public void Stop()
        {
            _botClient.StopReceiving();
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
            if (student.University == null)
                return new AddUniversityCommand(_botClient, chatId, message);
            if (student.Profession == null)
                return new AddprofessionCommand(_botClient, chatId, message);

            return new EndCommand(_botClient, chatId);
        }
    }
}