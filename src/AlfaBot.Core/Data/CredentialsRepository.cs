using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class CredentialsRepository : ICredentialsRepository
    {
        private readonly IMongoCollection<Credential> _credentials;

        /// <inheritdoc />
        public CredentialsRepository(IMongoClient client, IConfiguration config)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (config == null) throw new ArgumentNullException(nameof(config));
            _credentials = client.GetDatabase(config["DBNAME"]).GetCollection<Credential>(DbConstants.CredentialsCollectionName);
        }

        /// <inheritdoc />
        public Credential GetSecureUser(string userName)
        {
            var filter = Builders<Credential>.Filter.Eq(u => u.UserName, userName);
            return _credentials.Find(filter).FirstOrDefault();
        }
    }
}