using System;
using System.Web.Mvc;
using System.Web.Security;
using VMS.Models;
using VMS.DAL;

namespace VMS.Controllers
{
    public class AccountController : Controller
    {
        private UserDAL _userDal = new UserDAL();

        [HttpGet]
        public ActionResult Login()
        {
            // FIX: If AuthCookie exists but Session is missing, it causes an infinite redirect loop.
            if (User.Identity.IsAuthenticated)
            {
                if (Session["Role"] != null)
                {
                    return RedirectBasedOnRole();
                }
                else
                {
                    // Session was reset. Sign off the stale cookie.
                    FormsAuthentication.SignOut();
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    UserModel user = _userDal.AuthenticateUser(model.Username, model.Password);

                    if (user != null)
                    {
                        // Forms Auth
                        FormsAuthentication.SetAuthCookie(user.Username, false);

                        // Session
                        Session["UserID"] = user.UserID;
                        Session["UserName"] = user.FullName;
                        Session["Role"] = user.Role;

                        _userDal.LogAudit(user.UserID, "Login", "VMS_USERS", user.UserID, Request.UserHostAddress, "Successful login");

                        return RedirectBasedOnRole();
                    }
                    
                    ModelState.AddModelError("", "Invalid Username or Password. Please check your credentials.");
                }
                catch (Exception ex)
                {
                    // FIX: Catch Database disconnections instead of crashing the app
                    ModelState.AddModelError("", "Database Connection Error: Cannot verify credentials. Please ensure the Oracle DB is running and connected.");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    _userDal.LogAudit((int)Session["UserID"], "Logout", "VMS_USERS", (int)Session["UserID"], Request.UserHostAddress, "User logged out");
                }
            }
            catch { /* Ignore logging failures during logout */ }

            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Unauthorized()
        {
            return View();
        }

        private ActionResult RedirectBasedOnRole()
        {
            if (Session["Role"] == null) 
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }

            string role = Session["Role"].ToString();
            if (role == "ADMIN")
                return RedirectToAction("Dashboard", "Admin");
            else if (role == "GUARD")
                return RedirectToAction("Dashboard", "Guard");
            else
                return RedirectToAction("PreRegister", "User");
        }
    }
}
