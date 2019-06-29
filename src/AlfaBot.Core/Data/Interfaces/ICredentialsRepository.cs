using AlfaBot.Core.Models;

namespace AlfaBot.Core.Data.Interfaces
{
    /// <summary>
    /// User Mongodb repository
    /// </summary>
    public interface ICredentialsRepository
    {
        Credential GetSecureUser(string userName);
    }
}