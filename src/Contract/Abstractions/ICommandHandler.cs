using CleanArchitectureTest.Contract.Models;
using MediatR;

namespace CleanArchitectureTest.Contract.Abstractions;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, BaseResult>
    where TCommand : ICommand
{ }

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, BaseResult<TResponse>>
    where TCommand : ICommand<TResponse>
{ }
