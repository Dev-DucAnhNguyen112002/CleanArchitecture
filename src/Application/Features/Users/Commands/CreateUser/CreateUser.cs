using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Abstractions;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;
using MediatR;
namespace CleanArchitectureTest.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand : ICommand<Guid>
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string Password { get; set; } = null!;
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuthService _authService;

    public CreateUserCommandHandler(IUnitOfWork uow, IAuthService authService)
    {
        _uow = uow;
        _authService = authService;
    }
    public async Task<BaseResult<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<User>();
        var existedUserName = await repo.ExistsByPropertyAsync<Guid>(nameof(User.Username), command.Username);
        Guard.Against.Duplicate(existedUserName, nameof(User.Username));
        var existedMail = await repo.ExistsByPropertyAsync<Guid>(nameof(User.Email), command.Email);
        Guard.Against.Duplicate(existedMail, nameof(User.Email));
        var user = new User
        {
            Username = command.Username,
            Normalizedusername = command.Username.ToUpperInvariant(),
            Email = command.Email,
            Normalizedemail = command.Email.ToUpperInvariant(),
            IsActive = true,
            FullName = command.FullName,
            Passwordhash = _authService.HashPassword(command.Password),
            // Bạn cũng nên gán các giá trị mặc định khác nếu cần
            Lockoutenabled = true // Ví dụ: mặc định là bật tính năng khóa
        };

        await repo.AddAsync(user);
        await _uow.SaveChangesAsync();
        return user.Id;
    }
}
