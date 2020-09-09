using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomForRent.Helpers;
using RoomForRent.Infrastructure;
using RoomForRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace RoomForRent.Controllers
{
    public class AccountController : Controller
    {
        // bad idea but will do for now
        private static string Email { get; set; } = "admin@example.com";
        private static string Password { get; set; } = "Secret123$";

        public AccountController()
        {
        }

        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;

            if(!HttpContext.User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToLocal(returnUrl);
        }

        public async Task<IActionResult> LogOut(string returnUrl = "/")
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login",new { returnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel userModel,[FromQuery]string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View();

            // this is very bad code
            // but will do it for now anyway.
            if(userModel.Email == Email 
                && userModel.Password == Password )
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,userModel.Email),
                    new Claim(ClaimTypes.Role,"Admin"),
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = userModel.RememberMe,
                            ExpiresUtc = DateTime.UtcNow.AddHours(1)
                        }
                    );

                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
