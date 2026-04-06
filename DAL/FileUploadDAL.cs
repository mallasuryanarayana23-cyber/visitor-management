using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using VMS.Models;

namespace VMS.DAL
{
    public class FileUploadDAL
    {
        public void SaveFileUploadRecord(FileUploadModel model)
        {
            OracleParameter[] parameters = {
                new OracleParameter("p_VISITOR_ID", OracleDbType.Int32) { Value = model.VisitorID },
                new OracleParameter("p_UPLOAD_TYPE", OracleDbType.Varchar2) { Value = model.UploadType },
                new OracleParameter("p_ORIGINAL_NAME", OracleDbType.Varchar2) { Value = model.OriginalName },
                new OracleParameter("p_STORED_NAME", OracleDbType.Varchar2) { Value = model.StoredName },
                new OracleParameter("p_FILE_PATH", OracleDbType.Varchar2) { Value = model.FilePath },
                new OracleParameter("p_FILE_URL", OracleDbType.Varchar2) { Value = model.FileUrl },
                new OracleParameter("p_FILE_SIZE_BYTES", OracleDbType.Int64) { Value = model.FileSizeBytes },
                new OracleParameter("p_MIME_TYPE", OracleDbType.Varchar2) { Value = model.MimeType }
            };

            DBHelper.ExecuteNonQuery("SP_SAVE_FILE_UPLOAD", parameters);
        }
    }
}
