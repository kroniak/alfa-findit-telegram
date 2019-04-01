using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AlfaBot.Core.Data;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using AlfaBot.Core.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace AlfaBot.Core.Services
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public class MongoQueueService : IQueueService
    {
        private readonly ILogRepository _logRepository;
        private readonly IMongoCollection<QueueMessage> _queue;
        private readonly IMongoQueryable<QueueMessage> _queryable;

        public MongoQueueService(
            IMongoDatabase database,
            ILogRepository logRepository)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));

            _queue = database.GetCollection<QueueMessage>(DbConstants.QueueCollectionName);
            _queryable = _queue.AsQueryable();

            BsonClassMap.RegisterClassMap<ReplyKeyboardMarkup>();
            BsonClassMap.RegisterClassMap<ReplyKeyboardRemove>();
        }

        public void Add(QueueMessage message)
        {
            if (CheckMessageExist(message.ChatId) == null)
            {
                _queue.InsertOne(message);
                _logRepository.SaveQueuedTime(message.IncomeMessageId, DateTime.Now);
            }
            else
            {
                _logRepository.SaveEndedTime(message.IncomeMessageId, DateTime.Now);
            }
        }

        public void Dequeue(ObjectId id)
        {
            _queue.FindOneAndDelete(new BsonDocument("_id", id));
        }

        public IEnumerable<QueueMessage> GetTopHighPriority(int limit) =>
            GetTopMessages(true, limit);

        public IEnumerable<QueueMessage> GetTopLowPriority(int limit)
            => GetTopMessages(false, limit);

        public Task<long> HighPriorityCountAsync() => _queue.CountDocumentsAsync(PriorityFilter(true));

        public Task<long> LowPriorityCountAsync() => _queue.CountDocumentsAsync(PriorityFilter(false));

        public DateTime HighPriorityTime() => PriorityTime(true);

        public DateTime LowPriorityTime() => PriorityTime(false);

        private DateTime PriorityTime(bool isPriority)
        {
            var id = _queryable
                .Where(q => q.IsHighPriority == isPriority)
                .OrderBy(q => q.Id)
                .Select(q => q.Id).FirstOrDefault();

            return id.CreationTime;
        }

        private IEnumerable<QueueMessage> GetTopMessages(bool isPriority, int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));

            var sort = Builders<QueueMessage>.Sort.Ascending(m => m.Id);

            return _queue.Find(PriorityFilter(isPriority)).Sort(sort).Limit(limit).ToEnumerable();
        }

        private QueueMessage CheckMessageExist(long chatId) =>
            _queue.Find(GlobalChatIdFilter(chatId)).SingleOrDefault();

        private static FilterDefinition<QueueMessage> GlobalChatIdFilter(long chatId)
            => Builders<QueueMessage>.Filter.Eq(x => x.ChatId, chatId);

        private static FilterDefinition<QueueMessage> PriorityFilter(bool isPriority)
            => Builders<QueueMessage>.Filter.Eq(x => x.IsHighPriority, isPriority);
    }
}