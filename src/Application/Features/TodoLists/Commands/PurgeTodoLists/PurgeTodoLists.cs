using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Common.Security;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Domain.Constants;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.TodoLists.Commands.PurgeTodoLists;

public record PurgeTodoListsCommand : IRequest;

public class PurgeTodoListsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<PurgeTodoListsCommand>
{

    public async Task Handle(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoList>();
        var entity = await repo.GetAllAsync();
        repo.DeleteRange(entity);

        await unitOfWork.SaveChangesAsync();
    }
}
