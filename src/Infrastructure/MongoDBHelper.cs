using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FindAlfaITBot.Models;
using MongoDB.Driver;

namespace FindAlfaITBot.Infrastructure
{
    public class MongoDBHelper
    {
        private static string _connectionString = "mongodb://db";
        private static string _dbName = "FindIT";

        private static IMongoDatabase _database;

        private static MongoClient _client;

        public static string GetConnectionName => $"{_connectionString}:{_dbName}";

        public static void Configure(string connectionString, string dbName)
        {
            _connectionString = connectionString;
            _dbName = dbName;
        }

        public static void ConfigureConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void ConfigureDB(string dbName)
        {
            _dbName = dbName;
        }

        public static MongoClient Client
            => _client ?? (_client = new MongoClient(_connectionString));

        public static IMongoDatabase Database
            => _database ?? (_database = Client.GetDatabase(_dbName));
    }
}
