using System;
using System.Collections.Generic;
using System.Linq;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AlfaBot.Core.Data
{
    public class QuizResultRepository : IQuizResultRepository
    {
        private readonly IMongoCollection<QuizResult> _results;
        private readonly IMongoQueryable<QuizResult> _query;

        public QuizResultRepository(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _results = database.GetCollection<QuizResult>(DbConstants.QuizResultCollectionName);
            _query = _results.AsQueryable();
        }

        public IEnumerable<QuizResult> All()
            => _results.Find(_ => true).ToEnumerable();

        public int GetAnsweredCount(long chatId)
        {
            var questions = IAsyncCursorSourceExtensions.ToList(_query
                .Where(x => x.User.ChatId == chatId)
                .SelectMany(x => x.QuestionAnswers)).Where(q => q.IsAnswered);

            return questions.Count();
        }

        public void AddUserQuiz(User user)
        {
            var result = new QuizResult
            {
                User = user,
                Points = 0,
                isEnd = false
            };

            _results.InsertOne(result);
        }

        public QuizResult GetResult(long chatId) =>
            _results.Find(GlobalChatIdFilter(chatId)).SingleOrDefault();

        public UpdateResult SaveQuestionForUser(long chatId, QuestionAnswer answer)
        {
            var result = IAsyncCursorSourceExtensions.SingleOrDefault(_query
                .Where(x => x.User.ChatId == chatId)
                .Select(x => new QueryResult
                {
                    QuestionAnswers = x.QuestionAnswers,
                    Points = x.Points
                }));

            result.QuestionAnswers.Add(answer);
            result.Points += answer.Point;

            var update = Builders<QuizResult>.Update
                .Set(r => r.Points, result.Points)
                .Set(r => r.QuestionAnswers, result.QuestionAnswers);

            return _results.UpdateOne(GlobalChatIdFilter(chatId), update);
        }

        public UpdateResult CalcPoints(long chatId)
        {
            var questions = IAsyncCursorSourceExtensions.ToList(_query
                .Where(x => x.User.ChatId == chatId)
                .SelectMany(x => x.QuestionAnswers));

            double points = 0;
            foreach (var question in questions)
            {
                points += question.Point;
            }

            var update = Builders<QuizResult>.Update
                .Set(x => x.Points, points);

            return _results.UpdateOne(GlobalChatIdFilter(chatId), update);
        }

        public UpdateResult UpdateEnd(long chatId)
        {
            var update = Builders<QuizResult>.Update
                .Set(x => x.isEnd, true);

            return _results.UpdateOne(GlobalChatIdFilter(chatId), update);
        }

        private static FilterDefinition<QuizResult> GlobalChatIdFilter(long chatId)
            => Builders<QuizResult>.Filter.Eq(u => u.User.ChatId, chatId);
    }

    internal class QueryResult
    {
        internal List<QuestionAnswer> QuestionAnswers;

        public double Points;
    }
}