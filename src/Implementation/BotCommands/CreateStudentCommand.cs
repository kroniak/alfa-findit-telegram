using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;

namespace FindAlfaITBot.Implementation.BotCommands
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
            MongoDBHelperQuestion.AddPerson(_chatId);
            var keyboardMarkup = BotHelper.GetKeyBoardForContact();
            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.WelcomeMessage, replyMarkup:keyboardMarkup);
        }
    }
}