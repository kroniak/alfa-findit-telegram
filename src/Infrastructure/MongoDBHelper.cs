using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FindAlfaITBot.Models;
using MongoDB.Driver;

namespace FindAlfaITBot.Infrastructure
{
    public class MongoDBHelper
    {
        private static string _connectionString = "mongodb://db";
        private static string _dbName = "FindIT";

        private static IMongoDatabase _database;

        private static MongoClient _client;

        private static IMongoCollection<Person> _collection;
        private static IMongoCollection<Question> _question;
        private static IMongoCollection<Result> _result;

        public static string GetConnectionName => $"{_connectionString}:{_dbName}";

        public static void Configure(string connectionString, string dbName)
        {
            _connectionString = connectionString;
            _dbName = dbName;
        }

        public static void ConfigureConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void ConfigureDB(string dbName)
        {
            _dbName = dbName;
        }

        public static MongoClient Client
            => _client ?? (_client = new MongoClient(_connectionString));

        public static IMongoDatabase Database
            => _database ?? (_database = Client.GetDatabase(_dbName));

        public static IMongoCollection<Person> Collection
            => _collection ?? (_collection = Database.GetCollection<Person>("Students"));

        public static IMongoCollection<Question> Question
            => _question ?? (_question = Database.GetCollection<Question>("Questions"));

        public static IMongoCollection<Result> Result
            => _result ?? (_result = Database.GetCollection<Result>("Results"));

        public static async void AddPerson(Person student)
            => await Collection.InsertOneAsync(student);

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

        public static void AddPerson(long chatId)
            => AddPerson(new Person { ChatId = chatId });

        public static async Task<IEnumerable<Person>> All()
            => await Collection.Find(_ => true).ToListAsync();

        public static async Task<IEnumerable<Question>> AllQuestion()
            => await Question.Find(_ => true).ToListAsync();

        public static async Task<IEnumerable<Result>> AllResults() 
            => await Result.Find(_ => true).ToListAsync();


        public static async Task<Person> GetPerson(long chatId)
        {
            var filter = Builders<Person>.Filter.Eq(p => p.ChatId, chatId);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public static async Task<Question> GetQuestion(string id)
        {
            var filter = Builders<Question>.Filter
                .Eq(_ => _.QuestionId, id);

            return await Question.Find(filter).SingleOrDefaultAsync();
        }

        public static async Task<UpdateResult> SaveContact(long chatId, string phone, string telegramName)
        {
            var filter = Builders<Person>.Filter.Eq(p => p.ChatId, chatId);
            var update = Builders<Person>.Update
                .Set(p => p.Phone, phone)
                .Set(p => p.TelegramName, telegramName);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveEmail(long chatId, string email)
        {
            var filter = Builders<Person>.Filter.Eq(p => p.ChatId, chatId);
            var update = Builders<Person>.Update
                .Set(p => p.EMail, email);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SavePersonOrWorkerInfo(long chatId, bool? isSudent, bool? isAnswerAll)
        {
            var filter = Builders<Person>.Filter.Eq(x => x.ChatId, chatId);
            var update = Builders<Person>.Update
                .Set(p => p.IsStudent, isSudent)
                .Set(p => p.IsAnswerAll, isAnswerAll);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveName(long chatId, string name)
        {
            var filter = Builders<Person>.Filter.Eq(p => p.ChatId, chatId);
            var update = Builders<Person>.Update
                .Set(p => p.Name, name);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveUniversity(long chatId, string university)
        {
            var filter = Builders<Person>.Filter.Eq(p => p.ChatId, chatId);
            var update = Builders<Person>.Update
                .Set(p => p.University, university);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveCourse(long chatId, string course, bool? isAnsweredAll)
        {
            var filter = Builders<Person>.Filter.Eq(p => p.ChatId, chatId);
            var update = Builders<Person>.Update
                .Set(p => p.Course, course)
                .Set(p => p.IsAnswerAll, isAnsweredAll);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveProfession(long chatId, string profession)
        {
            var filter = Builders<Person>.Filter.Eq(p => p.ChatId, chatId);
            var update = Builders<Person>.Update
                .Set(p => p.Profession, profession);

            return await Collection.UpdateOneAsync(filter, update);
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