namespace FrontToBack.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<BasketItem> BasketItem { get; set; }

    }
}
