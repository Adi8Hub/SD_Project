using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using UrlShortener.API.Data;
using UrlShortener.API.Models;

public interface IUrlService
{
    Task<string> CreateShortUrl(string longUrl);
    Task<string> GetLongUrl(string code);
}

public class UrlService : IUrlService
{
    private readonly AppDbContext _db;
    private readonly IDistributedCache _cache;

    private const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public UrlService(AppDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<string> CreateShortUrl(string longUrl)
    {
        var entity = new UrlMapping { LongUrl = longUrl };
        _db.UrlMappings.Add(entity);
        await _db.SaveChangesAsync();

        var code = Encode(entity.Id);
        entity.ShortCode = code;
        await _db.SaveChangesAsync();

        await _cache.SetStringAsync(code, longUrl);
        return code;
    }

    public async Task<string> GetLongUrl(string code)
    {
        var cached = await _cache.GetStringAsync(code);
        if (cached != null) return cached;

        var entity = await _db.UrlMappings.FirstOrDefaultAsync(x => x.ShortCode == code);
        if (entity == null) return null;

        await _cache.SetStringAsync(code, entity.LongUrl);
        return entity.LongUrl;
    }

    private string Encode(int id)
    {
        var sb = new StringBuilder();
        while (id > 0)
        {
            sb.Insert(0, chars[id % 62]);
            id /= 62;
        }
        return sb.ToString();
    }
}
