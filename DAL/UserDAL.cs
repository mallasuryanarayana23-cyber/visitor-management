using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using VMS.Models;

namespace VMS.DAL
{
    public class UserDAL
    {
        public UserModel AuthenticateUser(string username, string password)
        {
            string passwordHash = ComputeSHA256Hash(password);
            
            using (OracleConnection conn = DBHelper.GetConnection())
            {
                using (OracleCommand cmd = new OracleCommand("SP_AUTHENTICATE_USER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_USERNAME", OracleDbType.Varchar2).Value = username;
                    cmd.Parameters.Add("p_PASSWORD_HASH", OracleDbType.Varchar2).Value = passwordHash;
                    
                    OracleParameter cursorParam = new OracleParameter("p_RECORDSET", OracleDbType.RefCursor);
                    cursorParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(cursorParam);

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            if (Convert.ToInt32(row["IS_ACTIVE"]) == 0)
                                return null;

                            return new UserModel
                            {
                                UserID = Convert.ToInt32(row["USER_ID"]),
                                Username = row["USERNAME"].ToString(),
                                FullName = row["FULL_NAME"].ToString(),
                                Role = row["ROLE"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdatePasswordForGuard(string empNo, string password)
        {
            string passwordHash = ComputeSHA256Hash(password);

            using (OracleConnection conn = DBHelper.GetConnection())
            {
                using (OracleCommand cmd = new OracleCommand("SP_SET_GUARD_PASSWORD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_EMP_NO", OracleDbType.Varchar2).Value = empNo;
                    cmd.Parameters.Add("p_PASSWORD_HASH", OracleDbType.Varchar2).Value = passwordHash;

                    OracleParameter outStatus = new OracleParameter("p_OUT_STATUS", OracleDbType.Int32);
                    outStatus.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outStatus);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    int status = Convert.ToInt32(outStatus.Value.ToString() == "null" ? "0" : outStatus.Value.ToString());
                    return status == 1; // 1 = success, 0 = guard not found
                }
            }
        }

        private string ComputeSHA256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        
        public void LogAudit(int? userId, string actionType, string entityName, int? entityId, string ipAddress, string details)
        {
            string query = "BEGIN INSERT INTO VMS_AUDIT_LOG (AUDIT_ID, USER_ID, ACTION_TYPE, ENTITY_NAME, ENTITY_ID, IP_ADDRESS, DETAILS) VALUES (SEQ_VMS_AUDIT_LOG.NEXTVAL, :p_user, :p_action, :p_ent_name, :p_ent_id, :p_ip, :p_details); END;";
            
            using (OracleConnection conn = DBHelper.GetConnection())
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("p_user", OracleDbType.Int32).Value = (object)userId ?? DBNull.Value;
                    cmd.Parameters.Add("p_action", OracleDbType.Varchar2).Value = actionType;
                    cmd.Parameters.Add("p_ent_name", OracleDbType.Varchar2).Value = (object)entityName ?? DBNull.Value;
                    cmd.Parameters.Add("p_ent_id", OracleDbType.Int32).Value = (object)entityId ?? DBNull.Value;
                    cmd.Parameters.Add("p_ip", OracleDbType.Varchar2).Value = (object)ipAddress ?? DBNull.Value;
                    cmd.Parameters.Add("p_details", OracleDbType.Clob).Value = (object)details ?? DBNull.Value;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
