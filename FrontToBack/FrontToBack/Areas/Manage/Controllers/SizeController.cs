using FrontToBack.Areas.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes.Include(x=>x.ProductSizes).ToListAsync();
            DashboardVM dashboardVM = new DashboardVM
            {
                Sizes =    sizes
            };
            return View(dashboardVM);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _context.Sizes.Any(x => x.Name.Trim().ToLower() == size.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }

            _context.Sizes.Add(size);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Size size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (size == null) return NotFound();
            return View(size);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Tag tag)
        {

            if (!ModelState.IsValid) return View();
            Size existed = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);

            if (existed == null) return NotFound();

            bool result = await _context.Sizes.AnyAsync(c => c.Name == tag.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This size is already available");
                return View();
            }

            existed.Name = tag.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Deletes(int id)
        {
            if (id <= 0) return BadRequest();

            Size existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();

            _context.Sizes.Remove(existed);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Size existed = await _context.Sizes.Include(x => x.ProductSizes).FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();
            return View(existed);

        }
    }
}
