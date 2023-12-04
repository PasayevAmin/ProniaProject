using FrontToBack.DAL;
using FrontToBack.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromDays(1);
});
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(
    opt=>opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    );
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
builder.Services.AddScoped<LayoutServices>();

var app = builder.Build();
app.UseSession();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    "area",
    "{area:exists}/{controller=Dashboard}/{action=index}/{id?}"
    );

app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();
