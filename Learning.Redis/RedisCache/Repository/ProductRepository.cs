using Learning.Redis.RedisCache.Contracts;
using StackExchange.Redis;
using System;

namespace Learning.Redis.RedisCache.Repository
{
    public class ProductRepository : BaseListRepository<Product>, ICacheProductRepository
    {
        protected override string KeyName => "products";

        public ProductRepository(IDatabase database) : base(database)
        {

        }
    }
}
