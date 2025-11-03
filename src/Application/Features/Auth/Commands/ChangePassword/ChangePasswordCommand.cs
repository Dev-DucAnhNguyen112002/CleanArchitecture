using CleanArchitectureTest.Application.Interfaces;
using MediatR;
using CleanArchitectureTest.Application.Interfaces.Repositories;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<bool>;

public sealed class ChangePasswordCommandHandler(IUnitOfWork uow, IAuthService authService) : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.Repository<User>();
        var user = await repo.GetFirstOrDefaultAsync(u => u.Id == request.UserId, disableTracking: false);
        if (user is null)
            return false;
        if (user.IsActive.HasValue && user.IsActive.Value == false)
            return false;

        if (!authService.VerifyPassword(request.CurrentPassword, user.Passwordhash))
            return false;

        user.Passwordhash = authService.HashPassword(request.NewPassword);
        await uow.SaveChangesAsync();
        return true;
    }
}
