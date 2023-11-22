using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name must be entered")]
        [MaxLength(25,ErrorMessage = "Name cannot be longer than 25")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
