using Microsoft.AspNetCore.Mvc;
using NageshaJewellers.Models;
using System.Linq;

namespace NageshaJewellers.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // LOGIN GET
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN POST
        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (existingUser != null)
            {
                // ✅ STORE SESSION HERE
                HttpContext.Session.SetString("UserEmail", existingUser.Email);

                return RedirectToAction("Home", "Guest");
            }

            ViewBag.Message = "Invalid Email or Password";
            return View();
        }

        // REGISTER GET
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER POST
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                ViewBag.Message = "Registration Successful!";
                return RedirectToAction("Login");
            }

            return View();
        }


        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();   // remove session
            return RedirectToAction("Login");
        }
    }
}