using System;
using System.Text.RegularExpressions;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Factories;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services.Interfaces;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Services.Commands
{
    public class GeneralCommandsFactory : IGeneralCommandsFactory
    {
        private readonly IUserRepository _userRepository;
        private readonly IQueueService _queueService;

        public GeneralCommandsFactory(IUserRepository userRepository, IQueueService queueService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
        }

        public Action CreateStartCommand(long chatId, int messageId)
        {
            return () =>
            {
                _userRepository.Add(chatId);

                _queueService.Add(new QueueMessage(chatId, messageId)
                {
                    Text = GeneralMessageDictionary.WelcomeMessage,
                    ReplyMarkup = BotHelper.GetKeyBoardForContact()
                });
            };
        }

        public Action AddContactCommand(long chatId, Message message)
        {
            return () =>
            {
                var contact = message.Contact;
                var phone = contact.PhoneNumber;
                var telegramName = $"{contact.FirstName} {contact.LastName}";

                _userRepository.SaveContact(chatId, phone, telegramName);
                _queueService.Add(new QueueMessage(chatId, message.MessageId)
                {
                    Text = GeneralMessageDictionary.NameMessage,
                    ReplyMarkup = BotHelper.GetRemoveKeyboard()
                });
            };
        }

        public Action ContactCommand(long chatId, int messageId)
        {
            return () =>
            {
                _queueService.Add(new QueueMessage(chatId, messageId)
                {
                    Text = GeneralMessageDictionary.ContactMessage,
                    ReplyMarkup = BotHelper.GetKeyBoardForContact()
                });
            };
        }

        public Action AddNameCommand(long chatId, Message message)
        {
            return () =>
            {
                var name = message.Text;

                _userRepository.SaveName(chatId, name);
                _queueService.Add(new QueueMessage(chatId, message.MessageId)
                {
                    Text = QuizMessageDictionary.StartMessage,
                    ReplyMarkup = BotHelper.GetKeyboardYesOrNo()
                });
            };
        }

        public Action AddEMailCommand(long chatId, Message message)
        {
            bool IsEmailValid(string email)
            {
                const string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

                var regex = new Regex(pattern);
                return regex.IsMatch(email);
            }

            return () =>
            {
                var email = message.Text;

                if (!IsEmailValid(email))
                {
                    _queueService.Add(new QueueMessage(chatId, message.MessageId)
                    {
                        Text = GeneralMessageDictionary.WrongEMailMessage
                    });
                    return;
                }

                _userRepository.SaveEmail(chatId, email);

                _queueService.Add(new QueueMessage(chatId, message.MessageId)
                {
                    Text = GeneralMessageDictionary.ProfessionMessage,
                    ReplyMarkup = BotHelper.GetKeyboardForProfession()
                });
            };
        }

        public Action AddProfessionCommand(long chatId, Message message)
        {
            return () =>
            {
                var profession = message.Text;

                _userRepository.SaveProfession(chatId, profession);

                _queueService.Add(new QueueMessage(chatId, message.MessageId)
                {
                    Text = GeneralMessageDictionary.IsStudentMessage,
                    ReplyMarkup = BotHelper.GetKeyboardYesOrNo()
                });
            };
        }

        public Action IsStudentCommand(long chatId, Message message)
        {
            return () =>
            {
                var answer = message.Text;
                bool? isStudent = null;
                bool? isAnsweredAllQuestions = null;

                if (string.Compare(answer, "да", StringComparison.InvariantCultureIgnoreCase) == 0)
                    isStudent = true;

                if (string.Compare(answer, "нет", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    isStudent = false;
                    isAnsweredAllQuestions = true;
                }

                _userRepository.SavePersonOrWorkerInfo(chatId, isStudent, isAnsweredAllQuestions);

                if (!isStudent.HasValue)
                {
                    _queueService.Add(new QueueMessage(chatId, message.MessageId)
                    {
                        Text = GeneralMessageDictionary.IsStudentMessage,
                        ReplyMarkup = BotHelper.GetKeyboardYesOrNo()
                    });
                }

                if (isStudent.HasValue && isStudent.Value)
                {
                    _queueService.Add(new QueueMessage(chatId, message.MessageId)
                    {
                        Text = GeneralMessageDictionary.UniversityMessage,
                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
                    });
                }
                else
                {
                    _queueService.Add(new QueueMessage(chatId, message.MessageId)
                    {
                        Text = GeneralMessageDictionary.EndOfAskingMessage,
                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
                    });
                }
            };
        }

        public Action EndCommand(long chatId, int messageId)
        {
            return () =>
            {
                _queueService.Add(new QueueMessage(chatId, messageId)
                {
                    Text = GeneralMessageDictionary.EndMessage
                });
            };
        }

        public Action AddUniversityCommand(long chatId, Message message)
        {
            return () =>
            {
                var university = message.Text;

                _userRepository.SaveUniversity(chatId, university);
                _queueService.Add(new QueueMessage(chatId, message.MessageId)
                {
                    Text = GeneralMessageDictionary.CourseMessage,
                    ReplyMarkup = BotHelper.GetKeyboardForCourse()
                });
            };
        }

        public Action AddCourseCommand(long chatId, Message message)
        {
            return () =>
            {
                var course = message.Text;

                bool? isYoung = null;
                bool? isAnsweredAllQuestions = null;

                if (string.Compare(course, "1-3", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    isYoung = true;
                    isAnsweredAllQuestions = true;
                }

                if (string.Compare(course, "4 и старше", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    isYoung = false;
                    isAnsweredAllQuestions = true;
                }

                _userRepository.SaveCourse(chatId, isYoung.HasValue ? course : null, isAnsweredAllQuestions);

                if (isYoung.HasValue)
                {
                    _queueService.Add(new QueueMessage(chatId, message.MessageId)
                    {
                        Text = GeneralMessageDictionary.EndOfAskingMessage,
                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
                    });

//                    if (!isYoung.Value)
//                    {
//                        _queueService.Add(new QueueMessage(chatId)
//                        {
//                            Text = GeneralMessageDictionary.OpenDoorsInvitationMessage
//                        });
//                    }
                }
                else
                {
                    _queueService.Add(new QueueMessage(chatId, message.MessageId)
                    {
                        Text = GeneralMessageDictionary.CourseMessage,
                        ReplyMarkup = BotHelper.GetKeyboardForCourse()
                    });
                }
            };
        }

        public Action WrongCommand(long chatId, int messageId)
        {
            return () =>
            {
                _queueService.Add(new QueueMessage(chatId, messageId)
                {
                    Text = GeneralMessageDictionary.WrongMessage
                });
            };
        }
    }
}