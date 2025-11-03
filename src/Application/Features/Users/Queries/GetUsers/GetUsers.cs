using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Features.Users.Models;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Abstractions;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTest.Application.Features.Users.Queries;

public record GetUsersQuery : PaginationRequest, IQueryPaging<UserDto>
{
    public string? UserName { get; set; }
}

public class GetUsersQueryHandler(IUnitOfWork uow, IMapper mapper) : IQueryPagingHandler<GetUsersQuery, UserDto>
{
    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var repo = uow.Repository<User>();
        var filter = repo.GetQueryable();
        if (!string.IsNullOrEmpty(request.UserName))
            filter = filter.Where(x => x.Username.Contains(request.UserName));
        else
            filter = filter.OrderByDescending(x => x.LastModified).ThenByDescending(x => x.Created);

        return await repo.GetPagedListAsync<UserDto>(query: filter,
                                                            mapper: mapper,
                                                            pageIndex: request.PageNumber,
                                                            pageSize: request.PageSize);
    }
}
