using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Configuration;

namespace FrontToBack.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;


        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);


                for (int i = 0; i < cookies.Count; i++)
                {
                    Product product = await _context.Products
                   .Include(x => x.ProductImages.Where(pi => pi.IsPrimary == true))
                   .FirstOrDefaultAsync(p => p.Id == cookies[i].Id);
                    if (cookies[i].Count >= 1)
                    {
                        if (product != null)
                        {
                            BasketItemVM itemVM = new BasketItemVM
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Image = product.ProductImages.FirstOrDefault().ImageURL,
                                Price = product.Price,
                                Count = cookies[i].Count,
                                SubTotal = product.Price * cookies[i].Count
                            };
                            items.Add(itemVM);
                        }
                    }

                }
            }
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            BasketVM basketHomeVM = new BasketVM
            {
                Settings = settings,
                BasketItemVM = items,
            };
            return View(basketHomeVM);
        }


    }
}


