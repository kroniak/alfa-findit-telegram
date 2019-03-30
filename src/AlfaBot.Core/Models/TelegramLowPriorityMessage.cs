namespace AlfaBot.Core.Models
{
    public class TelegramLowPriorityMessage : TelegramHighPriorityMessage
    {
        public TelegramLowPriorityMessage(long chatId) : base(chatId)
        {
        }
    }
}