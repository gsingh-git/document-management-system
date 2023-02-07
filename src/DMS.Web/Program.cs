using DMS.Data;
using EntityFrameworkCore.UseRowNumberForPaging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
services.AddControllersWithViews();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = new PathString("/Auth/Index/");
                        options.AccessDeniedPath = new PathString("/Auth/Forbidden/");
                    });

services.AddDbContext<DMSContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DmsDb"), b => b.UseRowNumberForPaging()));

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);

});
services.AddSingleton<IFileProvider>(
new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

services.AddAuthorization(options =>
{    options.AddPolicy("Admin", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "Admin");
    });
    options.AddPolicy("User", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "User");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
