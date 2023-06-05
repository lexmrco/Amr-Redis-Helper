using System;
using System.Collections.Generic;
using System.Text;

namespace Learning.Redis.RedisCache.Contracts
{
    public interface ICacheCustomerRepository: IBaseHashRepository<int, Customer>
    {
    }
}
