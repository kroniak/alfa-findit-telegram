using Telegram.Bot.Types;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IAlfaBankBot
    {
        void Start();

        void Stop();

        bool Ping();

        bool MessageHandler(Message message);
    }
}