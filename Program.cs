using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

using Microsoft.Extensions.Configuration;



var builder = WebApplication.CreateBuilder(args);
AppConfig.Configuration = builder.Configuration;
builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Register SQLite Helper for the demo module
builder.Services.AddSingleton<VMS.DAL.SqliteDBHelper>();

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

// Ensure the Directory exists on startup (Crucial for Docker/Render SQLite volumes)
var dbDirectory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Data");
if (!System.IO.Directory.Exists(dbDirectory))
{
    System.IO.Directory.CreateDirectory(dbDirectory);
}

app.Run();

public static class AppConfig
{
    public static IConfiguration Configuration { get; set; }
}
