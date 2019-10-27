using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace AlfaBot.Core.Models
{
    public class User
    {
        [BsonId] 
        public ObjectId Id { get; set; }
        public long ChatId { get; set; }
        public string TelegramName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string Profession { get; set; }
        public bool IsAnsweredAll { get; set; }
        public bool? IsQuizMember { get; set; }
    }
}