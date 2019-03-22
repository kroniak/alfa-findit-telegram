using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FindAlfaITBot.Models
{
    public class RegistryQueue
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public long ChatId { get; set; }
        public int Position { get; set; }
        public bool IsFullRegistry { get; set; }
    }
}
