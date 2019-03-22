using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FindAlfaITBot.Models;
using MongoDB.Driver;

namespace FindAlfaITBot.Infrastructure
{
    public class MongoDBHelperUser : MongoDBHelper
    {
        private static IMongoCollection<Person> _collection;

        private static IMongoCollection<RegistryQueue> _registry;

        public static IMongoCollection<Person> Collection
            => _collection ?? (_collection = Database.GetCollection<Person>("Students"));

        public static IMongoCollection<RegistryQueue> Registry
            => _registry ?? (_registry = Database.GetCollection<RegistryQueue>("RegistryQueue"));

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

        public static async void AddRegistryQueue(long chatId)
        {
            RegistryQueue registry = new RegistryQueue()
            {
                ChatId = chatId,
                Position = 0,
                IsFullRegistry = false
            };

            await Registry.InsertOneAsync(registry);
        }

        public static async Task<RegistryQueue> GetRegistry(long chatId)
        {
            var filter = Builders<RegistryQueue>.Filter.Eq(p => p.ChatId, chatId);
            return await Registry.Find(filter).FirstOrDefaultAsync();
        }

        public static async Task<UpdateResult> UpdateRegistryPosition(long chatId, int position)
        {
            var filterResult = Builders<RegistryQueue>.Filter.Eq(_ => _.ChatId, chatId);

            var update = Builders<RegistryQueue>.Update
                .Set(x => x.Position, position);

            return await Registry.UpdateOneAsync(filterResult, update);
        }

        public static async Task<UpdateResult> UpdateIsFullRegistry(long chatId)
        {
            var filterResult = Builders<RegistryQueue>.Filter.Eq(_ => _.ChatId, chatId);

            var update = Builders<RegistryQueue>.Update
                .Set(x => x.IsFullRegistry, true);

            return await Registry.UpdateOneAsync(filterResult, update);
        }
    }
}
