using Learning.Redis.DataAccess;
using Learning.Redis.RedisCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Learning.Redis
{
    class Program
    {
        private static CacheContext _cacheContext;
        static void Main(string[] args)
        {
            _cacheContext = new CacheContext("localhost:6379", 2);
            // Reload rediskeys
            Reload();
            Task.Delay(2000);
            // Get and set a Customer Key
            ValidateCustomer(10);
            // Get and set a Product Key
            ValidateProduct("P10");
            // Show all Product keys values
            GetProductList();

            Console.WriteLine("Proceso terminado");

            Console.ReadKey(true);
        }

        private static async void SetCustomer(int custId)
        {
            await _cacheContext.Customers.HashSet(
                custId,
                new Customer()
                {
                    Code = string.Concat("C", custId),
                    Name = string.Concat("Customer ", custId)                    
                });

            Console.WriteLine("Datos cliente '{0}' guardados.", custId);
        }

        private static async Task<Customer> GetCustomer(int customerId)
        {
            return await _cacheContext.Customers.HashGet(customerId);
            
        }

        private static void PrintCustomer(Customer customer)
        {
            Console.WriteLine("{0}DETALLE CLIENTE{0}" +
                   "Código             : '{1}'{0}" +
                   "Nombre             : '{2}'{0}" +
                   "Celular            : '{3}'{0}" +
                   "Teléfono           : '{4}'{0}" +
                   "Cuota              : '{5}'{0}" +
                   "Tarifa             : '{6}'{0}" +
                   "Fin de suscripción : '{7:yy/MM/dd}'",
                   Environment.NewLine,
                   customer.Code,
                   customer.Name,
                   customer.MobilePhone,
                   customer.OfficePhone,
                   customer.Quota,
                   customer.Rate,
                   customer.SuscriptionDueDate);
        }

        private static async void ValidateCustomer(int custId)
        {
            if(!await _cacheContext.Customers.HashExists(custId))
                SetCustomer(custId);

            var customer = await GetCustomer(custId);
            PrintCustomer(customer);

        }

        private static async void GetProductList()
        {
            foreach (var item in await _cacheContext.Products.ListRangeAsync())
            {
                PrintProduct(item);
            }
            
        }

        private static async Task<Product> GetProduct(string code)
        {
            return await _cacheContext.Products.FirtsOrDefaultAsync(x => x.Code == code);
        }

        private static void PrintProduct(Product product)
        {
            Console.WriteLine("{0}DETALLE PRODUCTO{0}" +
                     "Id                 : '{1}'{0}" +
                     "Código             : '{2}'{0}" +
                     "Nombre             : '{3}'{0}" +
                     "Valor unitario     : '{4}'{0}"
                     ,
                     Environment.NewLine,
                     product.Id,
                     product.Code,
                     product.Name,
                     product.UnitPrice);
        }
        private static async Task<long> SetProduct(string code)
        {
            Product product = new Product() { 
                Id = Guid.NewGuid(),
                Code = code,
                Name = string.Concat("Product ",code),
                UnitPrice = new Random().Next(5, 20)
            };
            return await _cacheContext.Products.AddAsync(product);
        }

        private static async void ValidateProduct(string code)
        {
            var product = await GetProduct(code);
            if (product == null)
            {
                await SetProduct(code);
                product = await GetProduct(code);
            }
            PrintProduct(product);
        }
        private static async void Reload()
        {
            // When key is deleted
            TimeSpan keyExpiry = new TimeSpan(0,1,0);
            try
            {
                await _cacheContext.Customers.ReloadAsync(GetCustomers(), keyExpiry);
                await _cacheContext.Products.ReloadAsync(GetProducts());
                Console.WriteLine("Reload. Proceso ejecutado correctamente");
            }
            catch (Exception)
            {
                Console.WriteLine("Ocurrío un error en la carga");
            }
        }

        private static List<KeyValuePair<int,Customer>> GetCustomers()
        {
            List<KeyValuePair<int, Customer>> customers = new List<KeyValuePair<int, Customer>>
            {
                new KeyValuePair<int, Customer>(1, new Customer() { Code = "C01", Name = "Cliente 1", MobilePhone = "3125684790", OfficePhone = "7845263" }),
                new KeyValuePair<int, Customer>(2, new Customer() { Code = "C02", Name = "Cliente 2", MobilePhone = "3102546123", OfficePhone = "4507985" }),
                new KeyValuePair<int, Customer>(3, new Customer() { Code = "C03", Name = "Cliente 3", MobilePhone = "3112364578", OfficePhone = "8924563", Rate = 3.5m }),
                new KeyValuePair<int, Customer>(4, new Customer() { Code = "C04", Name = "Cliente 4", MobilePhone = "3023165241", OfficePhone = "4223154" }),
                new KeyValuePair<int, Customer>(5, new Customer() { Code = "C05", Name = "Cliente 5", MobilePhone = "3202145786", OfficePhone = "2458695", SuscriptionDueDate = DateTime.Now })
            };
            return customers;
        }

        private static List<Product> GetProducts()
        {
            List<Product> products = new List<Product>
            {
                new Product() { Id = Guid.NewGuid(), Code = "P01", Name = "Product 1", UnitPrice = 10},
                new Product() { Id = Guid.NewGuid(), Code = "P02", Name = "Product 2", UnitPrice = 12},
                new Product() { Id = Guid.NewGuid(), Code = "P03", Name = "Product 3", UnitPrice = 18},
                new Product() { Id = Guid.NewGuid(), Code = "P04", Name = "Product 4", UnitPrice = 15},
                new Product() { Id = Guid.NewGuid(), Code = "P05", Name = "Product 5", UnitPrice = 8}
            };
            return products;
        }
    }
}
