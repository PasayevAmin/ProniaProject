using Azure;
using FrontToBack.Areas.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.Utilities.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SlideController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
            if (slide.Photo.CheckFile("image/"))
            {
                ModelState.AddModelError("Photo", "Invalid file type");
                return View();

            }
            if (!slide.Photo.CheckSize(4 * 1024))
            {
                ModelState.AddModelError("Photo", "File size should not be larger than 4 mb");
                return View();
            }

            slide.Image = await slide.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();
            return View(existed);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Slide slide)
        {

            Slide existed=await _context.Slides.FirstOrDefaultAsync(x=>x.Id == id);
            if (existed == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(existed);
            }
            if (slide.Photo is not null)
            {
                if (slide.Photo.CheckFile("image/"))
                {
                    ModelState.AddModelError("Photo", "Invalid file type");
                    return View();

                }
                if (!slide.Photo.CheckSize(4 * 1024))
                {
                    ModelState.AddModelError("Photo", "File size should not be larger than 4 mb");
                    return View();
                }
                string filename = await slide.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
                existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
            }

            existed.Title = slide.Title;
            existed.Description = slide.Description;
            existed.SubTitle = slide.SubTitle;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();
            existed.Image.DeleteFile(_env.WebRootPath,"assets","images","slider");

            _context.Slides.Remove(existed);
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
