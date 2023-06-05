using System;
using System.Collections.Generic;
using System.Text;

namespace Learning.Redis.DataAccess
{
    public interface IDbContext
    {
        ICustomerRepository Customers { get; }
    }
    public class DbContext : IDbContext
    {
        ICustomerRepository _customers;
        public ICustomerRepository Customers { 
            get
            {
                if (_customers == null)
                    _customers = new CustomerRepository();
                return _customers;
            }
}
    }
}
