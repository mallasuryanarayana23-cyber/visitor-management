using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Npgsql;
using VMS.Models;

namespace VMS.DAL
{
    public class UserDAL
    {
        public UserModel AuthenticateUser(string username, string password)
        {
            string passwordHash = ComputeSHA256Hash(password);

            using (NpgsqlConnection conn = DBHelper.GetConnection())
            {
                string query = "SELECT USER_ID, USERNAME, FULL_NAME, ROLE, IS_ACTIVE FROM VMS_USERS WHERE USERNAME = @p_USERNAME AND PASSWORD_HASH = @p_PASSWORD_HASH;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@p_USERNAME", username);
                    cmd.Parameters.AddWithValue("@p_PASSWORD_HASH", passwordHash);

                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
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

            using (NpgsqlConnection conn = DBHelper.GetConnection())
            {
                string sql = @"UPDATE VMS_USERS SET PASSWORD_HASH = @p_pwd WHERE USERNAME = @p_emp AND ROLE = 'GUARD';";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@p_emp", empNo);
                    cmd.Parameters.AddWithValue("@p_pwd", passwordHash);
                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    return affected > 0;
                }
            }
        }

        public DataTable GetUsers()
        {
            using (NpgsqlConnection conn = DBHelper.GetConnection())
            {
                string query = "SELECT USER_ID, USERNAME, FULL_NAME, ROLE, IS_ACTIVE, CREATED_DATE FROM VMS_USERS ORDER BY CREATED_DATE DESC;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public bool CreateUser(string username, string password, string fullName, string role)
        {
            string passwordHash = ComputeSHA256Hash(password);

            using (NpgsqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"INSERT INTO VMS_USERS (USER_ID, USERNAME, PASSWORD_HASH, FULL_NAME, ROLE, IS_ACTIVE) 
                               VALUES (nextval('SEQ_VMS_USERS'), @p_username, @p_pwd, @p_name, @p_role, 1);";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@p_username", username);
                    cmd.Parameters.AddWithValue("@p_pwd", passwordHash);
                    cmd.Parameters.AddWithValue("@p_name", fullName);
                    cmd.Parameters.AddWithValue("@p_role", role.ToUpper());
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateUser(string username, string fullName, string role, int isActive)
        {
            using (NpgsqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"UPDATE VMS_USERS SET FULL_NAME = @p_name, ROLE = @p_role, IS_ACTIVE = @p_status WHERE USERNAME = @p_username;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@p_username", username);
                    cmd.Parameters.AddWithValue("@p_name", fullName);
                    cmd.Parameters.AddWithValue("@p_role", role.ToUpper());
                    cmd.Parameters.AddWithValue("@p_status", isActive);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
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
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        public void LogAudit(int? userId, string actionType, string entityName, int? entityId, string ipAddress, string details)
        {
            string query = "INSERT INTO VMS_AUDIT_LOG (USER_ID, ACTION_TYPE, ENTITY_NAME, ENTITY_ID, IP_ADDRESS, DETAILS) VALUES (@p_user, @p_action, @p_ent_name, @p_ent_id, @p_ip, @p_details);";

            using (NpgsqlConnection conn = DBHelper.GetConnection())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@p_user", (object)userId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_action", actionType);
                    cmd.Parameters.AddWithValue("@p_ent_name", (object)entityName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_ent_id", (object)entityId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_ip", (object)ipAddress ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_details", (object)details ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
