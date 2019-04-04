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

        public IEnumerable<QuizResult> All()
            => _results.Find(_ => true).ToEnumerable();

        public QuizResult AddUserQuiz(User user)
        {
            var result = new QuizResult
            {
                User = user,
                Points = 0
            };

            _results.InsertOne(result);

            return result;
        }

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

        public void SaveQuestionForUser(QuizResult result, QuestionAnswer answer)
        {
            result.QuestionAnswers.Add(answer);

            var update = Builders<QuizResult>.Update
                .Set(r => r.QuestionAnswers, result.QuestionAnswers);

            _results.UpdateOne(GlobalChatIdFilter(result.User.ChatId), update);
        }

        public void UpdateTimeForUser(QuizResult result)
        {
            var seconds = (result.Id.CreationTime.ToLocalTime() - DateTime.Now).TotalSeconds;
            var update = Builders<QuizResult>.Update
                .Set(r => r.Seconds, seconds);

            _results.UpdateOne(GlobalChatIdFilter(result.User.ChatId), update);
        }

        private static FilterDefinition<QuizResult> GlobalChatIdFilter(long chatId)
            => Builders<QuizResult>.Filter.Eq(u => u.User.ChatId, chatId);
    }
}