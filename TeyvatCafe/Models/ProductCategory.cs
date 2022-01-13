using System.ComponentModel.DataAnnotations.Schema;

namespace TeyvatCafe.Models
{
    public class ProductCategory
    {
        public int IdCategory { get; set; }
        public int IdProduct { get; set; }
        [NotMapped]

        public string CategoryName { get; set; }
    }
}
