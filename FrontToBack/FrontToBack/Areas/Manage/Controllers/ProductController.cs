using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.Utilities.Extension;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]

    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env= env;
        }

        public async Task<IActionResult> Index()
        {

            List<Product> products= await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
           Product product=await _context.Products
                .Include(x=>x.ProductTags)
                .Include(x=>x.ProductColors)
                .Include(x=>x.ProductSizes)
                .Include(x=>x.Category)
                .FirstOrDefaultAsync();
            if (product==null) return NotFound();
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),

            };
            return View(productVM);
        }
        [HttpPost]

        public async Task<IActionResult> Create(CreateProductVM productVM)
        {


            if (productVM.Price<=0)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("Price", $"The price cannot be negative");
                return View(productVM);
            }
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                return View(productVM);

            }

            bool result= await _context.Categories.AnyAsync(c=>c.Id==productVM.CategoryId);

            if (!result) 
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("CategoryId", $"This category is not available");
                return View(productVM);
            }

            if (productVM.CategoryId==0)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("CategoryId","Category must be Choosen");
                return View(productVM);  

            }
        
            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                Description = productVM.Description,
                CategoryId=(int)productVM.CategoryId,

            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Update(int id )
        {
            if (id<=0)
            {
                return BadRequest();
            }
            Product product = await _context.Products
               .Include(x => x.ProductTags)
               .Include(x => x.ProductColors)
               .Include(x => x.ProductSizes)
               .FirstOrDefaultAsync();
            if (product == null) return NotFound();
            UpdateProductVM createProductVM = new UpdateProductVM
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                TagIds = product.ProductTags.Select(x => x.TagId).ToList(),
                ColorIds = product.ProductColors.Select(x => x.ColorId).ToList(),
                SizeIds = product.ProductSizes.Select(x => x.SizeId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),

            };
            return View(createProductVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {



            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            Product existed = await _context.Products
               .Include(x => x.ProductTags)
               .Include(x => x.ProductColors)
               .Include(x => x.ProductSizes).FirstOrDefaultAsync(x => x.Id == id);
            if (existed == null) return NotFound();
            if (!await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("CategoryId", "There is no such category");
            }
           

            foreach (ProductTag item in existed.ProductTags)
            {
                if (!productVM.TagIds.Exists(x => x == item.TagId))
                {
                    _context.ProductTags.Remove(item);
                }
            }
            foreach (int item in productVM.TagIds)
            {
                if (!existed.ProductTags.Any(x=>x.TagId==item))
                {
                    bool result =await _context.ProductTags.AnyAsync(x => x.TagId==item);
                    if (result)
                    {
                        productVM.Categories = await _context.Categories.ToListAsync();
                        productVM.Tags = await _context.Tags.ToListAsync();
                        productVM.Sizes = await _context.Sizes.ToListAsync();
                        productVM.Colors = await _context.Colors.ToListAsync();
                        ModelState.AddModelError("TagIds", "There is no such Tag");
                    }
                    
                    existed.ProductTags.Add(new ProductTag
                    {
                        TagId= item,
                    });
                }

            }
            foreach (ProductColor item in existed.ProductColors)
            {
                if (!productVM.ColorIds.Exists(x => x == item.ColorId))
                {
                    _context.ProductColors.Remove(item);
                }
            }
            foreach (int item in productVM.ColorIds)
            {
                if (!existed.ProductColors.Any(x => x.ColorId == item))
                {
                    bool result = await _context.ProductColors.AnyAsync(x => x.ColorId == item);
                    if (result)
                    {
                        productVM.Categories = await _context.Categories.ToListAsync();
                        productVM.Tags = await _context.Tags.ToListAsync();
                        productVM.Sizes = await _context.Sizes.ToListAsync();
                        productVM.Colors = await _context.Colors.ToListAsync();
                        ModelState.AddModelError("TagIds", "There is no such Tag");
                    }

                    existed.ProductColors.Add(new ProductColor
                    {
                        ColorId = item,
                    });
                }

            }
            foreach (ProductSize item in existed.ProductSizes)
            {
                if (!productVM.SizeIds.Exists(x => x == item.SizeId))
                {
                    _context.ProductSizes.Remove(item);
                }
            }
            foreach (int item in productVM.SizeIds)
            {
                if (!existed.ProductSizes.Any(x => x.SizeId == item))
                {
                    bool result = await _context.ProductSizes.AnyAsync(x => x.SizeId == item);
                    if (result)
                    {
                        productVM.Categories = await _context.Categories.ToListAsync();
                        productVM.Tags = await _context.Tags.ToListAsync();
                        productVM.Sizes = await _context.Sizes.ToListAsync();
                        productVM.Colors = await _context.Colors.ToListAsync();
                        ModelState.AddModelError("TagIds", "There is no such Tag");
                    }

                    existed.ProductSizes.Add(new ProductSize
                    {
                        SizeId = item,
                    });
                }

            }

            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.Description = productVM.Description;
            existed.SKU = productVM.SKU;
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
            public async Task<IActionResult> Details(int id)
            {
                if (id <= 0) return BadRequest();

                Product product = await _context.Products.Include(p => p.Category).Include(p => p.ProductImages).FirstOrDefaultAsync(c => c.Id == id);
                if (product == null) return NotFound();


            
                return View(product);
            }

        }
    }

