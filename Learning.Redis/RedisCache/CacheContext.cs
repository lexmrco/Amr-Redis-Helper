using Learning.Redis.RedisCache.Contracts;
using Learning.Redis.RedisCache.Repository;
using StackExchange.Redis;
using System;

namespace Learning.Redis.RedisCache
{
    public class CacheContext: ICacheContext
    {
        #region Properties

        private readonly ConnectionMultiplexer _redisConnection;

        private readonly IDatabase _database;

        #endregion   
        public CacheContext(string connection, int database)
        {
            _redisConnection = ConnectionMultiplexer.Connect(connection); ;
            _database = _redisConnection.GetDatabase(database);
        }

        #region Repositories

        ICacheCustomerRepository _customers;
        public ICacheCustomerRepository Customers {
            get
            {
                if (_customers == null)
                    _customers = new CustomerRepository(_database);
                return _customers;
            }
        }
        ICacheProductRepository _products;
        public ICacheProductRepository Products
        {
            get
            {
                if (_products == null)
                    _products = new ProductRepository(_database);
                return _products;
            }
        }
        #endregion

        private  void Close()
        {
            _redisConnection.Dispose();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
