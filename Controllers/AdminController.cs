using System;
using System.Web.Mvc;
using VMS.DAL;
using VMS.Helpers;

namespace VMS.Controllers
{
    [AuthFilter(AllowedRoles = "ADMIN")]
    public class AdminController : Controller
    {
        private VisitorDAL _visitorDal = new VisitorDAL();
        private UserDAL _userDal = new UserDAL();

        [HttpGet]
        public ActionResult Dashboard()
        {
            var stats = _visitorDal.GetDashboardCounts();
            return View(stats);
        }

        [HttpGet]
        public ActionResult Visitors()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Users()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Gallery()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Reports()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Masters()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AuditLog()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExportReport(DateTime start, DateTime end, int deptId, string format)
        {
            var data = _visitorDal.GetVisitorReport(start, end, deptId);

            if (format == "Excel")
                return ExportHelper.ExportToExcel(data);
            else if (format == "PDF")
                return ExportHelper.ExportToPDF(data);
                
            return Content("Invalid format");
        }

        [HttpPost]
        public JsonResult SetGuardPassword(string empNo, string password)
        {
            try
            {
                bool isUpdated = _userDal.UpdatePasswordForGuard(empNo, password);
                
                if (isUpdated)
                {
                    return Json(new { success = true, message = "Password updated successfully for Guard: " + empNo });
                }
                else
                {
                    return Json(new { success = false, message = "Could not find a GUARD account with Employee No: " + empNo });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update password: " + ex.Message });
            }
        }
    }
}
