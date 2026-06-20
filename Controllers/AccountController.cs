using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NageshaJewellers.Models;
using NageshaJewellers.Services;
using NageshaJewellers.ViewModels;

namespace NageshaJewellers.Controllers
{
    public class AccountController : Controller
    {
        // DATABASE DB

        private readonly AppDbContext _context;

        // EMAIL SERVICE

        private readonly EmailService _emailService;

        // CONSTRUCTOR

        public AccountController(
            AppDbContext context,
            EmailService emailService)
        {
            _context = context;

            _emailService = emailService;
        }

        // =========================================================
        // REGISTER PAGE
        // =========================================================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // =========================================================
        // REGISTER POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // CHECK MODEL VALIDATION

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // CHECK EMAIL EXISTS

            bool emailExists =
                await _context.Users
                .AnyAsync(x => x.Email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    "",
                    "Email already exists"
                );

                return View(model);
            }

            // CREATE USER OBJECT

            var user = new User
            {
                Name = model.Name,

                Email = model.Email,

                Phone = model.Phone,

                // HASH PASSWORD

                PasswordHash =
                    BCrypt.Net.BCrypt
                    .HashPassword(model.Password),

                // DEFAULT ROLE

                Role = "Customer"
            };

            // SAVE USER

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            // =====================================================
            // SEND WELCOME EMAIL
            // =====================================================

            string subject =
                "Welcome to Nagesha Jewellers";

            string body = $@"

            <div style='margin:0;
                        padding:0;
                        background:#f4f4f4;
                        font-family:Arial,sans-serif;'>

                <table width='100%'
                       cellpadding='0'
                       cellspacing='0'>

                    <tr>
                        <td align='center'>

                            <table width='600'
                                   style='background:white;
                                          margin-top:30px;
                                          border-radius:12px;
                                          overflow:hidden;
                                          box-shadow:0 0 15px rgba(0,0,0,0.1);'>

                                <!-- HEADER -->

                                <tr>

                                    <td style='background:#d4af37;
                                               padding:30px;
                                               text-align:center;
                                               color:white;'>

                                        <h1 style='margin:0;
                                                   font-size:36px;'>

                                            Nagesha Jewellers

                                        </h1>

                                        <p style='margin-top:10px;
                                                  font-size:16px;'>

                                            Premium Gold • Silver • Diamond Collections

                                        </p>

                                    </td>

                                </tr>

                                <!-- BODY -->

                                <tr>

                                    <td style='padding:40px;'>

                                        <h2 style='color:#333;'>

                                            Hello {user.Name},

                                        </h2>

                                        <p style='font-size:16px;
                                                  color:#555;
                                                  line-height:1.8;'>

                                            Thank you for registering with
                                            <strong>Nagesha Jewellers</strong>.

                                        </p>

                                        <p style='font-size:16px;
                                                  color:#555;
                                                  line-height:1.8;'>

                                            We are delighted to welcome you
                                            into our jewellery family.

                                        </p>

                                        <p style='font-size:16px;
                                                  color:#555;
                                                  line-height:1.8;'>

                                            Explore exclusive collections of:

                                        </p>

                                        <ul style='color:#555;
                                                   line-height:2;'>

                                            <li>Gold Jewellery</li>

                                            <li>Silver Jewellery</li>

                                            <li>Diamond Collections</li>

                                            <li>Wedding Jewellery</li>

                                        </ul>

                                        <!-- BUTTON -->

                                        <div style='text-align:center;
                                                    margin-top:40px;'>

                                            <a href='https://localhost:7000/Guest/Designs'

                                               style='background:#d4af37;
                                                      color:white;
                                                      text-decoration:none;
                                                      padding:15px 35px;
                                                      border-radius:50px;
                                                      font-size:18px;
                                                      display:inline-block;'>

                                                Explore Collections

                                            </a>

                                        </div>

                                    </td>

                                </tr>

