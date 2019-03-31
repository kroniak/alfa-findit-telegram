using System;
using System.Collections.Generic;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Data
{
    /// <inheritdoc />
    public class LogRepository : ILogRepository
    {
        private readonly ILogger<LogRepository> _logger;
        private readonly IMongoCollection<LogRecord> _log;

        public LogRepository(
            IMongoDatabase database,
            ILogger<LogRepository> logger)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _log = database.GetCollection<LogRecord>(DbConstants.LogCollectionName);
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

        public IEnumerable<LogRecord> All() =>
            _log.Find(_ => true).ToEnumerable();

        public IEnumerable<LogRecord> GetRecords(long chatId) =>
            _log.Find(GlobalChatIdFilter(chatId)).ToEnumerable();

        public IEnumerable<LogRecord> GetRecords(int messageId) =>
            _log.Find(GlobalMessageIdFilter(messageId)).ToEnumerable();

        public void SaveQueueMessage(int messageId, QueueMessage queueMessage, DateTime end)
        {
            var filter = GlobalMessageIdFilter(messageId);
            var update = Builders<LogRecord>.Update
                .Set(p => p.QueueMessage, queueMessage)
                .Set(p => p.End, end);

            _log.UpdateOne(filter, update);
        }

        private static FilterDefinition<LogRecord> GlobalChatIdFilter(long chatId)
            => Builders<LogRecord>.Filter.Eq(u => u.ChatId, chatId);

        private static FilterDefinition<LogRecord> GlobalMessageIdFilter(int messageId)
            => Builders<LogRecord>.Filter.Eq(u => u.MessageId, messageId);
    }
}