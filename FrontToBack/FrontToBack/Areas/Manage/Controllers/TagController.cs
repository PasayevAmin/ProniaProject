using FrontToBack.Areas.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]

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
            DashboardVM dashboardVM = new DashboardVM
            {
                Tags = tags
            };
            return View(dashboardVM);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _context.Tags.Any(x => x.Name.Trim().ToLower() == tag.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }

            _context.Tags.Add(tag);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
