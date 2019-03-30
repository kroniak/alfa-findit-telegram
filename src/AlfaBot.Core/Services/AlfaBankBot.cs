using System;
using System.Threading.Tasks;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlfaBot.Core.Services
{
    /// <inheritdoc />
    public class AlfaBankBot : IAlfaBankBot
    {
        private readonly IUserRepository _users;
        private readonly ICommandFactory _commandFactory;
        private readonly ITelegramBotClient _botClient;

        public AlfaBankBot(
            ITelegramBotClient botClient,
            IUserRepository users,
            ICommandFactory commandFactory)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));

            Build();
        }

        private void Build() => _botClient.OnMessage += OnMessageReceived;

        public void Start()
        {
            if (!_botClient.IsReceiving)
            {
                _botClient.StartReceiving();
            }
        }

        public void Stop() => _botClient.StopReceiving();

        public bool Ping() => _botClient.IsReceiving;

        private async void OnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;

            // Fix other highPriorityMessage
            switch (message.Type)
            {
                case MessageType.Contact:
                case MessageType.Text:
                {
                    var action = await CreateCommandAsync(message);
                    action.Invoke();
                    break;
                }
                default:
                {
                    var action = _commandFactory.WrongCommand(message.Chat.Id);
                    action.Invoke();
                    break;
                }
            }
        }

        private async Task<Action> CreateCommandAsync(Message message)
        {
            var chatId = message.Chat.Id;
            var user = await _users.GetAsync(chatId);

//            Result result = Question.GetResult(chatId).Result;

            if (user == null)
                return _commandFactory.CreateStudentCommand(chatId);

            if (message.Type == MessageType.Contact && user.Phone is null)
                return _commandFactory.AddContactCommand(chatId, message);

            if (message.Type != MessageType.Contact)
            {
                if (user.Phone == null)
                    return _commandFactory.ContactCommand(chatId);
                if (user.Name == null)
                    return _commandFactory.AddNameCommand(chatId, message);
                if (user.EMail == null)
                    return _commandFactory.AddEMailCommand(chatId, message);
                if (user.Profession == null)
                    return _commandFactory.AddProfessionCommand(chatId, message);
                if (user.IsStudent == null)
                    return _commandFactory.IsStudentCommand(chatId, message);
                if (user.IsAnsweredAll.HasValue && user.IsAnsweredAll.Value)
                    return _commandFactory.EndCommand(chatId);
                if (user.University == null)
                    return _commandFactory.AddUniversityCommand(chatId, message);
                if (user.Course == null)
                    return _commandFactory.AddCourseCommand(chatId, message);

//                if (result.Questions?.Count <= GetCountQuestion())
//                {
//                    return new AskQuestionCommand(_botClient, chatId, highPriorityMessage);
//                }
            }
            else return _commandFactory.WrongCommand(chatId);

            return _commandFactory.EndCommand(chatId);
        }

//        private int GetCountQuestion()
//        {
//            var questions = Question.AllQuestion().Result;
//            return questions.Count();
//        }
    }
}