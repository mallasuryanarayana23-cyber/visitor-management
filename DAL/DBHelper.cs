using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.Sqlite;

namespace VMS.DAL
{
    public static class DBHelper
    {
        public static string GetConnectionString()
        {
            return AppConfig.Configuration.GetConnectionString("DefaultConnection");
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(GetConnectionString());
        }

        public static DataTable ExecuteQuery(string cmdText, SqliteParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            using (SqliteConnection conn = GetConnection())
            {
                using (SqliteCommand cmd = new SqliteCommand(cmdText, conn))
                {
                    cmd.CommandType = cmdType;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string cmdText, SqliteParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            using (SqliteConnection conn = GetConnection())
            {
                using (SqliteCommand cmd = new SqliteCommand(cmdText, conn))
                {
                    cmd.CommandType = cmdType;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        
         public static void ExecuteNonQueryWithOutParams(string cmdText, SqliteParameter[] parameters, CommandType cmdType = CommandType.Text)
        {
            using (SqliteConnection conn = GetConnection())
            {
                using (SqliteCommand cmd = new SqliteCommand(cmdText, conn))
                {
                    cmd.CommandType = cmdType;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
