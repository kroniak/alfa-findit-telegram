using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FindAlfaITBot.Models
{
    public class Student
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public long ChatId { get; set; }
        public string TelegramName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string University { get; set; }
        public string Profession { get; set; }
    }
}