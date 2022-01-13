using System;

namespace TeyvatCafe.Models
{
    public class Role
    {
        [System.ComponentModel.DataAnnotations.Key]
        public Guid IDRole { get; set; }
        public string RoleName { get; set; }
        public bool? IsDelete { get; set; }
    }
}
