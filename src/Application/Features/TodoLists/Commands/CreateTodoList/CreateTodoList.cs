using System.Windows.Input;
using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Abstractions;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand : ICommand<Guid>
{
    public string? Title { get; init; }
}

public class CreateTodoListCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateTodoListCommand, Guid>
{
    public async Task<BaseResult<Guid>> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoList>();
        var entity = new TodoList();

        entity.Title = request.Title;

        await repo.AddAsync(entity);

        await unitOfWork.SaveChangesAsync();

        return entity.Id;
    }
}
