using System;
using System.Collections.Generic;
using System.Text;

namespace Learning.Redis
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }        

    }
}
