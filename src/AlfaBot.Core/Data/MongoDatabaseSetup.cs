using System;
using AlfaBot.Core.Models;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    /// <summary>
    /// This class initialized MongoDatabase
    /// </summary>
    public static class MongoDatabaseSetup
    {
        public static IMongoDatabase Init(this IMongoDatabase database)
        {
            var users = database.GetCollection<User>(DbConstants.UserCollectionName);
            var queue = database.GetCollection<QueueMessage>(DbConstants.QueueCollectionName);
            var quiz = database.GetCollection<QuizResult>(DbConstants.QuizResultCollectionName);

            var options = new CreateIndexOptions {Unique = true};

#pragma warning disable 618
            users.Indexes.CreateOne("{ChatId:1}", options);
            queue.Indexes.CreateOne("{ChatId:1}", options);
            queue.Indexes.CreateOne("{IsHighPriority:1}");
            quiz.Indexes.CreateOne("{User.ChatId:1}");
#pragma warning restore 618

            if (users.Find(_ => true).FirstOrDefault() != null)
            {
                return database;
            }

            // create new user
            var user = new User
            {
                EMail = "test@test.ru",
                Name = "Test Student",
                Profession = "Haskell",
                University = "sgu"
            };

            users.InsertOne(user);

            if (users.CountDocuments(_ => true) == 0)
                throw new SystemException("Fail to init DB");

            return database;
        }
    }
}