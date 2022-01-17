using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreEFCoreApp.Domain.Models
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // navigation propery sayesinde ilişkili olan nesneleri birbirine joinleyebiliriz.
        public virtual Category Category { get; set; }
    }
}
