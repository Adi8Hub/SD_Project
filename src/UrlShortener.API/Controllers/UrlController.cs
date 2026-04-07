using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrlShortener.API.Services;

[ApiController]
[Route("api/url")]
[EnableRateLimiting("fixed")]
public class UrlController : ControllerBase
{
    private readonly IUrlService _service;

    public UrlController(IUrlService service)
    {
        _service = service;
    }

    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] string longUrl)
    {
        var code = await _service.CreateShortUrl(longUrl);
        return Ok(new { shortUrl = code });
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToOriginal(string code)
    {
        var url = await _service.GetLongUrl(code);
        if (url == null) return NotFound();
        return Redirect(url);
    }

    [HttpGet("metrics")]
    public IActionResult Metrics()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
