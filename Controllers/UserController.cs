using System;
using Microsoft.AspNetCore.Mvc;
using VMS.Models;
using VMS.DAL;
using VMS.Helpers;

namespace VMS.Controllers
{
    [AuthFilter(AllowedRoles = "USER")]
    public class UserController : Controller
    {
        private VisitorDAL _visitorDal = new VisitorDAL();
        private FileUploadDAL _fileUploadDal = new FileUploadDAL();
        private UserDAL _userDal = new UserDAL();

        [HttpGet]
        public IActionResult PreRegister()
        {
            // Usually we'd populate dropdowns for Dept and Host here via ViewBag
            return View(new VisitorModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PreRegister(VisitorModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Register Visitor in DB
                    int loggedInUser = (int)HttpContext.Session.GetString("UserID"];
                    model.RegisteredBy = loggedInUser;
                    
                    int newVisitorId;
                    string newToken;

                    _visitorDal.RegisterVisitor(model, out newVisitorId, out newToken);

                    // 2. Upload Photo
                    if (model.PhotoUpload != null && model.PhotoUpload.ContentLength > 0)
                    {
                        var photoData = FileUploadHelper.UploadFile(model.PhotoUpload, newVisitorId, "Photo");
                        _fileUploadDal.SaveFileUploadRecord(photoData);
                    }

                    // 3. Upload ID Proof
                    if (model.IDProofUpload != null && model.IDProofUpload.ContentLength > 0)
                    {
                        var docData = FileUploadHelper.UploadFile(model.IDProofUpload, newVisitorId, "IDProof");
                        _fileUploadDal.SaveFileUploadRecord(docData);
                    }

                    // 4. Audit Log
                    _userDal.LogAudit(loggedInUser, "Register Visitor", "VMS_VISITORS", newVisitorId, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown", $"Registered visitor token {newToken}");

                    TempData["SuccessMessage"] = $"Registration successful! Token: {newToken}";
                    return RedirectToAction("ViewPass", new { token = newToken });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult MyVisits()
        {
            // Will fetch from View based on current User
            // Here just rendering view for now
            return View();
        }

        [HttpGet]
        public IActionResult ViewPass(string token)
        {
             // Render the Pass view
            ViewBag.Token = token;
            return View();
        }
    }
}
