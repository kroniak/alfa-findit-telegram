using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Telegram.Bot.Types.ReplyMarkups;

namespace AlfaBot.Core.Models
{
    public class QueueMessage
    {
        public QueueMessage(long chatId, int incomeMessageId, bool isHighPriority = true)
        {
            if (chatId <= 0) throw new ArgumentOutOfRangeException(nameof(chatId));

            ChatId = chatId;
            IsHighPriority = isHighPriority;
            IncomeMessageId = incomeMessageId;
        }

        [BsonId] 
        public ObjectId Id { get; set; }

        public int IncomeMessageId { get; set; }

        public bool IsHighPriority { get; set; }

        public long ChatId { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public IReplyMarkup ReplyMarkup { get; set; }
    }
}