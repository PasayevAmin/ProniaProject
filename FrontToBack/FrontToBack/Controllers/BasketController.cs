using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrontToBack.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
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
                    else
                    {
                        cookies.Remove(cookies[i]);
                        string json = JsonConvert.SerializeObject(cookies);
                        Response.Cookies.Append("Basket", json);
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
            List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            if (cookies.FirstOrDefault(x => x.Id == id) == null) return NotFound();
            

           
            cookies.Remove(cookies.FirstOrDefault(x => x.Id == id));
            string json = JsonConvert.SerializeObject(cookies);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Basket");
        }
        public async Task<IActionResult> Plus(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            if (cookies.FirstOrDefault(x => x.Id == id) == null) return NotFound();
            cookies.FirstOrDefault(x => x.Id == id).Count++;
            string json = JsonConvert.SerializeObject(cookies);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Basket");
        }
        public async Task<IActionResult> Minus(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
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
            return RedirectToAction(nameof(Index), "Basket");
        }

    }
}
    

