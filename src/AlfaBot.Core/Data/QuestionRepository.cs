//using System.Collections.Generic;
//using System.Threading.Tasks;
//using FindAlfaITBot.Models;
//using MongoDB.Driver;
//
//namespace FindAlfaITBot.Infrastructure
//{
//    public class Question : UserRepository
//    {
//        private static IMongoCollection<Models.Question> _question;
//        private static IMongoCollection<Result> _result;
//
//        public static IMongoCollection<Models.Question> Question
//            => _question ?? (_question = Database.GetCollection<Models.Question>("Questions"));
//
//        public static IMongoCollection<Result> Result
//            => _result ?? (_result = Database.GetCollection<Result>("Results"));
//
//        public static async Task<IEnumerable<Models.Question>> AllQuestion()
//            => Question.Find(_ => true).ToListAsync();
//
//        public static async Task<IEnumerable<Result>> AllResults()
//            => Result.Find(_ => true).ToListAsync();
//
//        public static async void AddPersonQuiz(User user)
//        {
//            await Collection.InsertOneAsync(user);
//
//            Result result = new Result()
//            {
//                ChatId = user.ChatId,
//                User = user,
//                Points = 0,
//                Questions = new List<Models.Question>(),
//                isEnd = false
//            };
//
//            await Result.InsertOneAsync(result);
//        }
//
//        public static async Task<Models.Question> GetQuestion(string id)
//        {
//            var filter = Builders<Models.Question>.Filter
//                .Eq(_ => _.QuestionId, id);
//
//            return await Question.Find(filter).SingleOrDefaultAsync();
//        }
//
//        public static async Task<Result> GetResult(long chatId)
//        {
//            var filter = Builders<Result>.Filter.Eq(_ => _.ChatId, chatId);
//            return await Result.Find(filter).SingleOrDefaultAsync();
//        }
//
//        public static async Task<UpdateResult> SaveResultForUser(long chatId, Models.Question question)
//        {
//            var filter = Builders<Result>.Filter.Eq(_ => _.ChatId, chatId);
//
//            var client = GetPerson(chatId);
//            var result = GetResult(chatId).Result;
//
//            var questions = result.Questions;
//
//            questions.Add(question);
//
//            var update = Builders<Result>.Update
//                .Set(x => x.User, client.Result)
//                .Set(x => x.Questions, questions);
//
//            return await Result.UpdateOneAsync(filter, update);
//        }
//
//        public static async Task<UpdateResult> UpdatePoints(long chatId)
//        {
//            var client = GetPerson(chatId).Result;
//
//            var filterResult = Builders<Result>.Filter.Eq(_ => _.User, client);
//
//            var questions = GetResult(chatId).Result.Questions;
//
//            double points = 0;
//            foreach (var question in questions)
//            {
//                points += question.Point;
//            }
//
//            var update = Builders<Result>.Update
//                .Set(x => x.Points, points);
//
//            return await Result.UpdateOneAsync(filterResult, update);
//        }
//
//        public static async Task<UpdateResult> UpdateEnd(long chatId)
//        {
//            var client = GetPerson(chatId).Result;
//            var filterResult = Builders<Result>.Filter.Eq(_ => _.User, client);
//
//            var update = Builders<Result>.Update
//                .Set(x => x.isEnd, true);
//
//            return await Result.UpdateOneAsync(filterResult, update);
//        }
//    }
//}

