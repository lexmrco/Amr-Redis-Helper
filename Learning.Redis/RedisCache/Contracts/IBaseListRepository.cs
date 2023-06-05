using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Learning.Redis.RedisCache.Contracts
{
    public interface IBaseListRepository<T>
    {
        /// <summary>
        /// Add item to list key value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Total items in the key value</returns>
        Task<long> AddAsync(T item);

        /// <summary>
        /// Gets all the items stored in the redis key
        /// </summary>
        /// <returns>List Iterator</returns>
        IAsyncEnumerator<T> GetAsyncEnumerator();

        /// <summary>
        /// Gets all the items stored in the redis key
        /// </summary>
        /// <param name="start">Start range. From the list begins </param>
        /// <param name="stop">Stop range. Index to stop, last in the list is -1, -2,-3 ...</param>
        /// <returns></returns>
        Task<List<T>> ListRangeAsync(long start = 0, long stop = -1);


        /// <summary>
        /// Search a item in the list of the redis value
        /// </summary>
        /// <param name="filter">Search criteria</param>
        /// <returns>Returns the item that meets the search criteria</returns>
        Task<T> FirtsOrDefaultAsync(Func<T, bool> filter);

        /// <summary>
        /// Count items of the key value
        /// </summary>
        /// <returns>Items quantity in the key</returns>
        Task<long> CountAsync();

        /// <summary>
        /// Stores the data in redis key when exists differences
        /// </summary>
        /// <param name="data">Data to store in redis</param>
        Task ReloadAsync(List<T> data);
    }
}
