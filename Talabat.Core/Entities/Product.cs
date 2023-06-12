using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        // Foreign Key
        public int ProductBrandId { get; set; }

        // Navigational Property [ONE]
        public ProductBrand ProductBrand { get; set; }

        // Foreign Key
        public int ProductTypeId { get; set; }

        // Navigational Property [ONE]
        public ProductType ProductType { get; set; }
    }
}
