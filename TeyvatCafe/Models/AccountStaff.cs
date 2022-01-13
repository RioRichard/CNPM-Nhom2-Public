using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeyvatCafe.Models
{
    public class AccountStaff
    {
        public string IDStaff { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpiredTokenTime { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? IsBanned { get; set; }
        public string FullName { get; set; }
        public bool? Gender { get; set; }

        [NotMapped]
        public List<Role> ListRole { get; set; }
    }
}
