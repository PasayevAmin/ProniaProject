using FrontToBack.Areas.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using FrontToBack.Models;
using Size = FrontToBack.Models.Size;

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
    }
}
