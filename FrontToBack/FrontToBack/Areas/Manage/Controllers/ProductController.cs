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
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {

            List<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {

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



            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                return View(productVM);

            }
            if (productVM.Price <= 0)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("Price", $"The price cannot be negative");
                return View(productVM);
            }

            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);

            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("CategoryId", $"This category is not available");
                return View(productVM);
            }

            if (productVM.CategoryId == 0)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("CategoryId", "Category must be Choosen");
                return View(productVM);

            }


            if (!productVM.MainPhoto.CheckFile("image/"))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("MainPhoto", "Image Type is Wrong");
                return View(productVM);
            }
            if (!productVM.MainPhoto.CheckSize(5 * 1024))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("MainPhoto", "Image Size is Wrong");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.CheckFile("image/"))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("HoverPhoto", "Image Type is Wrong");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.CheckSize(5 * 1024))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("HoverPhoto", "Image Size is Wrong");
                return View(productVM);
            }
            ProductImage main = new ProductImage
            {
                IsPrimary = true,
                ImageURL = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                Alternative = productVM.Name
            };
            ProductImage hover = new ProductImage
            {
                IsPrimary = false,
                ImageURL = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                Alternative = productVM.Name
            };


            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                Description = productVM.Description,
                CategoryId = (int)productVM.CategoryId,
                ProductTags = new List<ProductTag>(),
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>(),
                ProductImages = new List<ProductImage> { main, hover },

            };
            TempData["Message"] = "";
            foreach (var photo in productVM.Photos ?? new List<IFormFile>())
            {
                if (!photo.CheckFile("image/"))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type is invalid </p>";
                    continue;
                }
                if (!photo.CheckSize(2 * 1024))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size is invalid</p>";

                    continue;
                }
                product.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Alternative = productVM.Name,
                    ImageURL = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),

                });
            }



            foreach (var item in productVM.TagIds)
            {
                bool resulttag = await _context.Tags.AnyAsync(x => x.Id == item);
                if (!resulttag)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();


                    ModelState.AddModelError("TagIds", "There is no such tag");
                    return View(productVM);
                }
                ProductTag productTag = new ProductTag
                {
                    TagId = item
                };
                product.ProductTags.Add(productTag);
            }

            foreach (var item in productVM.SizeIds)
            {
                bool resultsize = await _context.Sizes.AnyAsync(x => x.Id == item);
                if (!resultsize)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();


                    ModelState.AddModelError("SizeIds", "There is no such size");
                    return View(productVM);
                }
                ProductSize productSize = new ProductSize
                {
                    SizeId = item
                };
                product.ProductSizes.Add(productSize);
            }
            foreach (var item in productVM.ColorIds)
            {
                bool resultcolor = await _context.Colors.AnyAsync(x => x.Id == item);
                if (!resultcolor)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();


                    ModelState.AddModelError("ColorIds", "There is no such color");
                    return View(productVM);
                }
                ProductColor productColor = new ProductColor
                {
                    ColorId = item
                };
                product.ProductColors.Add(productColor);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Product product = await _context.Products
                .Include(x => x.ProductImages)
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
                ProductImages = product.ProductImages,
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


            Product existed = await _context.Products
               .Include(x => x.ProductTags)
               .Include(x => x.ProductColors)
               .Include(x => x.ProductSizes)
               .Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.ProductImages = existed.ProductImages;
                return View(productVM);
            }


            if (existed == null) return NotFound();

            if (productVM.MainPhoto is not null)
            {

                if (!productVM.MainPhoto.CheckFile("image/"))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    ModelState.AddModelError("MainPhoto", "Image Type is Wrong");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.CheckSize(5 * 1024))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;


                    ModelState.AddModelError("MainPhoto", "Image Size is Wrong");
                    return View(productVM);
                }
            }
            if (productVM.HoverPhoto is not null)
            {

                if (!productVM.HoverPhoto.CheckFile("image/"))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    ModelState.AddModelError("HoverPhoto", "Image Type is Wrong");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.CheckSize(5 * 1024))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;


                    ModelState.AddModelError("HoverPhoto", "Image Size is Wrong");
                    return View(productVM);
                }
            }
            if (!await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.ProductImages = existed.ProductImages;

                ModelState.AddModelError("CategoryId", "There is no such category");
            }
            existed.ProductTags.RemoveAll(x => !productVM.TagIds.Exists(y => y == x.TagId));

            List<int> creatable = productVM.TagIds.Where(tg => !existed.ProductTags.Exists(x => x.TagId == tg)).ToList();


            foreach (var item in creatable)
            {
                bool result = await _context.Tags.AnyAsync(x => x.Id == item);
                if (!result)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    ModelState.AddModelError("TagIds", "There is no such tag");
                    return View(productVM);
                }
                existed.ProductTags.Add(new ProductTag
                {
                    TagId = item,
                });
            }




            existed.ProductColors.RemoveAll(x => !productVM.ColorIds.Exists(y => y == x.ColorId));

            List<int> creatable2 = productVM.ColorIds.Where(tg => !existed.ProductColors.Exists(x => x.ColorId == tg)).ToList();


            foreach (var item in creatable)
            {
                bool result = await _context.Colors.AnyAsync(x => x.Id == item);
                if (!result)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    ModelState.AddModelError("ColorIds", "There is no such tag");
                    return View(productVM);
                }
                existed.ProductColors.Add(new ProductColor
                {
                    ColorId = item,
                });
            }

            existed.ProductSizes.RemoveAll(x => !productVM.SizeIds.Exists(y => y == x.SizeId));

            List<int> creatable3 = productVM.SizeIds.Where(tg => !existed.ProductSizes.Exists(x => x.SizeId == tg)).ToList();


            foreach (var item in creatable)
            {
                bool result = await _context.Sizes.AnyAsync(x => x.Id == item);
                if (!result)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    ModelState.AddModelError("SizeIds", "There is no such tag");
                    return View(productVM);
                }
                existed.ProductSizes.Add(new ProductSize
                {
                    SizeId = item,
                });
            }

            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existImg = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                existImg.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(existImg);
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Alternative = productVM.Name,
                    ImageURL = fileName,
                });
            }
            if (productVM.HoverPhoto is not null)
            {
                string fileName = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existImg = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
                existImg.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(existImg);
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Alternative = productVM.Name,
                    ImageURL = fileName,
                });
            }

            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }
            List<ProductImage> removeable = existed.ProductImages.Where(x => !productVM.ImageIds.Exists(imgid => imgid == x.Id) && x.IsPrimary == null).ToList();
            foreach (ProductImage image in removeable)
            {
                image.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(image);
            }
            TempData["Message"] = "";
            foreach (var photo in productVM.Photos ?? new List<IFormFile>())
            {
                if (!photo.CheckFile("image/"))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type is invalid </p>";
                    continue;
                }
                if (!photo.CheckSize(2 * 1024))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size is invalid</p>";

                    continue;
                }
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Alternative = productVM.Name,
                    ImageURL = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),

                });
            }

            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.Description = productVM.Description;
            existed.SKU = productVM.SKU;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Product product = await _context.Products
                .Include(x => x.ProductImages)
               .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            foreach (var item in product.ProductImages ?? new List<ProductImage>())
            {
                item.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

            }
            _context.Products.Remove(product);
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

