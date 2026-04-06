using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;

namespace VMS.DAL
{
    public class SqliteDBHelper
    {
        private readonly string _connectionString;

        public SqliteDBHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqliteConnection") 
                ?? throw new InvalidOperationException("Connection string 'SqliteConnection' not found.");
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
