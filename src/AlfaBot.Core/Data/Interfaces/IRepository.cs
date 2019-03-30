using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlfaBot.Core.Data.Interfaces
{
    /// <summary>
    /// Generic repository for Mongodb
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> All();

        Task AddAsync(T item);
    }
}