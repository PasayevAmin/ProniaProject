using FrontToBack.Areas.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ColorController : Controller
    {

        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.Include(x => x.ProductColors).ToListAsync();
            DashboardVM dashboardVM = new DashboardVM
            {
                Colors = colors
            };
            return View(dashboardVM);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _context.Colors.Any(x => x.Name.Trim().ToLower() == color.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }

            _context.Colors.Add(color);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color == null) return NotFound();
            return View(color);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Color color)
        {

            if (!ModelState.IsValid) return View();
            Color existed = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);

            if (existed == null) return NotFound();

            bool result = await _context.Colors.AnyAsync(c => c.Name == existed.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This color is already available");
                return View();
            }

            existed.Name = color.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Deletes(int id)
        {
            if (id <= 0) return BadRequest();

            Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();

            _context.Colors.Remove(existed);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Color existed = await _context.Colors.Include(x => x.ProductColors).FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();
            return View(existed);

        }
    }
}
