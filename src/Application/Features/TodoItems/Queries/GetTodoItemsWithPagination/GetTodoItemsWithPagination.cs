
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsWithPaginationQuery : PaginationRequest,IRequest<PagedResult<TodoItemBriefDto>>
{
    public Guid ListId { get; init; }
}

public class GetTodoItemsWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetTodoItemsWithPaginationQuery, PagedResult<TodoItemBriefDto>>
{

    public async Task<PagedResult<TodoItemBriefDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoItem>();
        return await repo.GetPagedListAsync<TodoItemBriefDto>(repo.GetQueryable(), mapper: mapper);
    }
}
