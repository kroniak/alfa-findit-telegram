using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlfaBot.Core.Models
{
    /// <summary>
    /// Credential
    /// </summary>
    public class Credential
    {
        [BsonId] 
        // ReSharper disable once UnusedMember.Global
        public ObjectId Id { get; set; }
        
        /// <summary>
        /// UserName for login
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Not hashed password for login
        /// </summary>
        public string HashedPassword { get; set; }
        
        /// <summary>
        /// User Role
        /// </summary>
        public string Role { get; set; }
    }
}