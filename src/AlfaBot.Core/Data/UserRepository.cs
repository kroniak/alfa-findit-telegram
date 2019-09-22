using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient client, IConfiguration config)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (config == null) throw new ArgumentNullException(nameof(config));
            _users = client.GetDatabase(config["DBNAME"]).GetCollection<User>(DbConstants.UserCollectionName);
        }

        private void Add(User item) => _users.InsertOne(item);

        public void Add(long chatId) =>
            Add(new User
            {
                ChatId = chatId
            });

        public IEnumerable<User> All() =>
            _users.Find(_ => true).ToEnumerable();

        public User Get(long chatId) =>
            _users.Find(GlobalChatIdFilter(chatId)).SingleOrDefault();

        public DeleteResult Delete(long chatId) => _users.DeleteOne(GlobalChatIdFilter(chatId));

        public void SaveContact(long chatId, string phone, string telegramName)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Phone, phone)
                .Set(p => p.TelegramName, telegramName);

            _users.UpdateOne(filter, update);
        }

        public void SaveEmail(long chatId, string email)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.EMail, email);

            _users.UpdateOne(filter, update);
        }

        public void SaveName(long chatId, string name)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Name, name);

            _users.UpdateOne(filter, update);
        }

        public void SaveBet(long chatId, string bet)
        {
            var filter = GlobalChatIdFilter(chatId);
            var update = Builders<User>.Update
                .Set(p => p.Bet, bet)
                .Set(p=>p.IsAnsweredAll, true);

            _users.UpdateOne(filter, update);
        }
        
        private static FilterDefinition<User> GlobalChatIdFilter(long chatId)
            => Builders<User>.Filter.Eq(u => u.ChatId, chatId);
    }
}