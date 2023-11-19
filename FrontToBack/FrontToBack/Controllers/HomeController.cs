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
        public IActionResult Index()
        {
            List<Slide> slides = _context.Slides.ToList();
            List<Product> products = _context.Products.Include(x=>x.ProductImages).ToList();
            List<Product> lastproducts = _context.Products.Include(x=>x.ProductImages).OrderByDescending(x=>x.Id).Take(8).ToList();


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
