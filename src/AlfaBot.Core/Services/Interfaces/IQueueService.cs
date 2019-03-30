using System.Collections.Generic;
using System.Threading.Tasks;
using AlfaBot.Core.Models;

namespace AlfaBot.Core.Services.Interfaces
{
    public interface IQueueService
    {
        Task AddLowPriorityAsync(TelegramLowPriorityMessage message);

        Task AddHighPriorityAsync(TelegramHighPriorityMessage message);

        Task<List<TelegramHighPriorityMessage>> GetTopHighPriorityAsync(int limit);

        Task<List<TelegramLowPriorityMessage>> GetTopLowPriorityAsync(int limit);
    }
}