using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    [ExcludeFromCodeCoverage]
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IMongoCollection<Question> _questions;

        public int Count { get; private set; }

        public QuestionRepository(
            IMongoDatabase database,
            IMemoryCache cache)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _questions = database.GetCollection<Question>(DbConstants.QuestionCollectionName);

            Update();
        }

        private void Update()
        {
            var questions = All().ToArray();

            foreach (var question in questions)
            {
                _cache.Set(question.Id, question);
            }

            Count = questions.Length;
        }

        public Question Get(ObjectId id) =>
            _cache.GetOrCreate(id, entry =>
            {
                var filter = Builders<Question>.Filter
                    .Eq(q => q.Id, id);

                return _questions.Find(filter).SingleOrDefault();
            });

        public Question Add(Question question)
        {
            try
            {
                _questions.InsertOne(question);
                _cache.Set(question.Id, question);
            }
            catch (Exception)
            {
                return null;
            }

            return question;
        }

        public IEnumerable<Question> All() => _questions.Find(_ => true).ToEnumerable();
    }
}