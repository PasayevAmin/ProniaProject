
using FrontToBack.Areas.Manage.ViewModels;

using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]

    public class ColorController : Controller
    {

        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int count = await _context.Colors.CountAsync();

            List<Color> colors = await _context.Colors.Skip((page-1)*3).Take(3)
                .Include(x => x.ProductColors).ToListAsync();
            PaginationVM<Color> paginationVM = new PaginationVM<Color>
            {
                Items = colors,
                TotalPage = Math.Ceiling((double)count / 4),
                CurrentPage = page,

            };
            return View(paginationVM);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _context.Colors.Any(x => x.Name.Trim().ToLower() == colorVM.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }
            Color color = new Color
            {
                Name= colorVM.Name,
            };
            _context.Colors.Add(color);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color == null) return NotFound();
            UpdateColorVM colorVM=new UpdateColorVM 
            {
                Name = color.Name,
                
            };
            return View(colorVM);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {

            if (!ModelState.IsValid) return View();
            Color existed = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);

            if (existed == null) return NotFound();

            bool result = await _context.Colors.AnyAsync(c => c.Name == colorVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This color is already available");
                return View();
            }

            existed.Name = colorVM.Name;
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

            Color existed = await _context.Colors.Include(x => x.ProductColors).ThenInclude(x=>x.Product).ThenInclude(x=>x.ProductImages).FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();
            return View(existed);

        }
    }
}
