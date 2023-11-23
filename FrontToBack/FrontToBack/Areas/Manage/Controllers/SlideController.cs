using FrontToBack.Areas.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SlideController : Controller
    {

        private readonly AppDbContext _context;

        public SlideController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Slide> slides= await _context.Slides.ToListAsync();
            return View(slides);
        }
        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
          
            if (slide.Photo is null)
            {
                ModelState.AddModelError("Photo","Please Choose the Photo");
                return View();
            }
            if (!slide.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Invalid file type");
                return View();

            }
            if (slide.Photo.Length>4*1024*1024)
            {
                ModelState.AddModelError("Photo", "File size should not be larger than 4 mb");
                return View();
            }

            bool result = _context.Slides.Any(x => x.Title.Trim().ToLower() == slide.Title.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Title", "There is a Title with this name");
                return View();
            }
            FileStream fileStream = new FileStream(@"C:\Users\ASUS\OneDrive\İş masası\MVS\FrontToBack\FrontToBack\wwwroot\assets\images\slider\" + slide.Photo.FileName, FileMode.Create);
            await slide.Photo.CopyToAsync(fileStream);
            slide.Image = slide.Photo.FileName;

           await _context.Slides.AddAsync(slide);
           await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Slide existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);


            if (existed == null) return NotFound();
            return View(existed);

        }

    }
}
