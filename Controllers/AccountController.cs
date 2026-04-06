using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using VMS.Models;
using VMS.DAL;

namespace VMS.Controllers
{
    public class AccountController : Controller
    {
        private UserDAL _userDal = new UserDAL();

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (HttpContext.Session.GetString("Role") != null)
                {
                    return RedirectBasedOnRole();
                }
                else
                {
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    UserModel user = _userDal.AuthenticateUser(model.Username, model.Password);

                    if (user != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, user.Role)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties { IsPersistent = false };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme, 
                            new ClaimsPrincipal(claimsIdentity), 
                            authProperties);

                        HttpContext.Session.SetInt32("UserID", user.UserID);
                        HttpContext.Session.SetString("UserName", user.FullName);
                        HttpContext.Session.SetString("Role", user.Role);

                        _userDal.LogAudit(user.UserID, "Login", "VMS_USERS", user.UserID, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown", "Successful login");

                        return RedirectBasedOnRole();
                    }
                    
                    ModelState.AddModelError("", "Invalid Username or Password. Please check your credentials.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database Connection Error: " + ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId.HasValue)
            {
                _userDal.LogAudit(userId.Value, "Logout", "VMS_USERS", userId.Value, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown", "User logged out");
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Unauthorized()
        {
            return View();
        }

        private IActionResult RedirectBasedOnRole()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role)) 
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            if (role == "ADMIN")
                return RedirectToAction("Dashboard", "Admin");
            else if (role == "GUARD")
                return RedirectToAction("Dashboard", "Guard");
            else
                return RedirectToAction("PreRegister", "User");
        }
    }
}
