using System;

namespace TeyvatCafe.Models
{
    public class StaffRole
    {
        public Guid IdRole { get; set; }
        public string IDStaff { get; set; }
        public bool? IsDelete { get; set; }
    }
}
