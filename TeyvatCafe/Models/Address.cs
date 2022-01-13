using System.ComponentModel.DataAnnotations.Schema;

namespace TeyvatCafe.Models
{
    public class Address
    {

        public int? IdAddress { get; set; }
        [Column("Address")]
        public string Addressed { get; set; }
        public string Phone { get; set; }
        public string Receiver { get; set; }
        

    }
}
