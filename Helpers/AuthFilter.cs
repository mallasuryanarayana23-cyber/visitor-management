using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace VMS.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthFilter : Attribute, IAuthorizationFilter
    {
        public string AllowedRoles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var role = context.HttpContext.Session.GetString("Role");
            if (role != null)
            {
                if (string.IsNullOrEmpty(AllowedRoles))
                {
                    return;
                }

                if (AllowedRoles.Contains(role))
                {
                    return;
                }
            }

            context.Result = new RedirectToActionResult("Unauthorized", "Account", null);
        }
    }
}
