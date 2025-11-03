using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Abstractions;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;
using CleanArchitectureTest.Domain.Events;

namespace CleanArchitectureTest.Application.Features.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : ICommand<Guid>
{
    public Guid ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateTodoItemCommand, Guid>
{
    public async Task<BaseResult<Guid>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoItem>();

        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };
        await repo.AddAsync(entity);

        await unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
