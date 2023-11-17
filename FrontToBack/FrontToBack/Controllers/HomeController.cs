using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
                
        }
        public IActionResult Index()
        {
            List<Slide> slides = _context.Slides.ToList();
            List<Product> products = _context.Products.ToList();
            List<Product> lastproducts = _context.Products.OrderByDescending(x=>x.Id).Take(8).ToList();


            HomeVM home = new HomeVM
            {
                Products = products,
                Slides = slides,
                LastestProducts = lastproducts,
            };

            return View(home);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
