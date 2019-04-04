using System.Collections.Generic;
using AlfaBot.Core.Models;
using MongoDB.Bson;

namespace AlfaBot.Core.Data.Interfaces
{
    public interface IQuestionRepository
    {
        int Count { get; }

        Question Get(ObjectId id);

        void Update();

        IEnumerable<Question> All();
    }
}