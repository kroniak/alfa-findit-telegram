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

        private static IMongoCollection<Student> _collection;

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

        public static IMongoCollection<Student> Collection
            => _collection ?? (_collection = Database.GetCollection<Student>("Students"));

        public static async void AddStudent(Student student) => await Collection.InsertOneAsync(student);

        public static void AddStudent(long chatId)
        {
            Student student = new Student { ChatId = chatId };
            AddStudent(student);
        }

        public static async Task<IEnumerable<Student>> All() => await Collection.Find(_ => true).ToListAsync();

        public static async Task<Student> GetStudent(long chatId)
        {
            var filter = Builders<Student>.Filter.Eq(x => x.ChatId, chatId);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public static async Task<UpdateResult> SaveContact(long chatId, string phone, string telegramName)
        {
            var filter = Builders<Student>.Filter.Eq(x => x.ChatId, chatId);
            var update = Builders<Student>.Update
                .Set(x => x.Phone, phone)
                .Set(x => x.TelegramName, telegramName);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveEmail(long chatId, string email)
        {
            var filter = Builders<Student>.Filter.Eq(x => x.ChatId, chatId);
            var update = Builders<Student>.Update
                .Set(x => x.EMail, email);

            return await Collection.UpdateOneAsync(filter, update);
        }
        
        public static async Task<UpdateResult> SaveStudentOrWorkerInfo(long chatId, bool? isSudent, bool? isAnswerAll)
        {
            var filter = Builders<Student>.Filter.Eq(x => x.ChatId, chatId);
            var update = Builders<Student>.Update
                .Set(x => x.IsStudent, isSudent)
                .Set(x => x.IsAnswerAll, isAnswerAll);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveName(long chatId, string studentName)
        {
            var filter = Builders<Student>.Filter.Eq(x => x.ChatId, chatId);
            var update = Builders<Student>.Update
                .Set(x => x.Name, studentName);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveUniversity(long chatId, string university)
        {
            var filter = Builders<Student>.Filter.Eq(x => x.ChatId, chatId);
            var update = Builders<Student>.Update
                .Set(x => x.University, university);

            return await Collection.UpdateOneAsync(filter, update);
        }
        
        public static async Task<UpdateResult> SaveCourse(long chatId, string course, bool? isAnsweredAll)
        {
            var filter = Builders<Student>.Filter.Eq(x => x.ChatId, chatId);
            var update = Builders<Student>.Update
                .Set(x => x.Course, course)
                .Set(x => x.IsAnswerAll, isAnsweredAll);

            return await Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<UpdateResult> SaveProfession(long chatId, string profession)
        {
            var filter = Builders<Student>.Filter.Eq(x => x.ChatId, chatId);
            var update = Builders<Student>.Update
                .Set(x => x.Profession, profession);

            return await Collection.UpdateOneAsync(filter, update);
        }
    }
}