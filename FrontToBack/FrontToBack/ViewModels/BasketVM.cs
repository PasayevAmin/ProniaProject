using FrontToBack.Models;

namespace FrontToBack.ViewModels
{
    public class BasketVM
    {
        public Dictionary<string,string> Settings { get; set; }
        public List<BasketItemVM> BasketItemVM { get; set; }
    }
}
