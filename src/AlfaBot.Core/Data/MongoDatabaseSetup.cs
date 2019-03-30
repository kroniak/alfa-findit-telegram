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

            if (users.CountDocuments(_ => true) > 0)
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