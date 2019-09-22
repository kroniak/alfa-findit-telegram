using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Data
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class LogRepository : ILogRepository
    {
        private readonly ILogger<LogRepository> _logger;
        private readonly IMongoCollection<LogRecord> _log;
        private readonly SortDefinition<LogRecord> _sortDesc = Builders<LogRecord>.Sort.Descending(l => l.Id);

        public LogRepository(
            IMongoClient client,
            ILogger<LogRepository> logger,
            IConfiguration config)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (config == null) throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _log = client.GetDatabase(config["DBNAME"]).GetCollection<LogRecord>(DbConstants.LogCollectionName);
        }
        
        public void Add(Message message)
        {
            var record = new LogRecord(message);
            try
            {
                _log.InsertOne(record);
            }
            catch
            {
                _logger.LogError($"[{message.Chat.Id}] [{message.Text ?? ""}] Log message failed");
            }
        }

        public IEnumerable<LogRecord> All(int? top) =>
            !top.HasValue ? 
                _log.Find(_ => true).ToEnumerable() : 
                _log.Find(_ => true).Sort(_sortDesc).Limit(top.Value).ToEnumerable();

        public IEnumerable<LogRecord> GetRecords(long chatId, int? top) =>
            !top.HasValue ? 
                _log.Find(GlobalChatIdFilter(chatId)).ToEnumerable() : 
                _log.Find(GlobalChatIdFilter(chatId)).Sort(_sortDesc).Limit(top.Value).ToEnumerable();

        public IEnumerable<LogRecord> GetRecords(int messageId) =>
            _log.Find(GlobalMessageIdFilter(messageId)).ToEnumerable();

        public void SaveQueueMessage(int messageId, QueueMessage queueMessage, DateTime end)
        {
            var filter = GlobalMessageIdFilter(messageId);
            var update = Builders<LogRecord>.Update
                .Set(p => p.QueueMessage, queueMessage)
                .Set(p => p.Ended, end);

            _log.UpdateOne(filter, update);
        }

        public void SaveQueuedTime(int messageId, DateTime queued)
        {
            var filter = GlobalMessageIdFilter(messageId);
            var update = Builders<LogRecord>.Update
                .Set(p => p.Queued, queued);

            _log.UpdateOne(filter, update);
        }

        public void SaveEndedTime(int messageId, DateTime ended)
        {
            var filter = GlobalMessageIdFilter(messageId);
            var update = Builders<LogRecord>.Update
                .Set(p => p.Ended, ended);

            _log.UpdateOne(filter, update);
        }

        private static FilterDefinition<LogRecord> GlobalChatIdFilter(long chatId)
            => Builders<LogRecord>.Filter.Eq(u => u.ChatId, chatId);

        private static FilterDefinition<LogRecord> GlobalMessageIdFilter(int messageId)
            => Builders<LogRecord>.Filter.Eq(u => u.MessageId, messageId);
    }
}