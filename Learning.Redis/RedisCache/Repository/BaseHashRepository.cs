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
    public abstract class BaseHashRepository<TKey,TValue>: IBaseHashRepository<TKey, TValue>
    {
        #region properties
        private readonly IDatabase _database;
        /// <summary>
        /// Name of the Redis key 
        /// </summary>
        protected abstract string Namespace { get; }

        #endregion endproperties

        public BaseHashRepository(IDatabase database)
        {
            _database = database;
        }

        #region Private Methods
        private HashEntry[] ObjectToHashEntries(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Select
                (
                      property =>
                      {
                          object propertyValue = property.GetValue(obj);
                          string hashValue;

                          if (propertyValue is IEnumerable<object>)
                          {
                              hashValue = JsonConvert.SerializeObject(propertyValue);
                          }
                          else
                          {
                              hashValue = propertyValue?.ToString();
                          }

                          return new HashEntry(property.Name, hashValue);
                      }
                )
                .ToArray();
        }

        private TValue HasEntriesToObject(HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(TValue).GetProperties();
            var obj = Activator.CreateInstance(typeof(TValue));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;

                var propertyType = property.PropertyType;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = propertyType.GetGenericArguments()[0];
                }
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), propertyType));
            }
            return (TValue)obj;
        }

        #endregion

        #region Public Methods  

        public async Task<bool> HashExists(TKey key)
        {
            string keyName = string.Format("{0}:{1}", Namespace, key);
            return await _database.KeyExistsAsync(keyName);
        }

        public async Task<TValue> HashGet(TKey key)
        {
            string keyName = string.Format("{0}:{1}", Namespace, key);
            return HasEntriesToObject(await _database.HashGetAllAsync(keyName));
        }

        public async Task HashSet(TKey key, TValue value, TimeSpan? expiry = null)
        {
            string keyName = string.Format("{0}:{1}", Namespace, key);
            var hashProperties = ObjectToHashEntries(value);

            var hashFieldsToSet = hashProperties.Where(x => x.Value.HasValue)?.ToArray();
            var hashFieldsToRemove = hashProperties.Where(x => !x.Value.HasValue)?.ToArray();

            await _database.HashSetAsync(keyName, hashFieldsToSet);
            if (expiry.HasValue)
                await _database.KeyExpireAsync(keyName, expiry.Value);

            if (hashFieldsToRemove != null)
                foreach (var item in hashFieldsToRemove)
                {
                    await _database.HashDeleteAsync(keyName, item.Name);
                }
        }
        public async Task ReloadAsync(List<KeyValuePair<TKey,TValue>> data, TimeSpan? expiry = null)
        {
            foreach (var item in data)
            {
                await HashSet(item.Key, item.Value, expiry);
            }
        }        
        #endregion
    }
}
