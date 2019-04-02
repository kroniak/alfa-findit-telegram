using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Data.Interfaces;
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

//        private readonly IQuizResultRepository _resultRepository;
        private readonly ILogRepository _logRepository;
        private readonly IGeneralCommandsFactory _generalCommandsFactory;
        private readonly IQuestionCommandFactory _questionCommandFactory;
        private readonly ILogger<AlfaBankBot> _logger;
        private readonly ITelegramBotClient _botClient;

        [ExcludeFromCodeCoverage]
        public AlfaBankBot(
            ITelegramBotClient botClient,
            IUserRepository users,
//            IQuizResultRepository resultRepository,
            ILogRepository logRepository,
            IGeneralCommandsFactory generalCommandsFactory,
            IQuestionCommandFactory questionCommandFactory,
            ILogger<AlfaBankBot> logger)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
//            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
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
                    action = _generalCommandsFactory.WrongCommand(chatId, message.MessageId);
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

            if (user == null)
                return _generalCommandsFactory.CreateStartCommand(chatId, message.MessageId);

            if (message.Type == MessageType.Contact && user.Phone is null)
                return _generalCommandsFactory.AddContactCommand(chatId, message);

            if (message.Type != MessageType.Contact)
            {
                if (user.Phone == null)
                {
                    return _generalCommandsFactory.ContactCommand(chatId, message.MessageId);
                }

                if (user.Name == null)
                {
                    return _generalCommandsFactory.AddNameCommand(chatId, message);
                }

                if (user.IsQuizMember == null)
                {
                    return _questionCommandFactory.AddQuizCommand(user, message);
                }

                // TODO add second handler to quiz 

                if (user.EMail == null)
                {
                    return _generalCommandsFactory.AddEMailCommand(chatId, message);
                }

                if (user.Profession == null)
                {
                    return _generalCommandsFactory.AddProfessionCommand(chatId, message);
                }

                if (user.IsStudent == null)
                {
                    return _generalCommandsFactory.IsStudentCommand(chatId, message);
                }

                if (user.IsAnsweredAll)
                {
                    return _generalCommandsFactory.EndCommand(chatId, message.MessageId);
                }

                if (user.University == null)
                {
                    return _generalCommandsFactory.AddUniversityCommand(chatId, message);
                }

                if (user.Course == null)
                {
                    return _generalCommandsFactory.AddCourseCommand(chatId, message);
                }

//                var quizResult = _resultRepository.GetResult(chatId);
//                if (quizResult.isEnd)
//                {
//                    return _questionCommandFactory.EndQuestionCommand(chatId);
//                }
//
//                return _questionCommandFactory.QuestionCommand(quizResult);
            }

            return _generalCommandsFactory.WrongCommand(chatId, message.MessageId);
        }

//        private int GetCountQuestion()
//        {
//            var questions = Question.AllQuestion().Result;
//            return questions.Count();
//        }
    }
}