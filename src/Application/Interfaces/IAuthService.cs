using CleanArchitectureTest.Contract.Models.Auth;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Interfaces;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    TokenResponse GenerateTokenResponse(User user, CancellationToken ct = default);
    Task<Guid?> TryRedeemRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}
