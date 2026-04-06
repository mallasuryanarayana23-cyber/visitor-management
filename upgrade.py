import os
import re

directories = ["Controllers", "DAL", "Models", "Helpers"]

replacements = {
    "using System.Web.Mvc;": "using Microsoft.AspNetCore.Mvc;",
    "using System.Web.Security;": "using Microsoft.AspNetCore.Authentication.Cookies;\nusing Microsoft.AspNetCore.Authentication;\nusing System.Security.Claims;",
    "ActionResult": "IActionResult",
    "JsonResult": "IActionResult",
    "using System.Web;": "using Microsoft.AspNetCore.Http;",
}

for root in directories:
    if not os.path.exists(root):
        continue
    for filename in os.listdir(root):
        if filename.endswith(".cs"):
            filepath = os.path.join(root, filename)
            with open(filepath, 'r', encoding='utf-8') as f:
                content = f.read()
            
            for old, new in replacements.items():
                content = content.replace(old, new)
                
            # specific auth filter fix
            content = content.replace("filterContext.HttpContext.Session[\"Role\"]", "filterContext.HttpContext.Session.GetString(\"Role\")")
            content = content.replace("filterContext.Result = new RedirectResult(\"~/Account/Login\")", "filterContext.Result = new RedirectToActionResult(\"Login\", \"Account\", null)")
            content = content.replace("using Microsoft.AspNetCore.Mvc;\nusing VMS.DAL;", "using Microsoft.AspNetCore.Mvc;\nusing Microsoft.AspNetCore.Mvc.Filters;\nusing Microsoft.AspNetCore.Http;\nusing VMS.DAL;")
            
            # Request.UserHostAddress to HttpContext.Connection.RemoteIpAddress
            content = content.replace("Request.UserHostAddress", "HttpContext.Connection.RemoteIpAddress?.ToString() ?? \"Unknown\"")
            
            # Session to HttpContext.Session
            content = content.replace("Session[\"", "HttpContext.Session.GetString(\"")
                
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(content)
                
print("All files upgraded to ASP.NET Core 8 syntax.")
