using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Abstractions;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;
using MediatR;

namespace CleanArchitectureTest.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteUserCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var repo = _uow.Repository<User>();
        var User = await repo.GetByIdAsync(command.Id);
        Guard.Against.AppNotFound(command.Id, User);

        repo.Delete(User);
        await _uow.SaveChangesAsync();
    }
}
