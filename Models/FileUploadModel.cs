using System;

namespace VMS.Models
{
    public class FileUploadModel
    {
        public int FileID { get; set; }
        public int VisitorID { get; set; }
        public string UploadType { get; set; } // Photo or IDProof
        public string OriginalName { get; set; }
        public string StoredName { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
        public long FileSizeBytes { get; set; }
        public string MimeType { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
