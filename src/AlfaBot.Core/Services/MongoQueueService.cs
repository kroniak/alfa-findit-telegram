using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlfaBot.Core.Data;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AlfaBot.Core.Services
{
    /// <inheritdoc />
    public class MongoQueueService : IQueueService
    {
        private readonly IMongoCollection<TelegramLowPriorityMessage> _lowQueue;
        private readonly IMongoCollection<TelegramHighPriorityMessage> _highQueue;

        public MongoQueueService(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _lowQueue = database.GetCollection<TelegramLowPriorityMessage>(DbConstants.LowPriorityQueueCollectionName);
            _highQueue =
                database.GetCollection<TelegramHighPriorityMessage>(DbConstants.HighPriorityQueueCollectionName);
        }

        public async Task AddLowPriorityAsync(TelegramLowPriorityMessage message)
        {
            if (await CheckMessageExistAsync(_lowQueue, message.ChatId) == false)
                await _lowQueue.InsertOneAsync(message);
        }

        public async Task AddHighPriorityAsync(TelegramHighPriorityMessage message)
        {
            if (await CheckMessageExistAsync(_highQueue, message.ChatId) == false)
                await _highQueue.InsertOneAsync(message);
        }

        public Task<List<TelegramHighPriorityMessage>> GetTopHighPriorityAsync(int limit) =>
            GetTopMessagesAsync(_highQueue, limit);

        public Task<List<TelegramLowPriorityMessage>> GetTopLowPriorityAsync(int limit)
            => GetTopMessagesAsync(_lowQueue, limit);

        private Task<List<T>> GetTopMessagesAsync<T>(IMongoCollection<T> collection, int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));

            var sort = new BsonDocument("_id", 1);

            return collection.Find(_ => true).Sort(sort).Limit(limit).ToListAsync();
        }

        private static async Task<bool> CheckMessageExistAsync<T>(IMongoCollection<T> collection, long chatId)
        {
            var count = await collection.CountDocumentsAsync(GetGlobalFilter(chatId));
            return count > 0;
        }

        private static BsonDocument GetGlobalFilter(long chatId)
            => new BsonDocument("ChatId", chatId);
    }
}