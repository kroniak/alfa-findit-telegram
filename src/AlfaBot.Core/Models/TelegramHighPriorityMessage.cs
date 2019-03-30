using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Telegram.Bot.Types.ReplyMarkups;

namespace AlfaBot.Core.Models
{
    public class TelegramHighPriorityMessage
    {
        public TelegramHighPriorityMessage(long chatId)
        {
            ChatId = chatId;
        }

        [BsonId] public ObjectId Id { get; set; }

        public long ChatId { get; set; }

        public string Text { get; set; }

        public IReplyMarkup ReplyMarkup { get; set; }
    }
}