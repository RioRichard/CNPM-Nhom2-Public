using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeyvatCafe.Models
{
    public class ProductCart
    {
        public int IdProduct { get; set; }
        public Guid IdCart { get; set; }
        public int Quantity { get; set; }
        public int PaymentPrice { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string UrlImage { get; set; }
        [NotMapped]
        public int? Stock { get; set; }
        [NotMapped]
        public int? Total { get; set; }
        
        

    }
}
