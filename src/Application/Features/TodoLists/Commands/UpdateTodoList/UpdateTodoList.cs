using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.TodoLists.Commands.UpdateTodoList;

public record UpdateTodoListCommand : IRequest
{
    public Guid Id { get; init; }

    public string? Title { get; init; }
}

public class UpdateTodoListCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTodoListCommand>
{


    public async Task Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoList>();

        var entity = await repo.GetByIdAsync(request.Id);

        Guard.Against.AppNotFound(request.Id, entity);

        entity.Title = request.Title;

        await unitOfWork.SaveChangesAsync();
    }
}
