using FrontToBack.Areas.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories= await _context.Categories.Include(x=>x.Products).ToListAsync();
            DashboardVM dashboardVM = new DashboardVM
            {
                Categories = categories
            };
            return View(dashboardVM);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _context.Categories.Any(x => x.Name.Trim().ToLower() == category.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }

            _context.Categories.Add(category);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);

            if (category == null) return NotFound();
            return View(category);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Category category)
        {

            if(!ModelState.IsValid) return View();
            Category existed = await _context.Categories.FirstOrDefaultAsync(x=>x.Id==id);

            if (existed == null) return NotFound();

            bool result = await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "This category is already available");
                return View();
            }

            existed.Name= category.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Deletes(int id)
        {
            if (id <= 0) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();

            _context.Categories.Remove(existed);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Category existed = await _context.Categories.Include(x => x.Products).ThenInclude(x => x.ProductImages).FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();
            return View(existed);

        }

    }
}
