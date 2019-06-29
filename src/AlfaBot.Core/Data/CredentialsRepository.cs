using System;
using System.Diagnostics.CodeAnalysis;
using AlfaBot.Core.Data.Interfaces;
using AlfaBot.Core.Models;
using MongoDB.Driver;

namespace AlfaBot.Core.Data
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class CredentialsRepository : ICredentialsRepository
    {
        private readonly IMongoCollection<Credential> _credentials;

        /// <inheritdoc />
        public CredentialsRepository(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _credentials = database.GetCollection<Credential>(DbConstants.CredentialsCollectionName);
        }

        /// <inheritdoc />
        public Credential GetSecureUser(string userName)
        {
            var filter = Builders<Credential>.Filter.Eq(u => u.UserName, userName);
            return _credentials.Find(filter).FirstOrDefault();
        }
    }
}