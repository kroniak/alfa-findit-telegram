using FindAlfaITBot.Factories;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class AskContactCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;

        public AskContactCommand(TelegramBotClient botClient, long chatId)
        {
            _botClient = botClient;
            _chatId = chatId;
        }

        public async void Execute()
        {
            var keyboard = BotHelper.GetKeyBoardForContact();

            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.AskContactMessage, replyMarkup: keyboard);
        }
    }
}