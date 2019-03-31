using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlfaBot.Core.Models;
using MongoDB.Bson;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IQueueService
    {
        void Add(QueueMessage message);

        void Dequeue(ObjectId id);

        IEnumerable<QueueMessage> GetTopHighPriority(int limit);

        IEnumerable<QueueMessage> GetTopLowPriority(int limit);

        Task<long> HighPriorityCountAsync();

        Task<long> LowPriorityCountAsync();
        
        DateTime HighPriorityTime();

        DateTime LowPriorityTime();
    }
}