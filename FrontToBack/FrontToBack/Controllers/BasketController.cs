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
                foreach (var item in cookies)
                {
                    Product product=await _context.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true)).FirstOrDefaultAsync(p=>p.Id==item.Id);
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
        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            List<BasketCookieItemVM> basket;
            if (Request.Cookies["Basket"] is null)
            {
                basket = new List<BasketCookieItemVM>();
                BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                {
                    Id = id,
                    Count = 1
                };
                basket.Add(basketCookieItemVM);

            }
            else
            {
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                BasketCookieItemVM existed= basket.FirstOrDefault(x => x.Id == id);
                if (existed == null)
                {
                    BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                    basket.Add(basketCookieItemVM);
                }
                else
                {
                    existed.Count++;
                }
                
            }
           
            string json=JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket",json);
            return RedirectToAction(nameof(Index),"Home");
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
            List<BasketCookieItemVM> basket;
            basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);
            if (existed != null)
            {
                basket.Remove(existed);

            }

            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Home");
        }
        public async Task<IActionResult> PlusBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            List<BasketCookieItemVM> basket;
           
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);
                if (existed != null)
                {
                    BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                basketCookieItemVM.Count++;

                    basket.Add(basketCookieItemVM);
                }
                else
                {
                    existed.Count++;
                }

            

            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return View(Response);
        }
        public async Task<IActionResult> MinusBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            List<BasketCookieItemVM> basket;
            
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);
                if (existed != null)
                {
                    BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        
                    };
                    if (basketCookieItemVM.Count==0)
                    {
                        basket.Remove(existed);
                    }
                    basketCookieItemVM.Count -= 1;
                    basket.Add(basketCookieItemVM);
                }
                else
                {
                    existed.Count++;
                }

            

            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return View(Response);
        }
    }
}
