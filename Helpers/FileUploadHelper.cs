using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VMS.Models;

namespace VMS.Helpers
{
    public static class FileUploadHelper
    {
        // Single shared HttpClient (best practice)
        private static readonly HttpClient _httpClient = new HttpClient();

        public static FileUploadModel UploadFile(IFormFile file, int visitorId, string uploadType)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file selected for upload.");

            // ── Read config ──────────────────────────────────────────────────
            string supabaseUrl    = AppConfig.Configuration["AppSettings:SupabaseUrl"];
            string supabaseKey    = AppConfig.Configuration["AppSettings:SupabaseAnonKey"];
            string bucket         = AppConfig.Configuration["AppSettings:SupabaseBucket"];
            int maxSizeMB         = uploadType == "Photo"
                ? Convert.ToInt32(AppConfig.Configuration["AppSettings:MaxPhotoSizeMB"])
                : Convert.ToInt32(AppConfig.Configuration["AppSettings:MaxDocSizeMB"]);
            string allowedExts    = uploadType == "Photo"
                ? AppConfig.Configuration["AppSettings:AllowedImageExt"]
                : AppConfig.Configuration["AppSettings:AllowedDocExt"];

            // ── Validate size ─────────────────────────────────────────────────
            if (file.Length > (maxSizeMB * 1024 * 1024))
                throw new Exception($"{uploadType} must not exceed {maxSizeMB} MB.");

            // ── Validate extension ─────────────────────────────────────────────
            string extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExts.Split(',').Contains(extension))
                throw new Exception($"Invalid file extension for {uploadType}. Allowed: {allowedExts}");

            // ── Build storage path ─────────────────────────────────────────────
            string year        = DateTime.Now.ToString("yyyy");
            string month       = DateTime.Now.ToString("MM");
            string folder      = uploadType == "Photo" ? "Photos" : "IDProofs";
            string newFileName = Guid.NewGuid().ToString() + extension;
            string storagePath = $"{folder}/{year}/{month}/{newFileName}";   // path inside bucket

            // ── Upload to Supabase Storage via REST API ───────────────────────
            string uploadUrl = $"{supabaseUrl}/storage/v1/object/{bucket}/{storagePath}";

            using var fileStream   = file.OpenReadStream();
            using var fileContent  = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

            using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
            request.Headers.Add("Authorization", $"Bearer {supabaseKey}");
            request.Headers.Add("x-upsert", "true");   // overwrite if same name
            request.Content = fileContent;

            var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new Exception($"Supabase upload failed ({(int)response.StatusCode}): {body}");
            }

            // ── Build permanent public URL ─────────────────────────────────────
            string publicUrl = $"{supabaseUrl}/storage/v1/object/public/{bucket}/{storagePath}";

            return new FileUploadModel
            {
                VisitorID      = visitorId,
                UploadType     = uploadType,
                OriginalName   = Path.GetFileName(file.FileName),
                StoredName     = newFileName,
                FilePath       = storagePath,
                FileUrl        = publicUrl,
                FileSizeBytes  = file.Length,
                MimeType       = file.ContentType,
                UploadedDate   = DateTime.Now
            };
        }
    }
}
