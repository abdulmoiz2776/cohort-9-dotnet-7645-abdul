using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Infrastructure.Authentication.Settings;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Infrastructure.Authentication.Services;

public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    private string Issuer => !string.IsNullOrWhiteSpace(_settings.Issuer)
        ? _settings.Issuer
        : _configuration["Jwt:Issuer"] ?? string.Empty;

    private string Audience => !string.IsNullOrWhiteSpace(_settings.Audience)
        ? _settings.Audience
        : _configuration["Jwt:Audience"] ?? string.Empty;

    private string Secret => !string.IsNullOrWhiteSpace(_settings.Secret)
        ? _settings.Secret
        : _configuration["Jwt:Secret"] ?? string.Empty;

    public JwtService(
        IOptions<JwtSettings> options,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _settings = options.Value;
        _userManager = userManager;
        _configuration = configuration;

        Console.WriteLine($"JwtService constructor: Issuer='{Issuer}', Audience='{Audience}', SecretLength={Secret.Length}");
    }

    public async Task<string> GenerateAccessTokenAsync(
     string userId,
     string userName,
     string email,
     IList<string> roles)
    {
        Console.WriteLine("Step 1");

        var claims = await GenerateClaimsAsync(userId, userName, email, roles);

        Console.WriteLine("Step 2");

        Console.WriteLine($"Configured secret length: {Secret.Length}");
        Console.WriteLine($"Configured issuer: '{Issuer}'");
        Console.WriteLine($"Configured audience: '{Audience}'");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Secret));

        Console.WriteLine($"Step 3 - Signing key length: {Secret.Length}");

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        Console.WriteLine("Step 4");

        if (string.IsNullOrWhiteSpace(Issuer))
        {
            throw new InvalidOperationException("JWT issuer is not configured. Check Jwt:Issuer in appsettings.");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            throw new InvalidOperationException("JWT audience is not configured. Check Jwt:Audience in appsettings.");
        }

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        Console.WriteLine("Step 5");
        Console.WriteLine($"Generated token audiences: {string.Join(',', token.Audiences)}");
        Console.WriteLine($"Generated token exp: {token.ValidTo:o}");

        return tokenString;
    }
    public Task<List<Claim>> GenerateClaimsAsync(
        string userId,
        string userName,
        string email,
        IList<string> roles)
    {
        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, userId),
        new(JwtRegisteredClaimNames.Email, email),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

        new(ClaimTypes.NameIdentifier, userId),
        new(ClaimTypes.Name, userName),
        new(ClaimTypes.Email, email),

        new("UserId", userId),
        new("Username", userName),
        new("TokenId", Guid.NewGuid().ToString())
    };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return Task.FromResult(claims);
    }
    //public ClaimsPrincipal? ValidateToken(string token)
    //{

    //    Console.WriteLine($"Issuer: {_settings.Issuer}");
    //    Console.WriteLine($"Audience: {_settings.Audience}");
    //    Console.WriteLine($"Secret Length: {_settings.Secret.Length}");
    //    var handler = new JwtSecurityTokenHandler();

    //    try
    //    {
    //        return handler.ValidateToken(
    //            token,
    //            GetValidationParameters(true),
    //            out _);
    //    }
    //    catch
    //    {
    //        return null;
    //    }
    //}
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        

        token = token?.Trim() ?? string.Empty;

        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token[7..].Trim();
        }

        token = token.Trim('"');

        var jwt = handler.ReadJwtToken(token);
        Console.WriteLine($"Token payload issuer: '{jwt.Issuer}'");
        Console.WriteLine($"Token payload audiences: {string.Join(',', jwt.Audiences)}");
        Console.WriteLine($"Token payload audience count: {jwt.Audiences.Count()}\n");
        Console.WriteLine($"Token payload claims contains iss: {jwt.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Iss)}");
        Console.WriteLine($"Token payload exp present: {jwt.Payload.ContainsKey(JwtRegisteredClaimNames.Exp)}");

        if (jwt.Payload.TryGetValue(JwtRegisteredClaimNames.Exp, out var expValue))
        {
            Console.WriteLine($"Token payload exp value: {expValue}");
        }
        else
        {
            Console.WriteLine("Token payload exp value: <missing>");
        }

        Console.WriteLine($"Token ValidTo: {jwt.ValidTo:o}");

        var parameters = GetValidationParameters(true);

        try
        {
            return handler.ValidateToken(
                token,
                parameters,
                out _);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ValidateToken exception: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var principal = handler.ValidateToken(
            token,
            GetValidationParameters(false),
            out var securityToken);

        if (securityToken is not JwtSecurityToken jwt ||
            !jwt.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token.");
        }

        return principal;
    }

    private TokenValidationParameters GetValidationParameters(bool validateLifetime)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            RequireSignedTokens = true,
            ValidateLifetime = validateLifetime,
            RequireExpirationTime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Secret)),
            ClockSkew = TimeSpan.Zero
        };

        if (!string.IsNullOrWhiteSpace(Issuer))
        {
            parameters.ValidateIssuer = true;
            parameters.ValidIssuer = Issuer;
        }

        if (!string.IsNullOrWhiteSpace(Audience))
        {
            parameters.ValidateAudience = true;
            parameters.ValidAudience = Audience;
        }

        return parameters;
    }
}