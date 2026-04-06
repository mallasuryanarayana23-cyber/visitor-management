using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using VMS.DAL;
using VMS.Helpers;

namespace VMS.Controllers
{
    [AuthFilter(AllowedRoles = "GUARD")]
    public class GuardController : Controller
    {
        private VisitorDAL _visitorDal = new VisitorDAL();
        private UserDAL _userDal = new UserDAL();

        [HttpGet]
        public IActionResult Dashboard()
        {
            var stats = _visitorDal.GetDashboardCounts();
            return View(stats);
        }

        [HttpGet]
        public IActionResult TodayList()
        {
            // View should load DataTables hitting an API or populated directly
            return View();
        }

        [HttpGet]
        public IActionResult Search(string q)
        {
            ViewBag.Query = q;
            return View();
        }

        [HttpPost]
        public IActionResult CheckIn(int visitorId, int gateId)
        {
            try
            {
                int guardId = HttpContext.Session.GetInt32("UserID") ?? 0;
                _visitorDal.CheckInVisitor(visitorId, gateId, guardId);
                _userDal.LogAudit(guardId, "Check-In", "VMS_VISITORS", visitorId, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown", $"Gate check-in at gate {gateId}");
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                 return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CheckOut(int visitorId)
        {
             try
            {
                int guardId = HttpContext.Session.GetInt32("UserID") ?? 0;
                _visitorDal.CheckOutVisitor(visitorId);
                _userDal.LogAudit(guardId, "Check-Out", "VMS_VISITORS", visitorId, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown", $"Gate check-out");
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                 return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
