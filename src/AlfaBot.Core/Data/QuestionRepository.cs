using System;
using System.Collections.Generic;
using System.Linq;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IMongoCollection<Question> _questions;

        public int Count { get; private set; }

        public QuestionRepository(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _questions = database.GetCollection<Question>(DbConstants.QuestionCollectionName);

            var result = All();
            Count = result.Count();
        }

        public IEnumerable<Question> All() => _questions.Find(_ => true).ToEnumerable();

        public void Add(Question question)
        {
            _questions.InsertOne(question);
            Count = All().Count();
        }

        public Question Get(ObjectId id)
        {
            var filter = Builders<Question>.Filter
                .Eq(q => q.Id, id);

            return _questions.Find(filter).SingleOrDefault();
        }
    }
}