namespace TeyvatCafe.Models
{
    public class ProductAttribute
    {
        public int IdAttribute { get; set; }
        public int IdProduct { get; set; }
        public string Value { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string AttributeName { get; set; }
    }
}
