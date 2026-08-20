using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Domain.Identity;
using TaskManagement.Infrastructure.Authentication.Configurations;
using TaskManagement.Infrastructure.Authentication.Settings;
using TaskManagement.Infrastructure.Persistence.Contexts;

namespace TaskManagement.Infrastructure.Authentication.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly RefreshTokenSettings _settings;

    public RefreshTokenService(
        ApplicationDbContext context,
        ITokenGenerator tokenGenerator,
        IOptions<RefreshTokenSettings> settings)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
        _settings = settings.Value;
    }
    public async Task<RefreshTokenResult> GenerateAsync(
    ApplicationUser user,
    string ipAddress,
    bool rememberMe)
    {
        var rawToken = _tokenGenerator.GenerateRefreshToken();

        var hashedToken = _tokenGenerator.HashToken(rawToken);
        var expiryDays = rememberMe
    ? 30
    : _settings.RefreshTokenExpiryDays;
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = hashedToken,
            CreatedAt = DateTime.UtcNow,
            //ExpiresAt = DateTime.UtcNow.AddDays(
            //    _settings.RefreshTokenExpiryDays),
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedByIp = ipAddress,
            ApplicationUserId = user.Id
        };

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync();

        return new RefreshTokenResult
        {
            RefreshToken = rawToken,
            RefreshTokenEntity = refreshToken
        };
    }
    public bool Validate(RefreshToken token)
    {
        if (token == null)
            return false;

        if (token.IsRevoked)
            return false;

        if (token.IsExpired)
            return false;

        return true;
    }
    public bool IsExpired(RefreshToken token)
    {
        return token.ExpiresAt <= DateTime.UtcNow;
    }
    public async Task RevokeAsync(
    RefreshToken refreshToken,
    string ipAddress,
    string reason)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;

        refreshToken.RevokedByIp = ipAddress;

        refreshToken.ReasonRevoked = reason;

        _context.RefreshTokens.Update(refreshToken);

        await _context.SaveChangesAsync();
    }
    public async Task<RefreshTokenResult> RotateAsync(
    RefreshToken refreshToken,
    string ipAddress)
    {
        var newToken = _tokenGenerator.GenerateRefreshToken();

        var newHash = _tokenGenerator.HashToken(newToken);

        refreshToken.RevokedAt = DateTime.UtcNow;

        refreshToken.RevokedByIp = ipAddress;

        refreshToken.ReplacedByTokenHash = newHash;

        refreshToken.ReasonRevoked = "Refresh Token Rotated";

        var replacement = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = newHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _settings.RefreshTokenExpiryDays),
            CreatedByIp = ipAddress,
            ApplicationUserId = refreshToken.ApplicationUserId
        };

        _context.RefreshTokens.Update(refreshToken);

        _context.RefreshTokens.Add(replacement);

        await _context.SaveChangesAsync();

        return new RefreshTokenResult
        {
            RefreshToken = newToken,
            RefreshTokenEntity = replacement
        };
    }

    public async Task<RefreshToken?> GetByTokenAsync(string rawToken)
    {
        var hash = _tokenGenerator.HashToken(rawToken);

        return await _context.RefreshTokens
            .Include(x => x.ApplicationUser)
            .FirstOrDefaultAsync(x => x.TokenHash == hash);
    }

    public async Task<List<RefreshToken>> GetActiveTokensAsync(string userId)
    {
        var now = DateTime.UtcNow;

        return await _context.RefreshTokens
            .Where(x =>
                x.ApplicationUserId == userId &&
                x.ExpiresAt > now &&
                x.RevokedAt == null)
            .ToListAsync();
    }
    public async Task<RefreshToken?> GetByIdAsync(Guid id)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task RevokeAllAsync(string userId, string reason)
    {
        var tokens = await _context.RefreshTokens
            .Where(x =>
                x.ApplicationUserId == userId &&
                x.RevokedAt == null &&
                x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReasonRevoked = reason;
        }

        await _context.SaveChangesAsync();
    }
}












//using TaskManagement.Application.Common.Interfaces;
//using TaskManagement.Application.Common.Models;
//using TaskManagement.Domain.Identity;

//namespace TaskManagement.Infrastructure.Authentication.Services;

//public class RefreshTokenService : IRefreshTokenService
//{
//    public Task<RefreshTokenResult> GenerateAsync(
//        ApplicationUser user,
//        string ipAddress)
//    {
//        throw new NotImplementedException();
//    }

//    public bool Validate(RefreshToken token)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<RefreshToken> RotateAsync(
//        RefreshToken refreshToken,
//        string ipAddress)
//    {
//        throw new NotImplementedException();
//    }

//    public Task RevokeAsync(
//        RefreshToken refreshToken,
//        string ipAddress,
//        string reason)
//    {
//        throw new NotImplementedException();
//    }

//    public bool IsExpired(RefreshToken refreshToken)
//    {
//        throw new NotImplementedException();
//    }
//}
