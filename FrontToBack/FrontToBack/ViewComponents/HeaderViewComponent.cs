using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.Security.Claims;

namespace FrontToBack.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public HeaderViewComponent(AppDbContext context, UserManager<AppUser> userManager,IHttpContextAccessor http)
        {
            _context = context;
            _userManager = userManager;
            _http = http;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                   .Include(p => p.BasketItems)
                   .ThenInclude(x => x.Product)
                   .ThenInclude(x => x.ProductImages.Where(p => p.IsPrimary == true))
                   .FirstOrDefaultAsync(x => x.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));



                foreach (var item in user.BasketItems)
                    {
                        items.Add(new BasketItemVM
                        {
                            Id = item.ProductId,
                            Count = item.Count,
                            Name = item.Product.Name,
                            Price = item.Product.Price,
                            SubTotal = item.Count * item.Product.Price,
                            Image = item.Product.ProductImages.FirstOrDefault()?.ImageURL

                        });
                    }

                
            }
            else
            {
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


