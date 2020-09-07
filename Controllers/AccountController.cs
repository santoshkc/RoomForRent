using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomForRent.Helpers;
using RoomForRent.Infrastructure;
using RoomForRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            var loggedUserInfo = HttpContext.Session.GetLoggedUserInfo();
            if(loggedUserInfo.IsLoggedIn)
            {
                return Redirect(returnUrl);
            }
            else 
                return View();
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult LogOut(string returnUrl = "/")
        {
            // Authorized user should only be able to logout
            // for now will use hack
            this.HttpContext.Session.SetLoggedUserInfo(new LoggedUserInfo
            {

            });

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
                this.HttpContext.Session.SetLoggedUserInfo(new LoggedUserInfo
                {
                    Email = userModel.Email,
                    IsLoggedIn = true
                });

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
