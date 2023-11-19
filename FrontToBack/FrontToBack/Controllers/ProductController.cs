﻿using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Controllers
{
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            Product product=_context.Products.Include(p=>p.Category).Include(p=>p.ProductImages).FirstOrDefault(x => x.Id == id);
            List<Product> relatedproducts = _context.Products.Include(x => x.Category).Include(x => x.ProductImages).Where(p=>p.Id!=id).Where(p => p.CategoryId == product.CategoryId).ToList();
            //for (int i = 0; i < relatedproducts.Count; i++)
            //{
            //    relatedproducts[i] = _context.Products.Include(x => x.Category).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == product.CategoryId);


            //}
                ProductVM productVM = new ProductVM
                {

                    Products = product,
                    RelatedProducts = relatedproducts,

                };
            

            
            if (product == null)
            {
                return NotFound();
            }

            return View(productVM);
        }
    }
}