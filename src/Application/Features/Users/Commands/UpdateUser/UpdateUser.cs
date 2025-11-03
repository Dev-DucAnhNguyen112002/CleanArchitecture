using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Domain.Entities;
using MediatR;

namespace CleanArchitectureTest.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand : IRequest
{
    public Guid Id { get; init; }
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool? IsActive { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUnitOfWork _uow;

    public UpdateUserCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<User>();
        var user = await repo.GetByIdAsync(command.Id);
        Guard.Against.AppNotFound(command.Id, user);

        // 1. Chuẩn hóa đầu vào (username và email mới)
        var normalizedUser = command.Username.ToUpperInvariant();
        var normalizedMail = command.Email.ToUpperInvariant();

        // 2. Kiểm tra trùng lặp trên các cột Normalized (và bỏ qua chính user này)
        var existedUserName = await repo.ExistsByPropertyAsync<Guid>(nameof(User.Normalizedusername), normalizedUser, nameof(User.Id), command.Id);
        Guard.Against.Duplicate(existedUserName, nameof(User.Username), "Ten dang nhap da ton tai trong he thong");
        var existedMail = await repo.ExistsByPropertyAsync<Guid>(nameof(User.Normalizedemail), normalizedMail, nameof(User.Id), command.Id);
        Guard.Against.Duplicate(existedMail, nameof(User.Email), "Email nay da ton tai trong he thong");

        // 3. Cập nhật TẤT CẢ các trường, bao gồm cả Normalized
        user.Username = command.Username;
        user.Normalizedusername = normalizedUser; 
        user.Email = command.Email;
        user.Normalizedemail = normalizedMail;   
        user.FullName = command.FullName;
        user.IsActive = command.IsActive;

        repo.Update(user);
        await _uow.SaveChangesAsync();
    }
}
