using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Learning.Redis.RedisCache.Contracts
{
    public interface IBaseHashRepository<TKey, TValue>
    {
        Task<bool> HashExists(TKey key);
        Task<TValue> HashGet(TKey key);
        Task HashSet(TKey key, TValue value, TimeSpan? expiry = null);
        Task ReloadAsync(List<KeyValuePair<TKey, TValue>> data, TimeSpan? expiry = null);
    }
}
