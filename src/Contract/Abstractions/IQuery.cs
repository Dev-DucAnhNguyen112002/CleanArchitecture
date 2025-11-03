using CleanArchitectureTest.Contract.Models;
using MediatR;

namespace CleanArchitectureTest.Contract.Abstractions;

public interface IQuery<TResponse> : IRequest<BaseResult<TResponse>>
{ }


public interface IQueryPaging<TResponse> : IRequest<PagedResult<TResponse>>
{ }
