using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Factories;
using AlfaBot.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlfaBot.Core.Services
{
    /// <inheritdoc />
    public class AlfaBankBot : IAlfaBankBot
    {
        private readonly IUserRepository _users;

        private readonly IQuizResultRepository _resultRepository;
        private readonly ILogRepository _logRepository;
        private readonly IGeneralCommandsFactory _generalCommandsFactory;
        private readonly IQuestionCommandFactory _questionCommandFactory;
        private readonly ILogger<AlfaBankBot> _logger;
        private readonly ITelegramBotClient _botClient;

        [ExcludeFromCodeCoverage]
        public AlfaBankBot(
            ITelegramBotClient botClient,
            IUserRepository users,
            IQuizResultRepository resultRepository,
            ILogRepository logRepository,
            IGeneralCommandsFactory generalCommandsFactory,
            IQuestionCommandFactory questionCommandFactory,
            ILogger<AlfaBankBot> logger)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
            _generalCommandsFactory =
                generalCommandsFactory ?? throw new ArgumentNullException(nameof(generalCommandsFactory));
            _questionCommandFactory =
                questionCommandFactory ?? throw new ArgumentNullException(nameof(questionCommandFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));

            Build();
        }

        public void Start()
        {
            if (_botClient.IsReceiving) return;

            try
            {
                _botClient.StartReceiving();
                _logger.LogInformation("BotClient StartReceiving");
            }
            catch (ApiRequestException e)
            {
                _logger.LogCritical("This api key is invalid.", e);
                throw;
            }
        }

        public void Stop()
        {
            _botClient.StopReceiving();
            _logger.LogInformation("BotClient StopReceiving");
        }

        public bool Ping() => _botClient.IsReceiving;

        public bool MessageHandler(Message message)
        {
            if (message == null) return false;

            // log income message
            _logRepository.Add(message);

            if (message.Chat == null) return false;

            var chatId = message.Chat.Id;
            if (chatId <= 0) return false;

            var type = Enum.GetName(typeof(MessageType), message.Type);

            _logger.LogInformation($"[{chatId}] [{type}] [{message.Text ?? ""}] Received message");

            Action action;

            // Fix other highPriorityMessage
            switch (message.Type)
            {
                case MessageType.Contact:
                case MessageType.Text:
                {
                    action = CreateCommand(message);
                    break;
                }
                default:
                {
                    var queueMessageFactory = new QueueMessageFactory(message);
                    action = _generalCommandsFactory.Command(queueMessageFactory.WrongMessage);
                    break;
                }
            }

            try
            {
                action.Invoke();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"[{chatId}] [{type}] [{message.Text ?? ""}] MessageHandlerAsync Error.", e);
                return false;
            }
        }

        private void Build() => _botClient.OnMessage += OnMessageReceived;

        private void OnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;

            MessageHandler(message);
        }

        private Action CreateCommand(Message message)
        {
            var chatId = message.Chat.Id;
            var user = _users.Get(chatId);

            var factory = new QueueMessageFactory(message);

            if (user == null)
                return _generalCommandsFactory.StartCommand(factory.WelcomeMessage);

            // handle contact message
            switch (message.Type)
            {
                case MessageType.Contact when user.Phone is null:
                    return _generalCommandsFactory.AddContactCommand(message,
                        factory.AskNameMessage(message.Contact.FirstName));

                case MessageType.Contact:
                    return _generalCommandsFactory.Command(factory.WrongMessage);
            }

            // handle text messages
            if (user.Phone == null)
            {
                return _generalCommandsFactory.Command(factory.AskContactMessage);
            }

            if (user.Name == null)
            {
                return _generalCommandsFactory.AddNameCommand(message, factory.AskStartQuizMessage);
            }

            if (user.IsQuizMember == null)
            {
                return _questionCommandFactory.AddQuizCommand(user, message, factory.AskEmail);
            }

            if (user.IsQuizMember is true)
            {
                // add handler to quiz
                var result = _resultRepository.GetResult(chatId);
                return _questionCommandFactory.QuestionCommand(result, message, factory.AskEmail);
            }

            if (user.EMail == null)
            {
                return _generalCommandsFactory.AddEMailCommand(message, factory.AskProfessionMessage);
            }

            if (user.Profession == null)
            {
                return _generalCommandsFactory.AddProfessionCommand(message, factory.AskIsStudentMessage);
            }

            if (user.IsStudent == null)
            {
                return _generalCommandsFactory.AddIsStudentCommand(
                    message,
                    factory.AskUniversityMessage,
                    factory.EndMessage);
            }

            if (user.IsAnsweredAll)
            {
                return _generalCommandsFactory.Command(factory.EndMessage);
            }

            if (user.University == null)
            {
                return _generalCommandsFactory.AddUniversityCommand(message, factory.AskCourse);
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (user.Course == null)
            {
                return _generalCommandsFactory.AddCourseCommand(
                    message,
                    factory.EndMessage,
                    factory.AskCourse);
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (user.IsAnsweredAll)
            {
                return _generalCommandsFactory.Command(factory.EndMessage);
            }

            return _generalCommandsFactory.Command(factory.WrongMessage);
        }
    }
}