using System;
using System.Collections.Generic;
using System.Text;

namespace Learning.Redis.DataAccess
{
    public interface ICustomerRepository
    {
        List<Customer> GetAll();
        void Add(Customer customer);
    }
    public class CustomerRepository: ICustomerRepository
    {
        List<Customer> _customers;
        private List<Customer> customers => _customers;
        public CustomerRepository()
        {
            _customers = new List<Customer>();
            _customers.Add(new Customer() { Code = "C01", Name = "Cliente 1", MobilePhone = "3125684790", OfficePhone = "7845263" });
            _customers.Add(new Customer() { Code = "C02", Name = "Cliente 2", MobilePhone = "3102546123", OfficePhone = "4507985" });
            _customers.Add(new Customer() { Code = "C03", Name = "Cliente 3", MobilePhone = "3112364578", OfficePhone = "8924563" });
            _customers.Add(new Customer() { Code = "C04", Name = "Cliente 4", MobilePhone = "3023165241", OfficePhone = "4223154" });
            _customers.Add(new Customer() { Code = "C05", Name = "Cliente 5", MobilePhone = "3202145787", OfficePhone = "2458695" });
        }

        public List<Customer> GetAll()
        {
            return _customers;
        }

        public void Add(Customer customer)
        {
            _customers.Add(customer);
        }
    }
}
