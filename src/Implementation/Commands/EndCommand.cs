using FindAlfaITBot.Factories;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.Commands
{
    public class EndCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;

        public EndCommand(TelegramBotClient botClient, long chatId)
        {
            _botClient = botClient;
            _chatId = chatId;
        }

        public async void Execute()
        {
            await _botClient.SendTextMessageAsync(_chatId, MessageFactory.EndMessage);
        }
    }
}