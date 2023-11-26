using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]

    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            List<Product> products= await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories= await _context.Categories.ToListAsync();
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(CreateProductVM productVM)
        {


            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();

                return View(productVM);

            }

            bool result= await _context.Categories.AnyAsync(c=>c.Id==productVM.CategoryId);

            if (!result) 
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();

                ModelState.AddModelError("CategoryId", $"This category is not available");
                return View();
            }
            bool result1= await _context.Products.AnyAsync(c=>c.Price>=0);

            if (!result)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();

                ModelState.AddModelError("Price", $"The price cannot be negative");
                return View();
            }
            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                Description = productVM.Description,
                CategoryId=(int)productVM.CategoryId,


            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Product product=await _context.Products.Include(p=>p.Category).Include(p=>p.ProductImages).FirstOrDefaultAsync(c=>c.Id==id);


            if (product == null) return NotFound();
            return View(product);

        }
    }
}
