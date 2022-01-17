using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreEFCoreApp.Domain.Models
{
    public class Category
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }

        /// <summary>
        /// Navigation Property
        /// </summary>
        public virtual List<Product> Products { get; set; }

    }
}
