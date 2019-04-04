using System.Collections.Generic;
using AlfaBot.Core.Models;

namespace AlfaBot.Core.Data.Interfaces
{
    /// <summary>
    /// User Mongodb repository
    /// </summary>
    public interface IUserRepository
    {
        IEnumerable<User> All();

        void Add(User item);
        
        void Add(long chatId);

        User Get(long chatId);

        void SaveContact(long chatId, string phone, string telegramName);

        void SaveEmail(long chatId, string email);

        void SavePersonOrWorkerInfo(long chatId, bool? isStudent, bool? isAnswerAll);

        void SaveName(long chatId, string name);

        void SaveUniversity(long chatId, string university);

        void SaveCourse(long chatId, string course, bool? isAnsweredAll);

        void SaveProfession(long chatId, string profession);

        void SetQuizMember(long chatId, bool isMember);
    }
}