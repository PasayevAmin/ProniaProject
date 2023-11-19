using FrontToBack.Models;

namespace FrontToBack.ViewModels
{
    public class ProductVM
    {

        public Product Products { get; set; }

        public List<Product> RelatedProducts { get; set; }
        public List<ProductImage> ProductImages { get; set; }

    }
}
