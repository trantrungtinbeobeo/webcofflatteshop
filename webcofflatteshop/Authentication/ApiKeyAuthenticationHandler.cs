using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using webcofflatteshop.Data;

namespace webcofflatteshop.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly ApplicationDbContext _context;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApplicationDbContext context)
        : base(options, logger, encoder)
    {
        _context = context;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var values))
        {
            return AuthenticateResult.NoResult();
        }

        var rawKey = values.FirstOrDefault()?.Trim();
        if (string.IsNullOrWhiteSpace(rawKey) || !rawKey.StartsWith("clk_", StringComparison.Ordinal))
        {
            return AuthenticateResult.Fail("API Key không hợp lệ.");
        }

        var keyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)));
        var apiKey = await _context.ApiKeys
            .FirstOrDefaultAsync(key => key.KeyHash == keyHash && key.IsActive);

        if (apiKey is null)
        {
            return AuthenticateResult.Fail("API Key không hợp lệ hoặc đã bị thu hồi.");
        }

        var now = DateTime.UtcNow;
        if (apiKey.LastUsedAt is null || apiKey.LastUsedAt.Value < now.AddMinutes(-5))
        {
            apiKey.LastUsedAt = now;
            await _context.SaveChangesAsync();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, apiKey.UserId),
            new Claim(ClaimTypes.Name, apiKey.Name),
            new Claim("api_key_id", apiKey.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}
