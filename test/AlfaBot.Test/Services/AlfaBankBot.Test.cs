using System;
using System.Threading;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services;
using AlfaBot.Core.Services.Interfaces;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;
using User = AlfaBot.Core.Models.User;

namespace AlfaBot.Test.Services
{
    public class AlfaBankBotTest
    {
        private readonly IAlfaBankBot _bot;
        private readonly Mock<ITelegramBotClient> _client;
        private readonly Mock<ILogRepository> _logs;
        private readonly Mock<IUserRepository> _users;
        private Faker<Message> _messageFake;
        private Faker<User> _userFake;
        private readonly Mock<IGeneralCommandsFactory> _commands;

        public AlfaBankBotTest()
        {
            _client = new Mock<ITelegramBotClient>();
            _users = new Mock<IUserRepository>();
            _logs = new Mock<ILogRepository>();
            _commands = new Mock<IGeneralCommandsFactory>();
            var questions = new Mock<IQuestionCommandFactory>();
            var logger = new Mock<ILogger<AlfaBankBot>>();
            var results = new Mock<IQuizResultRepository>();

            var telegramBotClient = _client.Object;
            var userRepository = _users.Object;
            var logRepository = _logs.Object;
            var generalCommandsFactory = _commands.Object;
            var questionCommandFactory = questions.Object;
            var quizResultRepository = results.Object;

            _bot = new AlfaBankBot(
                telegramBotClient,
                userRepository,
                quizResultRepository,
                logRepository,
                generalCommandsFactory,
                questionCommandFactory,
                logger.Object);

            GenerateFakesInit();
        }

        private void GenerateFakesInit()
        {
            _messageFake = new Faker<Message>()
                .RuleFor(m => m.Chat, f => new Chat {Id = f.Random.Long(1)})
                .RuleFor(m => m.Text, f => f.Lorem.Text());

            _userFake = new Faker<User>();
        }

        [Fact]
        public void Stop_Return()
        {
            _bot.Stop();

            _client.Verify(c => c.StopReceiving(), Times.Once);
        }

        [Fact]
        public void Start_IsReceived_Return()
        {
            _client.Setup(c => c.IsReceiving).Returns(true);

            _bot.Start();

            _client.VerifyGet(c => c.IsReceiving);
            _client.Verify(c => c.StartReceiving(
                null,
                default(CancellationToken)), Times.Never);
        }

        [Fact]
        public void Start_IsNotReceived_Return()
        {
            _client.Setup(c => c.IsReceiving).Returns(false);

            _bot.Start();

            _client.VerifyGet(c => c.IsReceiving);
            _client.Verify(c => c.StartReceiving(
                null,
                default(CancellationToken)), Times.Once);
        }

        [Fact]
        public void Start_IsNotReceived_Exception_ReturnException()
        {
            _client.Setup(c => c.IsReceiving).Returns(false);
            _client.Setup(c => c.StartReceiving(null,
                default(CancellationToken))).Throws<Exception>();

            Assert.Throws<Exception>(() => _bot.Start());

            _client.VerifyGet(c => c.IsReceiving);
            _client.Verify(c => c.StartReceiving(
                null,
                default(CancellationToken)), Times.Once);
        }

        [Fact]
        public void Ping_IsReceiving_Return()
        {
            _client.Setup(c => c.IsReceiving).Returns(true);

            Assert.True(_bot.Ping());

            _client.VerifyGet(c => c.IsReceiving);

            _client.Verify(c => c.StartReceiving(
                null,
                default(CancellationToken)), Times.Never);
        }

        [Fact]
        public void Ping_IsNotReceiving_Return()
        {
            _client.Setup(c => c.IsReceiving).Returns(false);

            Assert.False(_bot.Ping());

            _client.VerifyGet(c => c.IsReceiving);

            _client.Verify(c => c.StartReceiving(
                null,
                default(CancellationToken)), Times.Never);
        }

        [Fact]
        public void MessageHandler_NullMessage_Return_False() => Assert.False(_bot.MessageHandler(null));

        [Fact]
        public void MessageHandler_NullMessageChat_Return_False()
        {
            var message = new Message {Chat = null};

            var result = _bot.MessageHandler(message);
            Assert.False(result);
        }

        [Fact]
        public void MessageHandler_NonTextContact_ReturnTrue_WrongCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => null)
                .RuleFor(m => m.Sticker, f => new Sticker {Emoji = f.Image.LoremFlickrUrl()})
                .Generate();

            _commands
                .Setup(c => c.Command(It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);

            _commands
                .Verify(c => c.Command(It.IsAny<QueueMessage>()), Times.Once);
        }

        [Fact]
        public void MessageHandler_AnyMessage_ReturnFalse_Exception()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => null)
                .RuleFor(m => m.Sticker, f => new Sticker {Emoji = f.Image.LoremFlickrUrl()})
                .Generate();

            _commands
                .Setup(c => c.Command(It.IsAny<QueueMessage>()))
                .Returns(() => () => throw new Exception());

            var result = _bot.MessageHandler(message);
            Assert.False(result);

            _logs.Verify(l => l.Add(message), Times.Once);

            _commands
                .Verify(c => c.Command(It.IsAny<QueueMessage>()), Times.Once);
        }

        [Fact]
        public void MessageHandler_Contact_UserNullPhone_ReturnTrue_StartCommand()
        {
            var message = _messageFake.Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns((User) null);
            _commands
                .Setup(c => c.StartCommand(It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(c => c.StartCommand(It.IsAny<QueueMessage>()), Times.Once());
        }

        [Fact]
        public void MessageHandler_Text_NullUser_ReturnTrue_StartCommand()
        {
            var message = _messageFake.Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns((User) null);
            _commands
                .Setup(c => c.StartCommand(It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(c => c.StartCommand(It.IsAny<QueueMessage>()), Times.Once());
        }

        [Fact]
        public void MessageHandler_Text_EmptyUser_ReturnTrue_ContactCommand()
        {
            var message = _messageFake.Generate();
            var user = _userFake
                //.RuleFor(u => u.Name, f => f.Name.FirstName())
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.Command(It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(c => c.Command(It.IsAny<QueueMessage>()), Times.Once);
        }

        [Fact]
        public void MessageHandler_Contact_EmptyUser_ReturnTrue_ContactCommand()
        {
            var message = _messageFake.Generate();
            var user = _userFake
                //.RuleFor(u => u.Name, f => f.Name.FirstName())
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.Command(It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(c => c.Command(It.IsAny<QueueMessage>()), Times.Once);
        }
    }
}