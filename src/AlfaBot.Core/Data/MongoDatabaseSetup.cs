using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Models;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    /// <summary>
    /// This class initialized MongoDatabase
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class MongoDatabaseSetup
    {
        public static IMongoDatabase Init(this IMongoDatabase database)
        {
            var users = database.GetCollection<User>(DbConstants.UserCollectionName);
            var queue = database.GetCollection<QueueMessage>(DbConstants.QueueCollectionName);
            var results = database.GetCollection<QuizResult>(DbConstants.QuizResultCollectionName);

            CreateIndexes(users, queue, results);

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

        private static void CreateIndexes(
            IMongoCollection<User> users,
            IMongoCollection<QueueMessage> queue,
            IMongoCollection<QuizResult> results)
        {
            var options = new CreateIndexOptions
            {
                Unique = true,
                Background = true
            };

            var userIndexModel = new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(x => x.ChatId),
                options);

            var queueIndexModel1 = new CreateIndexModel<QueueMessage>(
                Builders<QueueMessage>.IndexKeys
                    .Ascending(x => x.ChatId), options);

            var queueIndexModel2 = new CreateIndexModel<QueueMessage>(
                Builders<QueueMessage>.IndexKeys
                    .Ascending(x => x.IsHighPriority));

            var resultsIndexModel = new CreateIndexModel<QuizResult>(
                Builders<QuizResult>.IndexKeys.Ascending(x => x.User.ChatId),
                options);

            users.Indexes.CreateOne(userIndexModel);
            queue.Indexes.CreateMany(new[] {queueIndexModel1, queueIndexModel2});
            results.Indexes.CreateOne(resultsIndexModel);
        }
    }
}