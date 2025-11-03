using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Domain.Entities;
using CleanArchitectureTest.Domain.Events;

namespace CleanArchitectureTest.Application.Features.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(Guid Id) : IRequest;

public class DeleteTodoItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTodoItemCommand>
{

    public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoItem>();
        var entity = await repo.GetByIdAsync(request.Id);

        Guard.Against.AppNotFound(request.Id, entity);

        repo.Delete(entity);
        await unitOfWork.SaveChangesAsync();
    }

}
