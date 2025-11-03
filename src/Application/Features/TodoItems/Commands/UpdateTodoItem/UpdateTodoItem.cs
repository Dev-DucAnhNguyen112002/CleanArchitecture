using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand : IRequest
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }
}

public class UpdateTodoItemCommandHandlerI(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTodoItemCommand>
{
    
    public async Task Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoItem>();
        var entity = await repo.GetByIdAsync(request.Id);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;
        entity.Done = request.Done;
        repo.Update(entity);
        await unitOfWork.SaveChangesAsync();
    }
}
