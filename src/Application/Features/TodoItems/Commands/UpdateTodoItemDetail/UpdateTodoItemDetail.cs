using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Domain.Entities;
using CleanArchitectureTest.Domain.Enums;

namespace CleanArchitectureTest.Application.Features.TodoItems.Commands.UpdateTodoItemDetail;

public record UpdateTodoItemDetailCommand : IRequest
{
    public Guid Id { get; init; }

    public Guid ListId { get; init; }

    public int Priority { get; init; }

    public string? Note { get; init; }
}

public class UpdateTodoItemDetailCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTodoItemDetailCommand>
{

    public async Task Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoItem>();

        var entity = await repo.GetByIdAsync(request.Id);

        Guard.Against.AppNotFound(request.Id, entity);

        entity.ListId = request.ListId;
        entity.Priority = request.Priority;
        entity.Note = request.Note;


        repo.Update(entity);
        await unitOfWork.SaveChangesAsync();
    }
}
