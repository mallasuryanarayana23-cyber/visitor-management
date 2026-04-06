using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VMS.Helpers
{
    public class AuthFilter : AuthorizeAttribute
    {
        public string AllowedRoles { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            if (httpContext.Session != null && httpContext.Session["Role"] != null)
            {
                string userRole = httpContext.Session["Role"].ToString();

                if (string.IsNullOrEmpty(AllowedRoles))
                {
                    return true;
                }

                // Check against comma-separated allowed roles
                return AllowedRoles.Contains(userRole);
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "Login" })
                );
            }
            else
            {
                // Authenticated, but no permission
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "Unauthorized" })
                );
            }
        }
    }
}
