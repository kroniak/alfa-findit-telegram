using FindAlfaITBot.Factories;
using FindAlfaITBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FindAlfaITBot.Implementation.BotCommands
{
    public class WrongCommand : IMessageCommand
    {
        private TelegramBotClient _botClient;
        private long _chatId;

        private string _message = null;

        public WrongCommand(TelegramBotClient botClient, long chatId)
        {
            _botClient = botClient;
            _chatId = chatId;
        }

        public WrongCommand(TelegramBotClient botClient, long chatId, string message)
        {
            _botClient = botClient;
            _chatId = chatId;
            _message = message;
        }

        public async void Execute()
            => await _botClient.SendTextMessageAsync(_chatId, _message ?? MessageFactory.WrongMessage);
    }
}