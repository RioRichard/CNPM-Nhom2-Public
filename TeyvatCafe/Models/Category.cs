using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeyvatCafe.Models
{
    public class Category
    {
        

        public int? IDCategory { get; set; }
        public string CategoryName { get; set; }
        public int? FatherCategory { get; set; }
        public bool? IsDelete { get; set; }
        [NotMapped]
        public List<Category> ChildCategory { get; set; }

       


    }

}
