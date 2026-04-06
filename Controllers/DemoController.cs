using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using VMS.DAL;
using System;
using System.Collections.Generic;

namespace VMS.Controllers
{
    public class DemoController : Controller
    {
        private readonly SqliteDBHelper _dbHelper;

        public DemoController(SqliteDBHelper dbHelper)
        {
            _dbHelper = dbHelper;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = _dbHelper.GetConnection();
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS DemoVisitors (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    VisitDate DATETIME DEFAULT CURRENT_TIMESTAMP
                );
            ";
            command.ExecuteNonQuery();
        }

        public IActionResult Index()
        {
            var visitors = new List<string>();

            using var connection = _dbHelper.GetConnection();
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Name, VisitDate FROM DemoVisitors ORDER BY VisitDate DESC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                visitors.Add($"{reader.GetString(0)} (Visited: {reader.GetString(1)})");
            }

            return View(visitors); 
        }

        [HttpPost]
        public IActionResult AddVisitor(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return RedirectToAction("Index");

            using var connection = _dbHelper.GetConnection();
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO DemoVisitors (Name)
                VALUES ($name);
            ";
            command.Parameters.AddWithValue("$name", name);
            command.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}
