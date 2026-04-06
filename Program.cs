using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
AppConfig.Configuration = builder.Configuration;
builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Account/Login";
    });

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// ── Auto-Initialize Supabase PostgreSQL Schema on Startup ─────────────────────
try
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    using var conn = new NpgsqlConnection(connStr);
    conn.Open();

    using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        CREATE SEQUENCE IF NOT EXISTS seq_vms_visitors START 1;

        CREATE TABLE IF NOT EXISTS VMS_MASTER_DEPT (
            DEPT_ID SERIAL PRIMARY KEY,
            DEPT_NAME VARCHAR(100) NOT NULL,
            IS_ACTIVE INT DEFAULT 1
        );
        CREATE TABLE IF NOT EXISTS VMS_MASTER_IDPROOF (
            IDPROOF_ID SERIAL PRIMARY KEY,
            PROOF_NAME VARCHAR(100) NOT NULL,
            IS_ACTIVE INT DEFAULT 1
        );
        CREATE TABLE IF NOT EXISTS VMS_MASTER_HOST (
            HOST_ID SERIAL PRIMARY KEY,
            HOST_NAME VARCHAR(150) NOT NULL,
            DEPT_ID INT REFERENCES VMS_MASTER_DEPT(DEPT_ID),
            EMAIL VARCHAR(150),
            MOBILE VARCHAR(15),
            IS_ACTIVE INT DEFAULT 1
        );
        CREATE TABLE IF NOT EXISTS VMS_MASTER_GATE (
            GATE_ID SERIAL PRIMARY KEY,
            GATE_NUMBER VARCHAR(50) NOT NULL,
            IS_ACTIVE INT DEFAULT 1
        );
        CREATE TABLE IF NOT EXISTS VMS_USERS (
            USER_ID SERIAL PRIMARY KEY,
            USERNAME VARCHAR(100) UNIQUE NOT NULL,
            PASSWORD_HASH VARCHAR(256) NOT NULL,
            FULL_NAME VARCHAR(150) NOT NULL,
            ROLE VARCHAR(20) NOT NULL CHECK (ROLE IN ('USER', 'GUARD', 'ADMIN')),
            IS_ACTIVE INT DEFAULT 1,
            CREATED_DATE TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TABLE IF NOT EXISTS VMS_VISITORS (
            VISITOR_ID SERIAL PRIMARY KEY,
            VISIT_TOKEN VARCHAR(50) UNIQUE NOT NULL,
            FULL_NAME VARCHAR(150) NOT NULL,
            MOBILE VARCHAR(15) NOT NULL,
            EMAIL VARCHAR(150),
            COMPANY_NAME VARCHAR(150),
            PURPOSE VARCHAR(500) NOT NULL,
            HOST_ID INT REFERENCES VMS_MASTER_HOST(HOST_ID),
            DEPT_ID INT REFERENCES VMS_MASTER_DEPT(DEPT_ID),
            EXPECTED_DATETIME TIMESTAMP NOT NULL,
            IDPROOF_TYPE_ID INT REFERENCES VMS_MASTER_IDPROOF(IDPROOF_ID),
            IDPROOF_NUMBER VARCHAR(50) NOT NULL,
            STATUS VARCHAR(20) DEFAULT 'Pending',
            REGISTERED_BY INT REFERENCES VMS_USERS(USER_ID),
            CREATED_DATE TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TABLE IF NOT EXISTS VMS_FILE_UPLOADS (
            FILE_ID SERIAL PRIMARY KEY,
            VISITOR_ID INT REFERENCES VMS_VISITORS(VISITOR_ID),
            UPLOAD_TYPE VARCHAR(50),
            ORIGINAL_NAME VARCHAR(255) NOT NULL,
            STORED_NAME VARCHAR(255) NOT NULL,
            FILE_PATH VARCHAR(1000) NOT NULL,
            FILE_URL VARCHAR(1000) NOT NULL,
            FILE_SIZE_BYTES BIGINT,
            MIME_TYPE VARCHAR(50),
            UPLOADED_DATE TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TABLE IF NOT EXISTS VMS_CHECKIN_LOG (
            LOG_ID SERIAL PRIMARY KEY,
            VISITOR_ID INT REFERENCES VMS_VISITORS(VISITOR_ID),
            GATE_ID INT REFERENCES VMS_MASTER_GATE(GATE_ID),
            GUARD_ID INT REFERENCES VMS_USERS(USER_ID),
            CHECKIN_TIME TIMESTAMP NOT NULL,
            CHECKOUT_TIME TIMESTAMP,
            DURATION_MINUTES INT
        );
        CREATE TABLE IF NOT EXISTS VMS_AUDIT_LOG (
            AUDIT_ID SERIAL PRIMARY KEY,
            USER_ID INT REFERENCES VMS_USERS(USER_ID),
            ACTION_TYPE VARCHAR(100) NOT NULL,
            ENTITY_NAME VARCHAR(100),
            ENTITY_ID INT,
            IP_ADDRESS VARCHAR(50),
            ACTION_TIME TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            DETAILS TEXT
        );
    ";
    cmd.ExecuteNonQuery();

    // Seed admin user if not already present
    using var checkCmd = conn.CreateCommand();
    checkCmd.CommandText = "SELECT COUNT(*) FROM VMS_USERS WHERE ROLE = 'ADMIN'";
    var count = (long)checkCmd.ExecuteScalar();
    if (count == 0)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes("Surya@189489"));
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        using var seedCmd = conn.CreateCommand();
        seedCmd.CommandText = @"
            INSERT INTO VMS_USERS (USERNAME, PASSWORD_HASH, FULL_NAME, ROLE, IS_ACTIVE)
            VALUES (@username, @hash, 'System Administrator', 'ADMIN', 1)
            ON CONFLICT DO NOTHING";
        seedCmd.Parameters.AddWithValue("@username", "8142027323");
        seedCmd.Parameters.AddWithValue("@hash", hash);
        seedCmd.ExecuteNonQuery();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[DB INIT ERROR] {ex.Message}");
}
// ── End DB Init ────────────────────────────────────────────────────────────────

app.Run();

public static class AppConfig
{
    public static IConfiguration Configuration { get; set; }
}
