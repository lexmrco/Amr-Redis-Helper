using Learning.Redis.RedisCache.Contracts;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Learning.Redis.RedisCache.Repository
{
    public abstract class BaseListRepository<T> : IBaseListRepository<T>
    {
        #region properties
        private readonly IDatabase _database;
        /// <summary>
        /// Name of the Redis key 
        /// </summary>
        protected abstract string KeyName { get; }

        /// <summary>
        /// Name of the redis key when runs Reload method
        /// </summary>
        private string ReloadKeyName { get { return string.Concat(KeyName, "-reload"); } }


        #endregion endproperties

        public BaseListRepository(IDatabase database)
        {
            _database = database;
        }
        #region Private Methods
        /// <summary>
        /// Gets a row of the redis value using it index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Value as string</returns>
        private async Task<string> ListGetByIndexAsync(long index)
        {
            return (await _database.ListGetByIndexAsync(KeyName, index)).ToString();
        }
        private string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private T Deserialize(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }
        /// <summary>
        /// Add item to redis list value, using a reload key.Used in reload
        /// </summary>
        /// <param name="items"></param>
        /// <returns>true when all items were added</returns>
        private async Task<bool> AddListToReloadKeyAsync(List<T> items)
        {
            await ClearReloadKey();
            long itemsToAdd = items.Count;
            long itemsAdded = 0;
            foreach (var item in items)
            {
                await _database.ListRightPushAsync(ReloadKeyName, Serialize(item));
                itemsAdded++;
            }
            return itemsAdded == itemsToAdd;
        }

        private async Task<bool> ClearMainKey()
        {
            return await _database.KeyDeleteAsync(KeyName);
        }

        private async Task<bool> ClearReloadKey()
        {
            return await _database.KeyDeleteAsync(ReloadKeyName);
        }

        /// <summary>
        /// Copy the reload key value to main key value, clearing first the main key value
        /// </summary>
        /// <returns>when is equal return true</returns>
        private async Task CopyReloadToMain()
        {
            RedisKey reloadKey = new RedisKey(ReloadKeyName);
            RedisKey mainKey = new RedisKey(KeyName);
            byte[] dumpMain = default;

            var dumpReload = await _database.KeyDumpAsync(reloadKey);
            bool mainExists = await _database.KeyExistsAsync(mainKey);

            if (mainExists)
                dumpMain = await _database.KeyDumpAsync(KeyName);

            if (dumpMain == null || !dumpMain.SequenceEqual(dumpReload))
            {
                if (mainExists && !await ClearMainKey())
                    throw new Exception("Error clearing the main key.  Copy reload key value to main key value.");
                await _database.KeyRestoreAsync(mainKey, dumpReload);
            }

        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Add item to list key value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Total items in the key value</returns>
        public async Task<long> AddAsync(T item)
        {
            return await _database.ListRightPushAsync(KeyName, Serialize(item));
        }

        /// <summary>
        /// Count items of the key value
        /// </summary>
        /// <returns>Items quantity in the key</returns>
        public async Task<long> CountAsync()
        {
            return await _database.ListLengthAsync(KeyName);
        }

        /// <summary>
        /// Search a item in the list of the redis value
        /// </summary>
        /// <param name="filter">Search criteria</param>
        /// <returns>Returns the item that meets the search criteria</returns>
        public async Task<T> FirtsOrDefaultAsync(Func<T, bool> filter)
        {
            //var asyncEnumerator = GetAsyncEnumerator();
            //bool moveNext = await asyncEnumerator.MoveNextAsync();
            //while (moveNext)
            //{
            //    var customer = asyncEnumerator.Current;
            //    if (filter(customer))
            //        return customer;
            //    moveNext = await asyncEnumerator.MoveNextAsync();

            //};
            return (await ListRangeAsync()).FirstOrDefault(filter);
        }

        /// <summary>
        /// Stores the data in redis key when exists differences
        /// </summary>
        /// <param name="data">Data to store in redis</param>
        /// <returns>true when success</returns>
        public async Task ReloadAsync(List<T> data)
        {
            if (!await AddListToReloadKeyAsync(data))
                throw new Exception("Redis Cache. Reload. Could not be inserted into second key");

            await CopyReloadToMain();

            if (!await ClearReloadKey())
                throw new Exception("Redis Cache. Reload. Error clearing the Reload key");
        }

        /// <summary>
        /// Gets all the items stored in the redis key
        /// </summary>
        /// <returns>List Iterator</returns>
        public async IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            for (int i = 0; i < await CountAsync(); i++)
            {
                yield return Deserialize(await ListGetByIndexAsync(i));
            }
        }

        public async Task<List<T>> ListRangeAsync(long start = 0, long stop = -1)
        {
            return (await _database.ListRangeAsync(KeyName, start, stop)).Select(x => Deserialize(x)).ToList();
        }

        #endregion
    }
}
