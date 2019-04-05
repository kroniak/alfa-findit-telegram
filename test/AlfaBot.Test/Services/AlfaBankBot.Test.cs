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
using Telegram.Bot.Exceptions;
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
        private readonly Mock<IQuestionCommandFactory> _questions;

        public AlfaBankBotTest()
        {
            _client = new Mock<ITelegramBotClient>();
            _users = new Mock<IUserRepository>();
            _logs = new Mock<ILogRepository>();
            _commands = new Mock<IGeneralCommandsFactory>();
            _questions = new Mock<IQuestionCommandFactory>();
            var logger = new Mock<ILogger<AlfaBankBot>>();
            var results = new Mock<IQuizResultRepository>();

            var telegramBotClient = _client.Object;
            var userRepository = _users.Object;
            var logRepository = _logs.Object;
            var generalCommandsFactory = _commands.Object;
            var questionCommandFactory = _questions.Object;
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
        public void MessageHandler_ChaId0_Return_False()
        {
            var message = _messageFake.RuleFor(m => m.Chat, f => new Chat {Id = 0}).Generate();

            Assert.False(_bot.MessageHandler(message));
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
        public void MessageHandler_Text_EmptyUser_ReturnTrue_AskContactCommand()
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
        public void MessageHandler_Contact_EmptyUser_ReturnTrue_AddContactCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Contact, f => new Contact
                {
                    LastName = f.Name.LastName(),
                    FirstName = f.Name.FirstName(),
                    PhoneNumber = f.Phone.PhoneNumber()
                })
                .RuleFor(m => m.Text, f => null)
                .Generate();

            var user = _userFake.Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.AddContactCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(c => c.AddContactCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()), Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_EmptyNameUser_ReturnTrue_AddNameCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => f.Name.FirstName())
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.AddNameCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(c => c.AddNameCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()), Times.Once);
        }

        [Fact]
        public void MessageHandler_Contact_User_ReturnTrue_Command()
        {
            var message = _messageFake
                .RuleFor(m => m.Contact, f => new Contact
                {
                    LastName = f.Name.LastName(),
                    FirstName = f.Name.FirstName(),
                    PhoneNumber = f.Phone.PhoneNumber()
                })
                .RuleFor(m => m.Text, f => null)
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
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
        public void MessageHandler_Text_EmptyQuizUser_ReturnTrue_AddQuizCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "Викторина")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _questions
                .Setup(c => c.AddQuizCommand(user, It.IsAny<Message>(), It.IsAny<QueueMessage>(),
                    It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _questions
                .Verify(
                    c => c.AddQuizCommand(user, It.IsAny<Message>(), It.IsAny<QueueMessage>(),
                        It.IsAny<QueueMessage>()), Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_IsQuizMemberUser_ReturnTrue_QuestionCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => true)
                .RuleFor(u => u.IsAnsweredAll, f => false)
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _questions
                .Setup(c => c.QuestionCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>(),
                    false))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _questions
                .Verify(
                    c => c.QuestionCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>(),
                        false), Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_IsQuizMemberUser_IsAnsweredAll_ReturnTrue_QuestionCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => true)
                .RuleFor(u => u.IsAnsweredAll, f => true)
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _questions
                .Setup(c => c.QuestionCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>(),
                    true))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _questions
                .Verify(
                    c => c.QuestionCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>(),
                        true), Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_EmptyEmail_ReturnTrue_AddEMailCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => false)
                .RuleFor(u => u.EMail, f => null)
                .RuleFor(u => u.IsAnsweredAll, f => false)
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.AddEMailCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(
                    c => c.AddEMailCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()), Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_EmptyProfession_ReturnTrue_AddProfessionCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => false)
                .RuleFor(u => u.EMail, f => f.Person.Email)
                .RuleFor(u => u.Profession, f => null)
                .RuleFor(u => u.IsAnsweredAll, f => false)
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.AddProfessionCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(
                    c => c.AddProfessionCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()), Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_EmptyIsStudent_ReturnTrue_AddIsStudentCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => false)
                .RuleFor(u => u.EMail, f => f.Person.Email)
                .RuleFor(u => u.Profession, f => f.Person.Company.Name)
                .RuleFor(u => u.IsStudent, f => null)
                .RuleFor(u => u.IsAnsweredAll, f => false)
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.AddIsStudentCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>(),
                    It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(
                    c => c.AddIsStudentCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>(), It.IsAny<QueueMessage>()),
                    Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_EmptyUniversity_IsStudent_ReturnTrue_AddUniversityCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => false)
                .RuleFor(u => u.EMail, f => f.Person.Email)
                .RuleFor(u => u.Profession, f => f.Person.Company.Name)
                .RuleFor(u => u.IsStudent, f => true)
                .RuleFor(u => u.University, f => null)
                .RuleFor(u => u.IsAnsweredAll, f => false)
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.AddUniversityCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(
                    c => c.AddUniversityCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>()),
                    Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_EmptyUCourse_IsStudent_ReturnTrue_AddCourseCommand()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => false)
                .RuleFor(u => u.EMail, f => f.Person.Email)
                .RuleFor(u => u.Profession, f => f.Person.Company.Name)
                .RuleFor(u => u.IsStudent, f => true)
                .RuleFor(u => u.University, f => "123")
                .RuleFor(u => u.Course, f => null)
                .RuleFor(u => u.IsAnsweredAll, f => false)
                .Generate();

            _users.Setup(u => u.Get(message.Chat.Id)).Returns(user);
            _commands
                .Setup(c => c.AddCourseCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>(), It.IsAny<QueueMessage>()))
                .Returns(() => () => { });

            var result = _bot.MessageHandler(message);
            Assert.True(result);

            _logs.Verify(l => l.Add(message), Times.Once);
            _users.Verify(u => u.Get(message.Chat.Id), Times.Once);
            _commands
                .Verify(
                    c => c.AddCourseCommand(It.IsAny<Message>(), It.IsAny<QueueMessage>(), It.IsAny<QueueMessage>()),
                    Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_IsAnsweredAll_IsNotStudent_ReturnTrue_Command()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => false)
                .RuleFor(u => u.EMail, f => f.Person.Email)
                .RuleFor(u => u.Profession, f => f.Person.Company.Name)
                .RuleFor(u => u.IsStudent, f => false)
                .RuleFor(u => u.University, f => "123")
                .RuleFor(u => u.Course, f => null)
                .RuleFor(u => u.IsAnsweredAll, f => true)
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
                .Verify(
                    c => c.Command(It.IsAny<QueueMessage>()),
                    Times.Once);
        }

        [Fact]
        public void MessageHandler_Text_IsAnsweredAll_IsStudent_ReturnTrue_Command()
        {
            var message = _messageFake
                .RuleFor(m => m.Text, f => "123")
                .Generate();

            var user = _userFake
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.IsQuizMember, f => false)
                .RuleFor(u => u.EMail, f => f.Person.Email)
                .RuleFor(u => u.Profession, f => f.Person.Company.Name)
                .RuleFor(u => u.IsStudent, f => true)
                .RuleFor(u => u.University, f => "123")
                .RuleFor(u => u.Course, f => "123")
                .RuleFor(u => u.IsAnsweredAll, f => true)
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
                .Verify(
                    c => c.Command(It.IsAny<QueueMessage>()),
                    Times.Once);
        }
    }
}