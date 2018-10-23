using System.Text.RegularExpressions;
using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class AddEMailCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;
        private Message _message;

        public AddEMailCommand(TelegramBotClient botClient, long chatId, Message message)
        {
            _botClient = botClient;
            _chatId = chatId;
            _message = message;
        }

        public async void Execute()
        {
            var email = _message.Text;

            if (!IsEmailValid(email))
            {
                await _botClient.SendTextMessageAsync(_chatId, MessageFactory.WrongEMailMessage);
                return;
            }

            await MongoDBHelper.SaveEmail(_chatId, email);

            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskProfessionMessage,
                replyMarkup: BotHelper.GetKeyboardForProfession());
        }

        private bool IsEmailValid(string email)
        {
            string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
    }
}