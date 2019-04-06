using System;
using System.Text.RegularExpressions;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Factories.Dictionaries;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services.Interfaces;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Factories.Commands
{
    public class GeneralCommandsFactory : IGeneralCommandsFactory
    {
        private readonly IUserRepository _userRepository;
        private readonly IQueueService _queueService;
        private readonly IQuizResultRepository _resultRepository;

        public GeneralCommandsFactory(
            IUserRepository userRepository,
            IQueueService queueService,
            IQuizResultRepository resultRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(_resultRepository));
        }

        public Action Command(QueueMessage queueMessage)
        {
            return () => { _queueService.Add(queueMessage); };
        }

        public Action StartCommand(QueueMessage nextMessage)
        {
            return () =>
            {
                _userRepository.Add(nextMessage.ChatId);

                _queueService.Add(nextMessage);
            };
        }

        public Action AddContactCommand(Message message, QueueMessage nextMessage)
        {
            return () =>
            {
                var contact = message.Contact;
                var phone = contact.PhoneNumber;
                var telegramName = $"{contact.FirstName} {contact.LastName}";

                _userRepository.SaveContact(message.Chat.Id, phone, telegramName);

                _queueService.Add(nextMessage);
            };
        }

        public Action AddNameCommand(Message message, QueueMessage nextMessage)
        {
            return () =>
            {
                _userRepository.SaveName(message.Chat.Id, message.Text);

                _queueService.Add(nextMessage);
            };
        }

        public Action AddEMailCommand(Message message, QueueMessage nextMessage)
        {
            return () =>
            {
                var chatId = message.Chat.Id;
                var email = message.Text;

                bool IsEmailValid(string value)
                {
                    const string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

                    var regex = new Regex(pattern);
                    return regex.IsMatch(value);
                }

                if (IsEmailValid(email))
                {
                    _userRepository.SaveEmail(chatId, email);
                    _queueService.Add(nextMessage);
                }
                else
                {
                    _queueService.Add(new QueueMessage(chatId, message.MessageId, false)
                    {
                        Text = GeneralMessageDictionary.WrongEMailMessage
                    });
                }
            };
        }

        public Action AddProfessionCommand(Message message, QueueMessage nextMessage)
        {
            return () =>
            {
                _userRepository.SaveProfession(message.Chat.Id, message.Text);
                _queueService.Add(nextMessage);
            };
        }

        public Action AddIsStudentCommand(Message message, QueueMessage nextTrueMessage, QueueMessage nextFalseMessage)
        {
            return () =>
            {
                var answer = message.Text;
                var chatId = message.Chat.Id;
                bool? isStudent = null;

                bool? isAnsweredAllQuestions = null;
                if (Compare(answer, "да"))
                {
                    isStudent = true;
                }

                if (Compare(answer, "нет"))
                {
                    isStudent = false;
                    isAnsweredAllQuestions = true;
                }

                _userRepository.SaveStudentOrNot(chatId, isStudent, isAnsweredAllQuestions);
                if (!isStudent.HasValue)
                {
                    var factory = new QueueMessageFactory(message);
                    _queueService.Add(factory.AskIsStudentMessage);
                }

                if (isStudent.HasValue && isStudent.Value)
                {
                    _queueService.Add(nextTrueMessage);
                }
                else
                {
                    if (!_resultRepository.IsQuizMember(chatId))
                    {
                        _userRepository.SetQuizMember(chatId, null);
                    }

                    _queueService.Add(nextFalseMessage);
                }
            };
        }

        public Action AddUniversityCommand(Message message, QueueMessage nextMessage)
        {
            return () =>
            {
                var university = message.Text;
                _userRepository.SaveUniversity(message.Chat.Id, university);
                _queueService.Add(nextMessage);
            };
        }

        public Action AddCourseCommand(
            Message message,
            QueueMessage nextTrueMessage,
            QueueMessage nextFalseMessage)
        {
            var chatId = message.Chat.Id;

            return () =>
            {
                var course = message.Text;
                bool? isYoung = null;
                bool? isAnsweredAllQuestions = null;
                if (Compare(course, "1-3"))
                {
                    isYoung = true;
                    isAnsweredAllQuestions = true;
                }

                if (Compare(course, "4 и старше"))
                {
                    isYoung = false;
                    isAnsweredAllQuestions = true;
                }

                if (isAnsweredAllQuestions.HasValue && !_resultRepository.IsQuizMember(chatId))
                {
                    _userRepository.SetQuizMember(chatId, null);
                }

                _userRepository.SaveCourse(message.Chat.Id, isYoung.HasValue ? course : null, isAnsweredAllQuestions);
                _queueService.Add(isYoung.HasValue ? nextTrueMessage : nextFalseMessage);
            };
        }

        private static bool Compare(string a, string b) =>
            string.Compare(a, b, StringComparison.InvariantCultureIgnoreCase) == 0;
    }
}