using System;
using System.Data;
using Npgsql;

namespace VMS.DAL
{
    public static class DBHelper
    {
        public static string GetConnectionString()
        {
            return AppConfig.Configuration.GetConnectionString("DefaultConnection");
        }

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(GetConnectionString());
        }

        public static DataTable ExecuteQuery(string cmdText, NpgsqlParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(cmdText, conn))
                {
                    cmd.CommandType = cmdType;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string cmdText, NpgsqlParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            using (NpgsqlConnection conn = GetConnection())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(cmdText, conn))
                {
                    cmd.CommandType = cmdType;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ExecuteNonQueryWithOutParams(string cmdText, NpgsqlParameter[] parameters, CommandType cmdType = CommandType.Text)
        {
            using (NpgsqlConnection conn = GetConnection())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(cmdText, conn))
                {
                    cmd.CommandType = cmdType;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
