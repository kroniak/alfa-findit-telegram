using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class AddUniversityCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;
        private Message _message;
        
        public AddUniversityCommand(TelegramBotClient botClient, long chatId, Message message)
        {
            _botClient = botClient;
            _chatId = chatId;
            _message = message;
        }
        
        public async void Execute()
        {
            var university = _message.Text;

            await MongoDBHelper.SaveUniversity(_chatId, university);
            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskProfessionMessage, replyMarkup:BotHelper.GetKeyboardForProfession());
        }
    }
}