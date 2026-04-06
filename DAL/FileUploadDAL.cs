using System;
using System.Data;
using Npgsql;
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

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@p_VISITOR_ID", model.VisitorID),
                new NpgsqlParameter("@p_UPLOAD_TYPE", model.UploadType),
                new NpgsqlParameter("@p_ORIGINAL_NAME", model.OriginalName),
                new NpgsqlParameter("@p_STORED_NAME", model.StoredName),
                new NpgsqlParameter("@p_FILE_PATH", model.FilePath),
                new NpgsqlParameter("@p_FILE_URL", model.FileUrl),
                new NpgsqlParameter("@p_FILE_SIZE_BYTES", model.FileSizeBytes),
                new NpgsqlParameter("@p_MIME_TYPE", model.MimeType)
            };

            DBHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
