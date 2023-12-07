
using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]

    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Include(x=>x.ProductTags).ToListAsync();
            
            return View(tags);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _context.Tags.Any(x => x.Name.Trim().ToLower() == tagVM.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }
            Tag tag=new Tag
            {
                Name = tagVM.Name
            };

            _context.Tags.AddAsync(tag);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

            if (tag == null) return NotFound();
            UpdateTagVM tagVM = new UpdateTagVM
            {
                Name=tag.Name,
            };   
            return View(tagVM);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
        {

            if (!ModelState.IsValid) return View();
            Tag existed = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (existed == null) return NotFound();

            bool result = await _context.Tags.AnyAsync(c => c.Name == tagVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This tag is already available");
                return View();
            }

            existed.Name = tagVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Deletes(int id)
        {
            if (id <= 0) return BadRequest();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();

            _context.Tags.Remove(existed);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Tag existed = await _context.Tags.Include(x => x.ProductTags).ThenInclude(x=>x.Product).ThenInclude(x=>x.ProductImages).FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();
            return View(existed);

        }

    }
}
