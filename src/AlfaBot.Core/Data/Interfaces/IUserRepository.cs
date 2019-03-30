using System.Threading.Tasks;
using AlfaBot.Core.Models;
using MongoDB.Driver;

namespace AlfaBot.Core.Data.Interfaces
{
    /// <inheritdoc />
    public interface IUserRepository : IRepository<User>
    {
        Task AddAsync(long chatId);

        Task<User> GetAsync(long chatId);

        Task<User> GetWithQuestionAsync(long chatId);

        Task<UpdateResult> SaveContactAsync(long chatId, string phone, string telegramName);

        Task<UpdateResult> SaveEmailAsync(long chatId, string email);

        Task<UpdateResult> SavePersonOrWorkerInfoAsync(long chatId, bool? isStudent, bool? isAnswerAll);

        Task<UpdateResult> SaveNameAsync(long chatId, string name);

        Task<UpdateResult> SaveUniversityAsync(long chatId, string university);

        Task<UpdateResult> SaveCourseAsync(long chatId, string course, bool? isAnsweredAll);

        Task<UpdateResult> SaveProfessionAsync(long chatId, string profession);
    }
}