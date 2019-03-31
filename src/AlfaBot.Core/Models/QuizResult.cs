using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlfaBot.Core.Models
{
    public class QuizResult
    {
        [BsonId] 
        public ObjectId Id { get; set; }

        public User User { get; set; }

        public List<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();

        public double Points { get; set; }

        public bool isEnd { get; set; }
    }
}