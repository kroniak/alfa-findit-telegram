using System.Collections.Generic;
using AlfaBot.Core.Models;
using MongoDB.Bson;

namespace AlfaBot.Core.Data.Interfaces
{
    public interface IQuestionRepository
    {
        int Count { get; }

        void Add(Question question);

        Question Get(ObjectId id);

        IEnumerable<Question> All();
    }
}