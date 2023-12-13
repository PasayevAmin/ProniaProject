using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.Utilities.Exceptions;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Controllers
{
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) throw new WrongRequestExceptions("Wrong Search");
           

            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(x=>x.ProductColors).ThenInclude(x=>x.Color)
                .Include(x=>x.ProductTags).ThenInclude(t=>t.Tag)
                .Include(s=>s.ProductSizes).ThenInclude(s=>s.Size)
                .FirstOrDefaultAsync(x => x.Id == id);


            if (product == null) throw new NotFoundExceptions("No product found");
            
            List<Product> relatedproducts =await _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Where(p=>p.Id!=id)
                .Where(p => p.CategoryId == product.CategoryId)
                .Take(12)
                .ToListAsync();
           
                ProductVM productVM = new ProductVM
                {

                    Products = product,
                    RelatedProducts = relatedproducts,

                };
            

           

            return View(productVM);
        }
    }
}
