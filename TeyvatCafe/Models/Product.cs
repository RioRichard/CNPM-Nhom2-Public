using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeyvatCafe.Models
{
    public class Product
    {
        

        public int? IdProduct { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int? Stock { get; set; }
        public string ImageUrl { get; set; }
        public bool? IsDelete { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public List<ProductAttribute> ProductAttributes { get; set; }
        [NotMapped]
        public List<ProductCategory> Category { get; set; }

    }
}
