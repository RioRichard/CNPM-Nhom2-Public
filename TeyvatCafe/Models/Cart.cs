using System;

namespace TeyvatCafe.Models
{
    public class Cart
    {
        public Guid IdCart { get; set; }
        public bool? IsExpired { get; set; }
        public string IdAccount { get; set; }
    }
}
