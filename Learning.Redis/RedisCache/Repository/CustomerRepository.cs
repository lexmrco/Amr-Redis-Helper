using Learning.Redis.RedisCache.Contracts;
using StackExchange.Redis;
using System;

namespace Learning.Redis.RedisCache.Repository
{
    public class CustomerRepository : BaseHashRepository<int, Customer>, ICacheCustomerRepository
    {
        protected override string Namespace => "customers";

        public CustomerRepository(IDatabase database) : base(database)
        {

        }
    }
}
