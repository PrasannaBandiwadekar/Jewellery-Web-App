using Microsoft.AspNetCore.Mvc;
using NageshaJewellers.Models;
using System.Linq;
using X.PagedList;
using X.PagedList.Extensions;

namespace NageshaJewellers.Controllers
{
    public class GuestController : Controller
    {
        private readonly AppDbContext _context;

        public GuestController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("UserEmail") != null;
        }

        public IActionResult Home()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult Designs(int? page)
        {
            int pageSize = 8;

            int pageNumber = page ?? 1;

            var products = _context.Products
                .OrderByDescending(x => x.Id)
                .ToPagedList(pageNumber, pageSize);

            return View(products);
        }

        public IActionResult Search(string searchText)
        {
            var products = _context.Products
                .Where(p => p.Name.Contains(searchText))
                .ToList();

            return View("Designs", products);
        }

        public IActionResult AboutUs() => View();

        public IActionResult Services() => View();

        public IActionResult Contact() => View();

        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Category(string category)
        {
            var products = _context.Products
                .Where(p => p.Category == category)
                .ToList();

            return View("Designs", products);
        }

        public IActionResult AddToWishlist(int id)
        {
            var wishlist = new Wishlist
            {
                ProductId = id,
                UserEmail = HttpContext.Session.GetString("UserEmail")
            };

            _context.Wishlists.Add(wishlist);

            _context.SaveChanges();

            return RedirectToAction("Designs");
        }


        [HttpPost]
        public IActionResult Contact(Enquiry enquiry)
        {
            enquiry.CreatedAt = DateTime.Now;

            _context.Enquiries.Add(enquiry);

            _context.SaveChanges();

            ViewBag.Message = "Message Sent Successfully";

            return View();
        }


    }
}