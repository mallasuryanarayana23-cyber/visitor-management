using System;
using System.Web.Mvc;
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
        public ActionResult Dashboard()
        {
            var stats = _visitorDal.GetDashboardCounts();
            return View(stats);
        }

        [HttpGet]
        public ActionResult TodayList()
        {
            // View should load DataTables hitting an API or populated directly
            return View();
        }

        [HttpGet]
        public ActionResult Search(string q)
        {
            ViewBag.Query = q;
            return View();
        }

        [HttpPost]
        public JsonResult CheckIn(int visitorId, int gateId)
        {
            try
            {
                int guardId = (int)Session["UserID"];
                _visitorDal.CheckInVisitor(visitorId, gateId, guardId);
                _userDal.LogAudit(guardId, "Check-In", "VMS_VISITORS", visitorId, Request.UserHostAddress, $"Gate check-in at gate {gateId}");
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                 return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CheckOut(int visitorId)
        {
             try
            {
                int guardId = (int)Session["UserID"];
                _visitorDal.CheckOutVisitor(visitorId);
                _userDal.LogAudit(guardId, "Check-Out", "VMS_VISITORS", visitorId, Request.UserHostAddress, $"Gate check-out");
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                 return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
