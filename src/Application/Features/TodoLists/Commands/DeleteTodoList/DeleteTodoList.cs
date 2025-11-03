using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.TodoLists.Commands.DeleteTodoList;

public record DeleteTodoListCommand(Guid Id) : IRequest;

public class DeleteTodoListCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTodoListCommand>
{

    public async Task Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoList>();
        var entity = await repo.GetByIdAsync(request.Id);

        Guard.Against.AppNotFound(request.Id, entity);

        repo.Delete(entity);

        await unitOfWork.SaveChangesAsync();
    }
}
