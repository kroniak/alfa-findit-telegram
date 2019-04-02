using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlfaBot.Core.Models
{
    public class Question
    {
        [BsonId] 
        public ObjectId Id { get; set; }
        
        public bool IsPicture { get; set; }
        
        public string Answer { get; set; }
        
        public string Message { get; set; }
        
        public double Point { get; set; }
    }
}