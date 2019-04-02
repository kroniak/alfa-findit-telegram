using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlfaBot.Core.Models
{
    public class QuestionAnswer
    {
        [BsonId] 
        public ObjectId Id { get; set; }

        [BsonId] 
        public ObjectId QuestionId { get; set; }

        public string Answer { get; set; }

        public double Point { get; set; }

        public bool IsAnswered { get; set; }
    }
}