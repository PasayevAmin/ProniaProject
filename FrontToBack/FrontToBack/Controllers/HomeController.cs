using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
                
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            List<Product> products = await _context.Products.Include(x=>x.ProductImages).ToListAsync();
            List<Product> lastproducts = await _context.Products.Include(x=>x.ProductImages).OrderByDescending(x=>x.Id).Take(8).ToListAsync();


            HomeVM home = new HomeVM
            {
                Products = products,
                Slides = slides,
                LastestProducts = lastproducts,
            };

            return View(home);
        }
        public async Task<IActionResult> About()
        {
            
            return View();
        }
        public async Task<IActionResult> ErrorPage(string error= "it stopped")
        {
            return View(model:error);
        }
    }
}
