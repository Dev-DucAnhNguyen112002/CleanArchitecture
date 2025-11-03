using CleanArchitectureTest.Contract.Models;
using MediatR;

namespace CleanArchitectureTest.Contract.Abstractions;

public interface ICommand : IRequest<BaseResult>
{
}

public interface ICommand<TResponse> : IRequest<BaseResult<TResponse>>
{
}
