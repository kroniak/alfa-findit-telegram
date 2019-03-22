using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class AddContactCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;
        private Message _message;
        
        public AddContactCommand(TelegramBotClient botClient, long chatId, Message message)
        {
            _botClient = botClient;
            _chatId = chatId;
            _message = message;
        }
        
        public async void Execute()
        {
            var contact = _message.Contact;
            var phone = contact.PhoneNumber;
            var telegramName = $"{contact.FirstName} {contact.LastName}";

            var keyboardRemove = BotHelper.GetRemoveKeyboard();

            await MongoDBHelperUser.SaveContact(_chatId, phone, telegramName);
            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskNameMessage, replyMarkup: keyboardRemove);
        }
    }
}