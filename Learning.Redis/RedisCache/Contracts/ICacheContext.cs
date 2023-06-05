using System;

namespace Learning.Redis.RedisCache.Contracts
{
    public interface ICacheContext: IDisposable
    {
        ICacheCustomerRepository Customers { get; }
        ICacheProductRepository Products { get; }
    }
}
