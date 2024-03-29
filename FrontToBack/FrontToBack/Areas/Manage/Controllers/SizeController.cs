﻿
using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize]

    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page=1)
        {
            int count = await _context.Sizes.CountAsync();

            List<Size> sizes = await _context.Sizes.Skip((page-1)*3).Take(3)
                .Include(x=>x.ProductSizes)
                .ToListAsync();
            PaginationVM<Size> paginationVM = new PaginationVM<Size>
            {
                Items = sizes,
                TotalPage = Math.Ceiling((double)count / 3),
                CurrentPage = page,

            };
            return View(sizes);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _context.Sizes.Any(x => x.Name.Trim().ToLower() == sizeVM.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "There is a category with this name");
                return View();
            }
            Size size = new Size 
            {
                Name= sizeVM.Name,
            };

            _context.Sizes.Add(size);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Size size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (size == null) return NotFound();
            UpdateSizeVM updateSizeVM = new UpdateSizeVM
            {
                Name = size.Name,
            };
            return View(updateSizeVM);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSizeVM sizeVM)
        {

            if (!ModelState.IsValid) return View();
            Size existed = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);

            if (existed == null) return NotFound();

            bool result = await _context.Sizes.AnyAsync(c => c.Name == sizeVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This size is already available");
                return View();
            }

            existed.Name = sizeVM.Name;
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

            Size existed = await _context.Sizes.Include(x => x.ProductSizes).ThenInclude(x=>x.Product).ThenInclude(x=>x.ProductImages).FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();
            return View(existed);

        }
    }
}
