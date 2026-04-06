using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Configuration;
using VMS.Models;
using System.Linq;

namespace VMS.Helpers
{
    public static class FileUploadHelper
    {
        public static FileUploadModel UploadFile(IFormFile file, int visitorId, string uploadType)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file selected for upload.");

            // Get configurations
            string uploadServerPath = System.Configuration.ConfigurationManager.AppSettings["UploadServerPath"];
            string uploadBaseUrl = System.Configuration.ConfigurationManager.AppSettings["UploadBaseUrl"];
            
            // Validate Max Size
            int maxSizeMB = uploadType == "Photo" ? 
                Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxPhotoSizeMB"]) : 
                Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxDocSizeMB"]);
                
            if (file.Length > (maxSizeMB * 1024 * 1024))
                throw new Exception($"{uploadType} must not exceed {maxSizeMB} MB.");

            // Validate Extensions
            string extension = Path.GetExtension(file.FileName).ToLower();
            string allowedExts = uploadType == "Photo" ? 
                System.Configuration.ConfigurationManager.AppSettings["AllowedImageExt"] : 
                System.Configuration.ConfigurationManager.AppSettings["AllowedDocExt"];

            if (!allowedExts.Split(',').Contains(extension))
                throw new Exception($"Invalid file extension for {uploadType}. Allowed: {allowedExts}");

            // Generate structural folder paths
            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MM");
            string relativeFolder = uploadType == "Photo" ? $@"Photos\{year}\{month}\" : $@"IDProofs\{year}\{month}\";
            
            // Generate names
            string originalName = Path.GetFileName(file.FileName);
            string newFileName = Guid.NewGuid().ToString() + extension;
            string targetDirectory = Path.Combine(uploadServerPath, relativeFolder);
            string targetFilePath = Path.Combine(targetDirectory, newFileName);
            
            string fileUrl = uploadBaseUrl.TrimEnd('/') + "/" + relativeFolder.Replace(@"\", "/") + newFileName;

            // Ensure destination folder exists (Needs permissions on UNC path)
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Save the physical file using System.IO to the UNC path
            using (var stream = new FileStream(targetFilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Return the model structure needed for DB
            return new FileUploadModel
            {
                VisitorID = visitorId,
                UploadType = uploadType,
                OriginalName = originalName,
                StoredName = newFileName,
                FilePath = targetFilePath,
                FileUrl = fileUrl,
                FileSizeBytes = file.Length,
                MimeType = file.ContentType,
                UploadedDate = DateTime.Now
            };
        }
    }
}
