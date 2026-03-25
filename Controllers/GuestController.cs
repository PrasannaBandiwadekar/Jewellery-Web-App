using Microsoft.AspNetCore.Mvc;

namespace NageshaJewellers.Controllers
{
    public class GuestController : Controller
    {
        public IActionResult Home()
        {
            var user = HttpContext.Session.GetString("UserEmail");

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult Designs()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

    }
}
