using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;
using CleanArchitectureTest.Contract.Constants;
using CleanArchitectureTest.Contract.Models.Auth;
using CleanArchitectureTest.Domain.Configs;
using CleanArchitectureTest.Domain.Entities;
using CleanArchitectureTest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CleanArchitectureTest.Application.Interfaces;

namespace ISAT.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly JwtConfig _jwtOptions;
    private readonly IMemoryCache _cache;
    private readonly ApplicationDbContext _dbContext;

    private const string RefreshCachePrefix = "refresh:";

    public AuthService(IOptions<JwtConfig> jwtOptions, IMemoryCache cache, ApplicationDbContext dbContext)
    {
        _jwtOptions = jwtOptions.Value;
        _cache = cache;
        _dbContext = dbContext;
    }

    public TokenResponse GenerateTokenResponse(User user, CancellationToken ct = default)
    {
        var token = CreateJwt(user, out var expiresAtUtc);
        var refreshToken = GenerateRefreshToken();
        StoreRefreshToken(refreshToken, user.Id.ToString(), TimeSpan.FromDays(7));

        return new TokenResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            RefreshToken = refreshToken
        };
    }

    public Task<Guid?> TryRedeemRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Task.FromResult<Guid?>(null);

        if (!_cache.TryGetValue<string>(RefreshCachePrefix + refreshToken, out var userId) || string.IsNullOrEmpty(userId))
            return Task.FromResult<Guid?>(null);

        _cache.Remove(RefreshCachePrefix + refreshToken);

        if (!Guid.TryParse(userId, out var id))
            return Task.FromResult<Guid?>(null);

        return Task.FromResult<Guid?>(id);
    }

    public string HashPassword(string password)
        => BCryptNet.HashPassword(password);

    public bool VerifyPassword(string password, string passwordHash)
        => BCryptNet.Verify(password, passwordHash);

    private string CreateJwt(User user, out DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var nowUtc = DateTime.UtcNow;
        var expires = nowUtc.AddMinutes(Math.Max(1, _jwtOptions.Expired));
        expiresAtUtc = expires;

        var claims = BuildClaims(user);

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(_jwtOptions.Issuer) ? null : _jwtOptions.Issuer,
            audience: string.IsNullOrWhiteSpace(_jwtOptions.Audience) ? null : _jwtOptions.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private void StoreRefreshToken(string refreshToken, string userId, TimeSpan lifetime)
    {
        _cache.Set(RefreshCachePrefix + refreshToken, userId, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = lifetime
        });
    }

    private static IReadOnlyCollection<Claim> BuildClaims(User user)
    {
        var claims = new List<Claim>
    {
        // Các claim này OK vì user.Username và user.Id đều tồn tại
        new(JwtRegisteredClaimNames.Sub, user.Username),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Username),
        // Giả sử UserClaimTypes là các hằng số string bạn tự định nghĩa
        new(UserClaimTypes.UserId, user.Id.ToString()),
        new(UserClaimTypes.Username, user.Username)
    };

        // OK - user.Email tồn tại
        AddClaimIfPresent(claims, ClaimTypes.Email, user.Email);

        // THÊM MỚI - user.Phonenumber tồn tại
        // Bạn có thể dùng ClaimTypes.MobilePhone hoặc ClaimTypes.HomePhone tùy ý
        AddClaimIfPresent(claims, ClaimTypes.MobilePhone, user.Phonenumber);
        AddClaimIfPresent(claims, UserClaimTypes.FullName, user.FullName);

        // THÊM MỚI - Thêm các Role của user vào claims
        // Điều này RẤT QUAN TRỌNG để phân quyền (Authorization)
        if (user.Roles != null)
        {
            foreach (var role in user.Roles)
            {
                // Giả định rằng class Role của bạn có thuộc tính 'Name' (ví dụ: "Admin", "User")
                // Nếu thuộc tính tên là khác, hãy thay 'role.Name' ở đây
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
        }

        // LOẠI BỎ - Các thuộc tính này không tồn tại trên class User của bạn
        // AddClaimIfPresent(claims, UserClaimTypes.AvatarUrl, user.AvatarUrl);
        // AddClaimIfPresent(claims, UserClaimTypes.StorageQuota, user.StorageQuota);
        // AddClaimIfPresent(claims, UserClaimTypes.StorageUsed, user.StorageUsed);

        return claims;
    }

    private static void AddClaimIfPresent(ICollection<Claim> claims, string type, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            claims.Add(new Claim(type, value));
        }
    }

    private static void AddClaimIfPresent(ICollection<Claim> claims, string type, long? value)
    {
        if (value is not null)
        {
            claims.Add(new Claim(type, value.Value.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
