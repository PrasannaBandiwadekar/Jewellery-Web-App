using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NageshaJewellers.Models;

namespace NageshaJewellers.Controllers
{
    public class AdminController : Controller
    {
        // =====================================================
        // DATABASE CONTEXT
        // =====================================================

        private readonly AppDbContext _context;

        // =====================================================
        // WEB HOST ENVIRONMENT
        // USED FOR IMAGE UPLOAD
        // =====================================================

        private readonly IWebHostEnvironment _env;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public AdminController(
            AppDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;

            _env = env;
        }

        // =====================================================
        // CHECK ADMIN ACCESS
        // =====================================================

        private bool IsAdmin()
        {
            return HttpContext.Session
                .GetString("UserRole") == "Admin";
        }

        // =====================================================
        // ADMIN DASHBOARD
        // =====================================================

        public IActionResult Dashboard()
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            ViewBag.TotalProducts =
                _context.Products.Count();

            ViewBag.TotalUsers =
                _context.Users.Count();

            ViewBag.GoldProducts =
                _context.Products
                .Count(x => x.Category == "Gold");

            ViewBag.SilverProducts =
                _context.Products
                .Count(x => x.Category == "Silver");

            ViewBag.DiamondProducts =
                _context.Products
                .Count(x => x.Category == "Diamond");

            return View();
        }

        // =====================================================
        // LOAD ADD PRODUCT PAGE
        // =====================================================

        [HttpGet]
        public IActionResult AddProduct()
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            return View();
        }

        // =====================================================
        // SAVE PRODUCT
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(
            Product product,
            IFormFile ImageFile)
        {
            // CHECK ADMIN

            if (!IsAdmin())
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            try
            {
                // MODEL VALIDATION

                if (!ModelState.IsValid)
                {
                    return View(product);
                }

                // =================================================
                // IMAGE UPLOAD
                // =================================================

                if (ImageFile != null &&
                    ImageFile.Length > 0)
                {
                    // IMAGE FOLDER

                    string uploadsFolder =
                        Path.Combine(
                            _env.WebRootPath,
                            "images");

                    // CREATE FOLDER IF NOT EXISTS

                    if (!Directory.Exists(
                        uploadsFolder))
                    {
                        Directory.CreateDirectory(
                            uploadsFolder);
                    }

                    // UNIQUE FILE NAME

                    string uniqueFileName =
                        Guid.NewGuid().ToString() +
                        Path.GetExtension(
                            ImageFile.FileName);

                    // FINAL FILE PATH

                    string filePath =
                        Path.Combine(
                            uploadsFolder,
                            uniqueFileName);

                    // SAVE IMAGE TO FOLDER

                    using (var stream =
                        new FileStream(
                            filePath,
                            FileMode.Create))
                    {
                        await ImageFile
                            .CopyToAsync(stream);
                    }

                    // SAVE IMAGE PATH IN DATABASE

                    product.ImagePath =
                        "/images/" + uniqueFileName;
                }

                // =================================================
                // SAVE PRODUCT
                // =================================================

                _context.Products.Add(product);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Product Added Successfully";

                return RedirectToAction(
                    "Designs",
                    "Guest");
            }
            catch (Exception ex)
            {
                return Content(
                    "Error : " + ex.Message);
            }
        }

        // =====================================================
        // LOAD EDIT PRODUCT PAGE
        // =====================================================

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            var product =
                _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // =====================================================
        // UPDATE PRODUCT
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(
            Product product,
            IFormFile? ImageFile)
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            try
            {
                var existingProduct =
                    await _context.Products
                    .FindAsync(product.Id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                // =============================================
                // UPDATE BASIC DETAILS
                // =============================================

                existingProduct.Name =
                    product.Name;

                existingProduct.Category =
                    product.Category;

                existingProduct.Price =
                    product.Price;

                existingProduct.Description =
                    product.Description;

                // =============================================
                // UPDATE IMAGE IF NEW IMAGE SELECTED
                // =============================================

                if (ImageFile != null &&
                    ImageFile.Length > 0)
                {
                    string uploadsFolder =
                        Path.Combine(
                            _env.WebRootPath,
                            "images");

                    string uniqueFileName =
                        Guid.NewGuid().ToString() +
                        Path.GetExtension(
                            ImageFile.FileName);

                    string filePath =
                        Path.Combine(
                            uploadsFolder,
                            uniqueFileName);

                    using (var stream =
                        new FileStream(
                            filePath,
                            FileMode.Create))
                    {
                        await ImageFile
                            .CopyToAsync(stream);
                    }

                    existingProduct.ImagePath =
                        "/images/" + uniqueFileName;
                }

                // =============================================
                // SAVE CHANGES
                // =============================================

                _context.Products
                    .Update(existingProduct);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Product Updated Successfully";

                return RedirectToAction(
                    "Designs",
                    "Guest");
            }
            catch (Exception ex)
            {
                return Content(
                    "Error : " + ex.Message);
            }
        }

        // =====================================================
        // DELETE PRODUCT
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(
            int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            var product =
                await _context.Products
                .FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // =============================================
            // DELETE IMAGE FROM FOLDER
            // =============================================

            if (!string.IsNullOrEmpty(
                product.ImagePath))
            {
                string imagePath =
                    Path.Combine(
                        _env.WebRootPath,
                        product.ImagePath
                        .TrimStart('/'));

                if (System.IO.File.Exists(
                    imagePath))
                {
                    System.IO.File.Delete(
                        imagePath);
                }
            }

            // =============================================
            // DELETE PRODUCT
            // =============================================

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Product Deleted Successfully";

            return RedirectToAction(
                "Designs",
                "Guest");
        }

        // =====================================================
        // VIEW ALL PRODUCTS
        // =====================================================

        public IActionResult ProductList()
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            var products =
                _context.Products
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(products);
        }
    }
}