using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FindAlfaITBot.Models;
using MongoDB.Driver;

namespace FindAlfaITBot.Infrastructure
{
    public class MongoDBHelperQuestion : MongoDBHelperUser
    {
        private static IMongoCollection<Question> _question;
        private static IMongoCollection<Result> _result;

        public static IMongoCollection<Question> Question
            => _question ?? (_question = Database.GetCollection<Question>("Questions"));

        public static IMongoCollection<Result> Result
            => _result ?? (_result = Database.GetCollection<Result>("Results"));

        public static async Task<IEnumerable<Question>> AllQuestion()
            => await Question.Find(_ => true).ToListAsync();

        public static async Task<IEnumerable<Result>> AllResults()
            => await Result.Find(_ => true).ToListAsync();

        public static async void AddPersonQuiz(Person person)
        {
            await Collection.InsertOneAsync(person);

            Result result = new Result()
            {
                ChatId = person.ChatId,
                Person = person,
                Points = 0,
                Questions = new List<Question>(),
                isEnd = false
            };

            await Result.InsertOneAsync(result);
        }

        public static async Task<Question> GetQuestion(string id)
        {
            var filter = Builders<Question>.Filter
                .Eq(_ => _.QuestionId, id);

            return await Question.Find(filter).SingleOrDefaultAsync();
        }

        public static async Task<Result> GetResult(long chatId)
        {
            var filter = Builders<Result>.Filter.Eq(_ => _.ChatId, chatId);
            return await Result.Find(filter).SingleOrDefaultAsync();
        }

        public static async Task<UpdateResult> SaveResultForUser(long chatId, Question question)
        {
            var filter = Builders<Result>.Filter.Eq(_ => _.ChatId, chatId);

            var client = GetPerson(chatId);
            var result = GetResult(chatId).Result;

            var questions = result.Questions;

            questions.Add(question);

            var update = Builders<Result>.Update
                .Set(x => x.Person, client.Result)
                .Set(x => x.Questions, questions);

            return await Result.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> UpdatePoints(long chatId)
        {
            var client = GetPerson(chatId).Result;

            var filterResult = Builders<Result>.Filter.Eq(_ => _.Person, client);

            var questions = GetResult(chatId).Result.Questions;

            double points = 0;
            foreach (var question in questions)
            {
                points += question.Point;
            }

            var update = Builders<Result>.Update
                .Set(x => x.Points, points);

            return await Result.UpdateOneAsync(filterResult, update);
        }

        public static async Task<UpdateResult> UpdateEnd(long chatId)
        {
            var client = GetPerson(chatId).Result;
            var filterResult = Builders<Result>.Filter.Eq(_ => _.Person, client);

            var update = Builders<Result>.Update
                .Set(x => x.isEnd, true);

            return await Result.UpdateOneAsync(filterResult, update);
        }
    }
}
