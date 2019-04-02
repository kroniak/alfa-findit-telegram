using System;
using System.Collections.Generic;
using AlfaBot.Core.Models;
using Telegram.Bot.Types;

namespace AlfaBot.Core.Data.Interfaces
{
    public interface ILogRepository
    {
        void Add(Message message);

        IEnumerable<LogRecord> All();

        IEnumerable<LogRecord> GetRecords(long chatId);

        IEnumerable<LogRecord> GetRecords(int messageId);

        void SaveQueueMessage(int messageId, QueueMessage queueMessage, DateTime end);

        void SaveQueuedTime(int messageId, DateTime queued);

        void SaveEndedTime(int messageId, DateTime ended);
    }
}