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
    }
}
