using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using UrlShortener.API.Data;
using UrlShortener.API.Services;
using UrlShortener.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer("Server=sqlserver;Database=UrlDb;User=sa;Password=Your_password123;"));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379";
});

builder.Services.AddScoped<IUrlService, UrlService>();

builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("fixed", o =>
    {
        o.PermitLimit = 100;
        o.Window = TimeSpan.FromMinutes(1);
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseRateLimiter();

app.MapControllers();

app.Run();
