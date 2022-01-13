using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeyvatCafe.Models
{
    public class Invoice
    {
        public Guid IdInvoice { get; set; }
        public int IdStatus { get; set; }
        public int IdAddress { get; set; }
        public Guid IdCart { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateExpired { get; set; }
        [NotMapped]
        public List<ProductCart> ProductCarts { get; set; }
        [NotMapped]
        public Address Address { get; set; }
        [NotMapped]
        public string Status { get; set; }
        [NotMapped]
        public int Total { get; set; }
        [NotMapped]
        public string UserName { get; set; }

    }
}
