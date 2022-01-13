using System;
using System.Collections.Generic;

namespace TeyvatCafe.Models
{
    public class StaffModel
    {
        public string UserName { get; set; }
        public string Pass { get; set; }
        public string Email { get; set; }
        public List<Guid> RoleId { get; set; }
    }
}
