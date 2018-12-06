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

        public static async void AddPerson(Person student)
            => await Collection.InsertOneAsync(student);

        public static void AddPerson(long chatId)
            => AddPerson(new Person { ChatId = chatId });

        public static async Task<IEnumerable<Person>> All()
            => await Collection.Find(_ => true).ToListAsync();

        public static async Task<Person> GetPerson(long chatId)
        {
            var filter = Builders<Person>.Filter.Eq(p => p.ChatId, chatId);
            return await Collection.Find(filter).FirstOrDefaultAsync();
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
    }
}