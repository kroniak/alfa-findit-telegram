using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class AddProfessionCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;
        private Message _message;
        
        public AddProfessionCommand(TelegramBotClient botClient, long chatId, Message message)
        {
            _botClient = botClient;
            _chatId = chatId;
            _message = message;
        }
        
        public async void Execute()
        {
            var profession = _message.Text;

            await MongoDBHelper.SaveProfession(_chatId, profession);
            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskIsStudentMessage, replyMarkup:BotHelper.GetKeyboardYesOrNo());
        }
    }
}