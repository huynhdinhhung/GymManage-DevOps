using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GYM_Manage.Data;
using Microsoft.AspNetCore.Http;
using GYM_Manage.Models.Momo;
using GYM_Manage.Services;
using Hangfire; // <--- thêm
using Hangfire.SqlServer; // <--- thêm

var builder = WebApplication.CreateBuilder(args);

// ================== DỊCH VỤ ==================
builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmailService, EmailService>();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Database
builder.Services.AddDbContext<GYM_DBcontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===========================================================
// HANGFIRE + SERVICE THÔNG BÁO
// ===========================================================
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddScoped<ThongBaoService>();

var app = builder.Build();

// ================== MIDDLEWARE ==================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// ===========================================================
// HANGFIRE DASHBOARD + JOB MỖI NGÀY
// ===========================================================
app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<ThongBaoService>(
    "nhac-lich-tap-hang-ngay",
    service => service.TaoThongBaoLichTapTrongNgay(),
    "0 6 * * *",
    TimeZoneInfo.Local
);

// ===========================================================
//   1. CHẶN TRUY CẬP ADMIN — CHỈ Admin & Staff
// ===========================================================
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var role = context.Session.GetString("UserRole");

    if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
    {
        if (string.IsNullOrEmpty(role) ||
           !(role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
             role.Equals("Staff", StringComparison.OrdinalIgnoreCase)))
        {
            context.Response.Redirect("/Auth/Login");
            return;
        }
    }

    await next();
});

// ===========================================================
//   2. CHẶN TRUY CẬP KHU VỰC HLV — CHỈ Trainer
// ===========================================================
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var role = context.Session.GetString("UserRole");

    if (path.StartsWith("/HLV", StringComparison.OrdinalIgnoreCase))
    {
        if (path.Equals("/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Home", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Auth", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        if (string.IsNullOrEmpty(role) ||
            !role.Equals("Trainer", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect("/Auth/Login");
            return;
        }
    }

    await next();
});

// ===========================================================
// TRANG LỖI HTTP
// ===========================================================
app.UseStatusCodePagesWithReExecute("/Error/{0}");

// ===========================================================
// ROUTING
// ===========================================================
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
