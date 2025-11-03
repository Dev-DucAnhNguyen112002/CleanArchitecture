using MediatR;

namespace CleanArchitectureTest.Contract.Abstractions;

public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
