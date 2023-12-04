using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrontToBack.ViewComponents
{
    public class BasketViewComponent:ViewComponent
    {
        public readonly AppDbContext _context;

        public BasketViewComponent(AppDbContext context)
        {
            _context = context;


        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                foreach (var item in cookies)
                {
                    Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (product == null)
                    {
                        BasketItemVM itemVM = new BasketItemVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Image = product.ProductImages.FirstOrDefault().ImageURL,
                            Price = product.Price,
                            Count = item.Count,
                            SubTotal = product.Price * item.Count
                        };
                        items.Add(itemVM);
                    }
                }
            }
            return View(items);
        }
    }
}
