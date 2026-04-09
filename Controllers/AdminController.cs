using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
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
        public IActionResult Dashboard()
        {
            var stats = _visitorDal.GetDashboardCounts();
            return View(stats);
        }

        [HttpGet]
        public IActionResult Visitors()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Gallery()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Reports()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Masters()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AuditLog()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExportReport(DateTime start, DateTime end, int deptId, string format)
        {
            var data = _visitorDal.GetVisitorReport(start, end, deptId);

            if (format == "Excel")
                return ExportHelper.ExportToExcel(data);
            else if (format == "PDF")
                return ExportHelper.ExportToPDF(data);
                
            return Content("Invalid format");
        }

        [HttpGet]
        public IActionResult GetUsersList()
        {
            var dt = _userDal.GetUsers();
            var list = new System.Collections.Generic.List<object>();
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new
                {
                    UserId = row["USER_ID"],
                    Username = row["USERNAME"],
                    FullName = row["FULL_NAME"],
                    Role = row["ROLE"],
                    IsActive = row["IS_ACTIVE"]
                });
            }
            return Json(list);
        }

        [HttpPost]
        public IActionResult SaveUser(string username, string name, string role, string password, bool isEdit)
        {
            try
            {
                bool success;
                if (isEdit)
                {
                    success = _userDal.UpdateUser(username, name, role, 1);
                }
                else
                {
                    success = _userDal.CreateUser(username, password, name, role);
                }

                return Json(new { success = success, message = success ? "User saved successfully" : "Execution failed" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ToggleUserStatus(string username, bool active)
        {
            try
            {
                // We reuse UpdateUser with the status flag
                // We need to fetch the existing data first or just pass status
                // To keep it simple, we just update status
                using (var conn = DBHelper.GetConnection())
                {
                    string sql = "UPDATE VMS_USERS SET IS_ACTIVE = @p_val WHERE USERNAME = @p_username";
                    using (var cmd = new Npgsql.NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@p_val", active ? 1 : 0);
                        cmd.Parameters.AddWithValue("@p_username", username);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SetGuardPassword(string empNo, string password)
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
