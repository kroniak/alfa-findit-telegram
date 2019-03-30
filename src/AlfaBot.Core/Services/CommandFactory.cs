using System;
using System.Text.RegularExpressions;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Factories;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services.Interfaces;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Services
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IUserRepository _userRepository;
        private readonly IQueueService _queueService;

        public CommandFactory(IUserRepository userRepository, IQueueService queueService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
        }

        public Action CreateStudentCommand(long chatId)
        {
            return async () =>
            {
                await _userRepository.AddAsync(chatId);

                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.WelcomeMessage,
                    ReplyMarkup = BotHelper.GetKeyBoardForContact()
                });
//                await _botClient.SendTextMessageAsync(_chatId, MessageDictionary.WelcomeMessage,
//                    replyMarkup: keyboardMarkup);
            };
        }

        public Action AddContactCommand(long chatId, Message message)
        {
            return async () =>
            {
                var contact = message.Contact;
                var phone = contact.PhoneNumber;
                var telegramName = $"{contact.FirstName} {contact.LastName}";

                await _userRepository.SaveContactAsync(chatId, phone, telegramName);
                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.NameMessage,
                    ReplyMarkup = BotHelper.GetRemoveKeyboard()
                });
            };
        }

        public Action ContactCommand(long chatId)
        {
            return async () =>
            {
                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.ContactMessage,
                    ReplyMarkup = BotHelper.GetKeyBoardForContact()
                });
            };
        }

        public Action AddNameCommand(long chatId, Message message)
        {
            return async () =>
            {
                var studentName = message.Text;

                await _userRepository.SaveNameAsync(chatId, studentName);
                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.EmailMessage
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

            return async () =>
            {
                var email = message.Text;

                if (!IsEmailValid(email))
                {
                    await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                    {
                        Text = MessageDictionary.WrongEMailMessage
                    });
                    return;
                }

                await _userRepository.SaveEmailAsync(chatId, email);

                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.ProfessionMessage,
                    ReplyMarkup = BotHelper.GetKeyboardForProfession()
                });
            };
        }

        public Action AddProfessionCommand(long chatId, Message message)
        {
            return async () =>
            {
                var profession = message.Text;

                await _userRepository.SaveProfessionAsync(chatId, profession);

                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.IsStudentMessage,
                    ReplyMarkup = BotHelper.GetKeyboardYesOrNo()
                });
            };
        }

        public Action IsStudentCommand(long chatId, Message message)
        {
            return async () =>
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

                await _userRepository.SavePersonOrWorkerInfoAsync(chatId, isStudent, isAnsweredAllQuestions);

                if (!isStudent.HasValue)
                {
                    await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                    {
                        Text = MessageDictionary.IsStudentMessage,
                        ReplyMarkup = BotHelper.GetKeyboardYesOrNo()
                    });
                }

                if (isStudent.HasValue && isStudent.Value)
                {
                    await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                    {
                        Text = MessageDictionary.UniversityMessage,
                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
                    });
                }
                else
                {
                    await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                    {
                        Text = MessageDictionary.EndOfAskingMessage,
                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
                    });
                }
            };
        }

        public Action EndCommand(long chatId)
        {
            return async () =>
            {
                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.EndMessage
                });
            };
        }

        public Action AddUniversityCommand(long chatId, Message message)
        {
            return async () =>
            {
                var university = message.Text;

                await _userRepository.SaveUniversityAsync(chatId, university);
                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.CourseMessage,
                    ReplyMarkup = BotHelper.GetKeyboardForCourse()
                });
            };
        }

        public Action AddCourseCommand(long chatId, Message message)
        {
            return async () =>
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

                await _userRepository.SaveCourseAsync(chatId, isYoung.HasValue ? course : null, isAnsweredAllQuestions);

                if (isYoung.HasValue)
                {
                    await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                    {
                        Text = MessageDictionary.EndOfAskingMessage,
                        ReplyMarkup = BotHelper.GetRemoveKeyboard()
                    });

                    if (!isYoung.Value)
                    {
                        await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                        {
                            Text = MessageDictionary.OpenDoorsInvitationMessage
                        });
                    }
                }
                else
                {
                    await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                    {
                        Text = MessageDictionary.CourseMessage,
                        ReplyMarkup = BotHelper.GetKeyboardForCourse()
                    });
                }
            };
        }

        public Action WrongCommand(long chatId)
        {
            return async () =>
            {
                await _queueService.AddHighPriorityAsync(new TelegramHighPriorityMessage(chatId)
                {
                    Text = MessageDictionary.WrongMessage
                });
            };
        }
    }
}