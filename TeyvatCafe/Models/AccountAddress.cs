using System.ComponentModel.DataAnnotations.Schema;

namespace TeyvatCafe.Models
{
    public class AccountAddress
    {
        public string IdAccount { get; set; }
        public int IdAddress { get; set; }
        public bool? IsDefault { get; set; }
        [NotMapped]
        public string Adresss { get; set; }
        [NotMapped]
        public string PhoneNumber { get; set; }
        [NotMapped]
        public string Receiver { get; set; }
    }
}
