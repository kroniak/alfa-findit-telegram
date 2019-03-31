using System;
using System.Collections.Generic;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
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

        public void Add(User item) => _users.InsertOne(item);

        public void Add(long chatId) =>
            Add(new User
            {
                ChatId = chatId
            });

        public IEnumerable<User> All() =>
            _users.Find(_ => true).ToEnumerable();

        public User Get(long chatId) =>
            _users.Find(GlobalChatIdFilter(chatId)).SingleOrDefault();

        public UpdateResult SaveContact(long chatId, string phone, string telegramName)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Phone, phone)
                .Set(p => p.TelegramName, telegramName);

            return _users.UpdateOne(filter, update);
        }

        public UpdateResult SaveEmail(long chatId, string email)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.EMail, email);

            return _users.UpdateOne(filter, update);
        }

        public UpdateResult SavePersonOrWorkerInfo(long chatId, bool? isStudent, bool? isAnswerAll)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.IsStudent, isStudent)
                .Set(p => p.IsAnsweredAll, isAnswerAll);

            return _users.UpdateOne(filter, update);
        }

        public UpdateResult SaveName(long chatId, string name)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Name, name);

            return _users.UpdateOne(filter, update);
        }

        public UpdateResult SaveUniversity(long chatId, string university)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.University, university);

            return _users.UpdateOne(filter, update);
        }

        public UpdateResult SaveCourse(long chatId, string course, bool? isAnsweredAll)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Course, course)
                .Set(p => p.IsAnsweredAll, isAnsweredAll);

            return _users.UpdateOne(filter, update);
        }

        public UpdateResult SaveProfession(long chatId, string profession)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Profession, profession);

            return _users.UpdateOne(filter, update);
        }

        public UpdateResult SetQuizMember(long chatId, bool isMember)
        {
            var filter = GlobalChatIdFilter(chatId);

            var update = Builders<User>.Update
                .Set(p => p.IsQuizMember, isMember);

            return _users.UpdateOne(filter, update);
        }

        private static FilterDefinition<User> GlobalChatIdFilter(long chatId)
            => Builders<User>.Filter.Eq(u => u.ChatId, chatId);
    }
}