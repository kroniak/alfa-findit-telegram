using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    /// <summary>
    /// This class initialized MongoDatabase
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class MongoDatabaseSetup
    {
        public static IMongoDatabase Init(this IMongoDatabase database, string adminPass, string userPass)
        {
            var users = database.GetCollection<User>(DbConstants.UserCollectionName);
            var queue = database.GetCollection<QueueMessage>(DbConstants.QueueCollectionName);
            var results = database.GetCollection<QuizResult>(DbConstants.QuizResultCollectionName);
            var logs = database.GetCollection<LogRecord>(DbConstants.LogCollectionName);
            var questions = database.GetCollection<Question>(DbConstants.QuestionCollectionName);
            var credentials = database.GetCollection<Credential>(DbConstants.CredentialsCollectionName);

            try
            {
                CreateIndexes(credentials, users, queue, results, logs);
                if (questions.Find(_ => true).FirstOrDefault() == null)
                {
                    InsertQuestion(questions);
                }

                if (credentials.Find(_ => true).FirstOrDefault() == null)
                {
                    InsertCredentials(credentials, adminPass, userPass);
                }
            }
            catch (Exception e)
            {
                throw new SystemException("Fail to init DB", e);
            }

            return database;
        }

        private static void InsertCredentials(
            IMongoCollection<Credential> credentials,
            string adminPass,
            string userPass)
        {
            var cs = new[]
            {
                new Credential
                {
                    UserName = "admin",
                    Role = "Administrators"
                },
                new Credential
                {
                    UserName = "user",
                    Role = "User"
                }
            };

            foreach (var credential in cs)
            {
                credential.HashedPassword = GetHash(credential, adminPass, userPass);
            }

            credentials.InsertMany(cs);
        }

        private static string GetHash(Credential credential, string adminPass,
            string userPass) =>
            new PasswordHasher<Credential>().HashPassword(credential,
                credential.Role == "Administrators" ? adminPass : userPass);

        private static void InsertQuestion(IMongoCollection<Question> questions)
        {
            var qs = new[]
            {
                new Question
                {
                    Point = 10,
                    Answer = "1993",
                    IsPicture = false,
                    Message = "В каком году был основан Альфа-Банк?"
                },
                new Question
                {
                    Point = 10,
                    Answer = "A",
                    IsPicture = true,
                    Message = "http://alfa-it-bot-qa.s3-website.eu-central-1.amazonaws.com/1.png"
                }
            };

            questions.InsertMany(qs);
        }

        private static void CreateIndexes(
            IMongoCollection<Credential> credentials,
            IMongoCollection<User> users,
            IMongoCollection<QueueMessage> queue,
            IMongoCollection<QuizResult> results,
            IMongoCollection<LogRecord> logs)
        {
            var optionsUnique = new CreateIndexOptions
            {
                Unique = true,
                Background = true
            };

            var optionsBackground = new CreateIndexOptions
            {
                Background = true
            };

            var credentialIndexModel = new CreateIndexModel<Credential>(
                Builders<Credential>.IndexKeys.Ascending(x => x.UserName),
                optionsUnique);

            var userIndexModel = new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(x => x.ChatId),
                optionsUnique);

            var queueIndexModelFirst = new CreateIndexModel<QueueMessage>(
                Builders<QueueMessage>.IndexKeys
                    .Ascending(x => x.ChatId), optionsUnique);

            var queueIndexModelSecond = new CreateIndexModel<QueueMessage>(
                Builders<QueueMessage>.IndexKeys
                    .Ascending(x => x.IsHighPriority), optionsBackground);

            var resultsIndexModel = new CreateIndexModel<QuizResult>(
                Builders<QuizResult>.IndexKeys.Ascending(x => x.User.ChatId),
                optionsUnique);

            var logIndexModelFirst = new CreateIndexModel<LogRecord>(
                Builders<LogRecord>.IndexKeys
                    .Ascending(x => x.MessageId), optionsUnique);

            var logIndexModelSecond = new CreateIndexModel<LogRecord>(
                Builders<LogRecord>.IndexKeys
                    .Ascending(x => x.ChatId), optionsBackground);

            credentials.Indexes.CreateOne(credentialIndexModel);
            users.Indexes.CreateOne(userIndexModel);
            queue.Indexes.CreateMany(new[] {queueIndexModelFirst, queueIndexModelSecond});
            results.Indexes.CreateOne(resultsIndexModel);
            logs.Indexes.CreateMany(new[] {logIndexModelFirst, logIndexModelSecond});
        }
    }
}