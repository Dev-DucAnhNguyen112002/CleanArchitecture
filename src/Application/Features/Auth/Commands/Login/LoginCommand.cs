using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Models.Auth;
using MediatR;
using CleanArchitectureTest.Application.Interfaces.Repositories;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Username, string Password) : IRequest<TokenResponse?>;

public sealed class LoginCommandHandler(IUnitOfWork uow, IAuthService authService) : IRequestHandler<LoginCommand, TokenResponse?>
{
    public async Task<TokenResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
            var repo = uow.Repository<User>();
            var user = await repo.GetFirstOrDefaultAsync(u => u.Username == request.Username, disableTracking: true);
            if (user is null)
                return null;
            if (user.IsActive.HasValue && user.IsActive.Value == false)
                return null;

            var ok = authService.VerifyPassword(request.Password, user.Passwordhash);
            if (!ok)
                return null;

            var token = authService.GenerateTokenResponse(user, cancellationToken);
            return token;
    }
}
