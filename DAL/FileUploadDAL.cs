using System;
using System.Data;
using Microsoft.Data.Sqlite;
using VMS.Models;

namespace VMS.DAL
{
    public class FileUploadDAL
    {
        public void SaveFileUploadRecord(FileUploadModel model)
        {
            string query = @"
                INSERT INTO VMS_FILE_UPLOADS (
                    VISITOR_ID, UPLOAD_TYPE, ORIGINAL_NAME, STORED_NAME, FILE_PATH, FILE_URL, FILE_SIZE_BYTES, MIME_TYPE
                ) VALUES (
                    @p_VISITOR_ID, @p_UPLOAD_TYPE, @p_ORIGINAL_NAME, @p_STORED_NAME, @p_FILE_PATH, @p_FILE_URL, @p_FILE_SIZE_BYTES, @p_MIME_TYPE
                );";

            SqliteParameter[] parameters = {
                new SqliteParameter("@p_VISITOR_ID", model.VisitorID),
                new SqliteParameter("@p_UPLOAD_TYPE", model.UploadType),
                new SqliteParameter("@p_ORIGINAL_NAME", model.OriginalName),
                new SqliteParameter("@p_STORED_NAME", model.StoredName),
                new SqliteParameter("@p_FILE_PATH", model.FilePath),
                new SqliteParameter("@p_FILE_URL", model.FileUrl),
                new SqliteParameter("@p_FILE_SIZE_BYTES", model.FileSizeBytes),
                new SqliteParameter("@p_MIME_TYPE", model.MimeType)
            };

            DBHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
