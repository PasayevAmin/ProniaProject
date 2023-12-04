using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        public readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
     
        
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Product> products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
            return View (products);
        }
    }
}

