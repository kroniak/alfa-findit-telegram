using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Telegram.Bot.Types.ReplyMarkups;

namespace AlfaBot.Core.Models
{
    public class QueueMessage
    {
        public QueueMessage(long chatId, bool isHighPriority = true)
        {
            if (chatId <= 0) throw new ArgumentOutOfRangeException(nameof(chatId));

            ChatId = chatId;
            IsHighPriority = isHighPriority;
        }

        [BsonId] 
        public ObjectId Id { get; set; }

        public bool IsHighPriority { get; set; }

        public long ChatId { get; set; }

        public string Text { get; set; }

        public IReplyMarkup ReplyMarkup { get; set; }
    }
}