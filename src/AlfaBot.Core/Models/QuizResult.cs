using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace AlfaBot.Core.Models
{
    public class QuizResult
    {
        [BsonId] public ObjectId Id { get; set; }

        public User User { get; set; }

        public List<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();

        public double Points { get; set; }

        public DateTime Started { get; set; }

        public DateTime Ended { get; set; }

        public int Sorting { get; set; }
    }
}