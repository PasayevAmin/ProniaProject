using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FrontToBack.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                   .Include(p => p.BasketItems)
                   .ThenInclude(x=>x.Product)
                   .ThenInclude(x=>x.ProductImages.Where(p=>p.IsPrimary==true))
                   .FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                        else
                        {
                            cookies.Remove(cookies[i]);
                            string json = JsonConvert.SerializeObject(cookies);
                            Response.Cookies.Append("Basket", json);
                        }
                    }

                }
            }
           
            return View(items);
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();


            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(p => p.BasketItems)
                    .FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


                if (user == null) return NotFound();

               BasketItem item= user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id);
                if (item == null)
                {
                    item = new BasketItem
                    {
                        AppUserId = user.Id,
                        Count = 1,
                        ProductId = product.Id,
                        Price = product.Price,

                    };
                    user.BasketItems.Add(item);
                }
                else
                {
                    item.Count++;
                }

                await _context.SaveChangesAsync();


            }
            else
            {
                List<BasketCookieItemVM> basket;
                if (Request.Cookies["Basket"] is null)
                {

                    basket = new List<BasketCookieItemVM>();
                    BasketCookieItemVM itemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                    basket.Add(itemVM);
                }
                else
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed == null)
                    {
                        BasketCookieItemVM itemVM = new BasketCookieItemVM
                        {
                            Id = id,
                            Count = 1
                        };
                        basket.Add(itemVM);
                    }
                    else
                    {
                        existed.Count++;

                    }

                }


                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("Basket", json);

            }
            
            return RedirectToAction(nameof(Index), "Home");
        }
        public IActionResult GetBasket()
        {

            return Content(Request.Cookies["Basket"]);
        }
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(p => p.BasketItems)
                    .FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


                if (user == null) return NotFound();

                BasketItem item = user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id);
                if (item != null)
                {
                    
                    user.BasketItems.Remove(item);
                }
               

                await _context.SaveChangesAsync();

            }
            else
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                if (cookies.FirstOrDefault(x => x.Id == id) == null) return NotFound();



                cookies.Remove(cookies.FirstOrDefault(x => x.Id == id));
                string json = JsonConvert.SerializeObject(cookies);
                Response.Cookies.Append("Basket", json);
            }
           
            return RedirectToAction(nameof(Index), "Basket");
        }
        public async Task<IActionResult> Plus(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(p => p.BasketItems)
                    .FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


                if (user == null) return NotFound();

                BasketItem item = user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id);
                if (item != null)
                {

                    item.Count++;
                }


                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                if (cookies.FirstOrDefault(x => x.Id == id) == null) return NotFound();
                cookies.FirstOrDefault(x => x.Id == id).Count++;
                string json = JsonConvert.SerializeObject(cookies);
                Response.Cookies.Append("Basket", json);
            }
            
            return RedirectToAction(nameof(Index), "Basket");
        }
        public async Task<IActionResult> Minus(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(p => p.BasketItems)
                    .FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (user == null) return NotFound();

                BasketItem item = user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id);
                if (item != null)
                {
                    if (item.Count <= 1)
                    {
                        user.BasketItems.Remove(item);
                    }
                    else
                    {
                        item.Count--;
                    }
                }
                


                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                if (cookies.FirstOrDefault(x => x.Id == id) == null) return NotFound();
                if (cookies.FirstOrDefault(x => x.Id == id).Count <= 1)
                {
                    cookies.Remove(cookies.FirstOrDefault(x => x.Id == id));
                }
                else
                {
                    cookies.FirstOrDefault(x => x.Id == id).Count--;

                }
                string json = JsonConvert.SerializeObject(cookies);
                Response.Cookies.Append("Basket", json);
            }
           
            return RedirectToAction(nameof(Index), "Basket");
        }

    }
}
    

