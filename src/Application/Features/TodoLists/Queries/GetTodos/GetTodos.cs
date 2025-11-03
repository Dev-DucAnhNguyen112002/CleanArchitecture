using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.TodoLists.Queries.GetTodos;
public record GetToDoListQuery : IRequest<PagedResult<TodoList>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetToDoListQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetToDoListQuery, PagedResult<TodoList>>
{


    public async Task<PagedResult<TodoList>> Handle(GetToDoListQuery request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.Repository<TodoList>();
        return await repo.GetPagedListAsync<TodoList>(repo.GetQueryable());
    }
}




