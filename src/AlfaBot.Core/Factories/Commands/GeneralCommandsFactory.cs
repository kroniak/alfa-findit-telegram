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

                var chatId = message.Chat.Id;
                if (!_resultRepository.IsQuizMember(chatId))
                {
                    _userRepository.SetQuizMember(chatId, null);
                }

                _queueService.Add(nextMessage);
            };
        }
    }
}