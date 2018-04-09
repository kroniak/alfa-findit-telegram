using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.Commands
{
    public class CreateStudentCommand : IMessageCommand
    {
        private long _chatId;
        private TelegramBotClient _botClient;
        
        public CreateStudentCommand(TelegramBotClient botclient, long chatId)
        {
            _botClient = botclient;
            _chatId = chatId;
        }

        public async void Execute()
        {
            MongoDBHelper.AddStudent(_chatId);
            var keyboardMarkup = BotHelper.GetKeyBoardForContact();
            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.WelcomeMessage, replyMarkup:keyboardMarkup);
        }
    }
}