                                <!-- FOOTER -->

                                <tr>

                                    <td style='background:#222;
                                               color:#ccc;
                                               padding:30px;
                                               text-align:center;'>

                                        <h3 style='color:white;'>

                                            Nagesha Jewellers

                                        </h3>

                                        <p>

                                            Main Road, Shirol,
                                            Kolhapur, Maharashtra

                                        </p>

                                        <p>

                                            📞 +91 8421058477

                                        </p>

                                        <p>

                                            ✉️ bandiwadekarprasanna@gmail.com

                                        </p>

                                        <hr style='border-color:#444;'>

                                        <p style='font-size:13px;'>

                                            © 2026 Nagesha Jewellers.
                                            All Rights Reserved.

                                        </p>

                                    </td>

                                </tr>

                            </table>

                        </td>

                    </tr>

                </table>

            </div>

            ";

            try
            {
                _emailService.SendEmail(
                    user.Email,
                    subject,
                    body
                );
            }
            catch
            {
                // OPTIONAL:
                // avoid app crash if email fails
            }

            // SUCCESS MESSAGE

            TempData["Success"] =
                "Registration Successful";

            // REDIRECT

            return RedirectToAction(nameof(Login));
        }

        // =========================================================
        // LOGIN PAGE
        // =========================================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // =========================================================
        // LOGIN POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // MODEL VALIDATION

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // FIND USER

            var user =
                await _context.Users
                .FirstOrDefaultAsync(
                    x => x.Email == model.Email
                );

            // CHECK USER EXISTS

            if (user == null)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid Email"
                );

                return View(model);
            }

            // VERIFY PASSWORD

            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    model.Password,
                    user.PasswordHash
                );

            if (!isPasswordValid)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid Password"
                );

                return View(model);
            }

            // =====================================================
            // SESSION MANAGEMENT
            // =====================================================

            HttpContext.Session.SetString(
                "UserEmail",
                user.Email
            );

            HttpContext.Session.SetString(
                "UserName",
                user.Name
            );

            HttpContext.Session.SetString(
                "UserRole",
                user.Role
            );

            // SUCCESS MESSAGE

            TempData["Success"] =
                "Login Successful";

            // REDIRECT

            return RedirectToAction(
                "Home",
                "Guest"
            );
        }

        // =========================================================
        // LOGOUT
        // =========================================================

        public IActionResult Logout()
        {
            // CLEAR SESSION

            HttpContext.Session.Clear();

            TempData["Success"] =
                "Logged Out Successfully";

            return RedirectToAction(nameof(Login));
        }

        // =========================================================
        // ACCESS DENIED PAGE
        // =========================================================

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(
        ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email not found");
                return View(model);
            }

            // GENERATE OTP

            Random random = new Random();

            string otp = random.Next(100000, 999999).ToString();

            user.ResetOTP = otp;

            user.OTPExpiry = DateTime.Now.AddMinutes(10);

            await _context.SaveChangesAsync();

            // SEND EMAIL

            _emailService.SendEmail(
                user.Email,
                "Password Reset OTP",
                $"<h2>Your OTP is: {otp}</h2>"
            );

            TempData["Success"] =
                "OTP sent to your email";

            return RedirectToAction(
                "ResetPassword",
                new { email = user.Email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            return View(new ResetPasswordViewModel
            {
                Email = email
            });
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(
    ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            if (user.ResetOTP != model.OTP)
            {
                ModelState.AddModelError("", "Invalid OTP");
                return View(model);
            }

            if (user.OTPExpiry < DateTime.Now)
            {
                ModelState.AddModelError("", "OTP expired");
                return View(model);
            }

            // CHANGE PASSWORD

            user.PasswordHash =
                BCrypt.Net.BCrypt
                .HashPassword(model.NewPassword);

            user.ResetOTP = null;

            user.OTPExpiry = null;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Password changed successfully";

            return RedirectToAction("Login");
        }
    }
}