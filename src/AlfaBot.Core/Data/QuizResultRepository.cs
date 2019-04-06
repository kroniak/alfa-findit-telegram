using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    [ExcludeFromCodeCoverage]
    public class QuizResultRepository : IQuizResultRepository
    {
        private readonly IMongoCollection<QuizResult> _results;

        public QuizResultRepository(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _results = database.GetCollection<QuizResult>(DbConstants.QuizResultCollectionName);
        }

        public IEnumerable<QuizResult> All() =>
            _results.Find(_ => true).Sort(Ordered).ToEnumerable();

        public IEnumerable<QuizResult> All(int limit) =>
            _results.Find(GlobalPointFilter).Sort(Ordered).Limit(limit).ToEnumerable();

        public QuizResult AddUserQuiz(User user)
        {
            var result = new QuizResult
            {
                User = user,
                Points = 0,
                Started = DateTime.Now
            };

            _results.InsertOne(result);

            return result;
        }

        public QuizResult Get(long chatId) =>
            _results.Find(GlobalChatIdFilter(chatId)).SingleOrDefault();

        public DeleteResult Delete(long chatId) => _results.DeleteOne(GlobalChatIdFilter(chatId));

        public bool IsQuizMember(long chatId) => _results.Find(GlobalChatIdFilter(chatId)).Any();

        public QuizResult GetResult(long chatId) =>
            _results.Find(GlobalChatIdFilter(chatId)).SingleOrDefault();

        public void UpdateQuestionsForUser(QuizResult result)
        {
            var points = result.QuestionAnswers.Sum(answer => answer.Point);
            result.Points = points;

            var update = Builders<QuizResult>.Update
                .Set(r => r.Points, points)
                .Set(r => r.QuestionAnswers, result.QuestionAnswers);

            _results.UpdateOne(GlobalChatIdFilter(result.User.ChatId), update);
        }

        public void UpdateTimeForUser(QuizResult result)
        {
            var sorting = (int) Math.Round((DateTime.Now - result.Started).TotalSeconds);

            var update = Builders<QuizResult>.Update
                .Set(r => r.Sorting, sorting)
                .Set(r => r.Ended, DateTime.Now);

            _results.UpdateOne(GlobalChatIdFilter(result.User.ChatId), update);
        }

        private static FilterDefinition<QuizResult> GlobalPointFilter
            => Builders<QuizResult>.Filter.Gt(u => u.Points, 0);

        private static FilterDefinition<QuizResult> GlobalChatIdFilter(long chatId)
            => Builders<QuizResult>.Filter.Eq(u => u.User.ChatId, chatId);

        private static SortDefinition<QuizResult> Ordered =>
            Builders<QuizResult>.Sort.Descending(r => r.Points).Ascending(r => r.Sorting);
    }
}