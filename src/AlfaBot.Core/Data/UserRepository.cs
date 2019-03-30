using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    /// <inheritdoc />
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _users = database.GetCollection<User>(DbConstants.UserCollectionName);
        }

        public Task AddAsync(User item)
            => _users.InsertOneAsync(item);

        public Task AddAsync(long chatId)
            => AddAsync(new User
            {
                ChatId = chatId
            });

        public IEnumerable<User> All()
            => _users.Find(_ => true).ToEnumerable();

        public Task<User> GetAsync(long chatId)
        {
//            var projection = Builders<User>.Projection.Exclude(u => u.Q);

            return _users.Find(GetGlobalFilter(chatId)).FirstOrDefaultAsync();
        }

        public Task<User> GetWithQuestionAsync(long chatId) =>
            _users.Find(GetGlobalFilter(chatId)).FirstOrDefaultAsync();

        public Task<UpdateResult> SaveContactAsync(long chatId, string phone, string telegramName)
        {
            var filter = GetGlobalFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Phone, phone)
                .Set(p => p.TelegramName, telegramName);

            return _users.UpdateOneAsync(filter, update);
        }

        public Task<UpdateResult> SaveEmailAsync(long chatId, string email)
        {
            var filter = GetGlobalFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.EMail, email);

            return _users.UpdateOneAsync(filter, update);
        }

        public Task<UpdateResult> SavePersonOrWorkerInfoAsync(long chatId, bool? isStudent, bool? isAnswerAll)
        {
            var filter = GetGlobalFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.IsStudent, isStudent)
                .Set(p => p.IsAnsweredAll, isAnswerAll);

            return _users.UpdateOneAsync(filter, update);
        }

        public Task<UpdateResult> SaveNameAsync(long chatId, string name)
        {
            var filter = GetGlobalFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Name, name);

            return _users.UpdateOneAsync(filter, update);
        }

        public Task<UpdateResult> SaveUniversityAsync(long chatId, string university)
        {
            var filter = GetGlobalFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.University, university);

            return _users.UpdateOneAsync(filter, update);
        }

        public Task<UpdateResult> SaveCourseAsync(long chatId, string course, bool? isAnsweredAll)
        {
            var filter = GetGlobalFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Course, course)
                .Set(p => p.IsAnsweredAll, isAnsweredAll);

            return _users.UpdateOneAsync(filter, update);
        }

        public Task<UpdateResult> SaveProfessionAsync(long chatId, string profession)
        {
            var filter = GetGlobalFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Profession, profession);

            return _users.UpdateOneAsync(filter, update);
        }

//        public static async void AddRegistryQueueAsync(long chatId)
//        {
//            RegistryQueue registry = new RegistryQueue()
//            {
//                ChatId = chatId,
//                Position = 0,
//                IsFullRegistry = false
//            };
//
//            await Registry.InsertOneAsync(registry);
//        }
//
//        public static async Task<RegistryQueue> GetRegistry(long chatId)
//        {
//            var filter = Builders<RegistryQueue>.Filter.Eq(p => p.ChatId, chatId);
//            return await Registry.Find(filter).FirstOrDefaultAsync();
//        }
//
//        public static async Task<UpdateResult> UpdateRegistryPosition(long chatId, int position)
//        {
//            var filterResult = Builders<RegistryQueue>.Filter.Eq(_ => _.ChatId, chatId);
//
//            var update = Builders<RegistryQueue>.Update
//                .Set(x => x.Position, position);
//
//            return await Registry.UpdateOneAsync(filterResult, update);
//        }
//
//        public static async Task<UpdateResult> UpdateIsFullRegistry(long chatId)
//        {
//            var filterResult = Builders<RegistryQueue>.Filter.Eq(_ => _.ChatId, chatId);
//
//            var update = Builders<RegistryQueue>.Update
//                .Set(x => x.IsFullRegistry, true);
//
//            return await Registry.UpdateOneAsync(filterResult, update);
//        }

        private static BsonDocument GetGlobalFilter(long chatId)
            => new BsonDocument("ChatId", chatId);
    }
}