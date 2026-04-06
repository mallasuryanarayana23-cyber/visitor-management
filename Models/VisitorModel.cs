using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VMS.Models
{
    public class VisitorModel
    {
        public int VisitorID { get; set; }
        public string VisitToken { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(150)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile must be exactly 10 digits")]
        public string Mobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Purpose is required")]
        public string Purpose { get; set; }

        [Required(ErrorMessage = "Host is required")]
        public int HostID { get; set; }
        public string HostName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public int DeptID { get; set; }
        public string DeptName { get; set; }

        [Required(ErrorMessage = "Expected Date & Time is required")]
        public DateTime ExpectedDateTime { get; set; }

        [Required(ErrorMessage = "ID Proof Type is required")]
        public int IDProofTypeID { get; set; }
        public string IDProofName { get; set; }

        [Required(ErrorMessage = "ID Proof Number is required")]
        public string IDProofNumber { get; set; }

        public string Status { get; set; }
        public int RegisteredBy { get; set; }
        public string RegisteredByName { get; set; }

        // For views
        public string PhotoUrl { get; set; }
        public string IDProofUrl { get; set; }

        // For uploads in Registration
        [Required(ErrorMessage = "Photo is required")]
        public IFormFile PhotoUpload { get; set; }

        [Required(ErrorMessage = "ID Proof document is required")]
        public IFormFile IDProofUpload { get; set; }
    }
}
