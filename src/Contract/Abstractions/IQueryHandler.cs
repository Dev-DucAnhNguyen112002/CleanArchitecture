using CleanArchitectureTest.Contract.Models;
using MediatR;

namespace CleanArchitectureTest.Contract.Abstractions;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, BaseResult<TResponse>>
    where TQuery : IQuery<TResponse>
{ }

public interface IQueryPagingHandler<TQuery, TResponse> : IRequestHandler<TQuery, PagedResult<TResponse>>
    where TQuery : IQueryPaging<TResponse>
{ }
