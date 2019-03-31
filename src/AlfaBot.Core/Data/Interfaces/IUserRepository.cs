using System.Collections.Generic;
using AlfaBot.Core.Models;
using MongoDB.Driver;

namespace AlfaBot.Core.Data.Interfaces
{
    /// <inheritdoc />
    public interface IUserRepository
    {
        IEnumerable<User> All();

        void Add(User item);
        
        void Add(long chatId);

        User Get(long chatId);

        UpdateResult SaveContact(long chatId, string phone, string telegramName);

        UpdateResult SaveEmail(long chatId, string email);

        UpdateResult SavePersonOrWorkerInfo(long chatId, bool? isStudent, bool? isAnswerAll);

        UpdateResult SaveName(long chatId, string name);

        UpdateResult SaveUniversity(long chatId, string university);

        UpdateResult SaveCourse(long chatId, string course, bool? isAnsweredAll);

        UpdateResult SaveProfession(long chatId, string profession);

        UpdateResult SetQuizMember(long chatId, bool isMember);
    }
}