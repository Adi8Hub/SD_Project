using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Models;

public class AppDbContext : DbContext
{
    public DbSet<UrlMapping> UrlMappings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
}
