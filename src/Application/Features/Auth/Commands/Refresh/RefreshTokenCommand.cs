using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Application.Interfaces.Repositories;
using CleanArchitectureTest.Contract.Models.Auth;
using CleanArchitectureTest.Domain.Entities;
using MediatR;

namespace CleanArchitectureTest.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponse?>;

public sealed class RefreshTokenCommandHandler(IUnitOfWork uow, IAuthService authService) : IRequestHandler<RefreshTokenCommand, TokenResponse?>
{
    public async Task<TokenResponse?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = await authService.TryRedeemRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
            return null;

        var repo = uow.Repository<User>();
        var user = await repo.GetFirstOrDefaultAsync(u => u.Id == userId.Value, disableTracking: true);
        if (user is null)
            return null;
        if (user.IsActive.HasValue && user.IsActive.Value == false)
            return null;

        var token = authService.GenerateTokenResponse(user, cancellationToken);
        return token;
    }
}
