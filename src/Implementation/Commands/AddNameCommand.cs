using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.Commands
{
    public class AddNameCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;
        private Message _message;
        
        public AddNameCommand(TelegramBotClient botClient, long chatId, Message message)
        {
            _botClient = botClient;
            _chatId = chatId;
            _message = message;
        }
        
        public async void Execute()
        {
            var studentName = _message.Text;

            await MongoDBHelper.SaveName(_chatId, studentName);
            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskEmailMessage);
        }
    }
}