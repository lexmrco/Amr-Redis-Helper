using System;
using System.Collections.Generic;
using System.Text;

namespace Learning.Redis
{
    public class Customer
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string MobilePhone { get; set; }
        public string OfficePhone { get; set; }
        public long? Quota { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? SuscriptionDueDate { get; set; }

    }
}
