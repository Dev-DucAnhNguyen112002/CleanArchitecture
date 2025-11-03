using System;
using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Domain.Entities;
using MediatR;

namespace CleanArchitectureTest.Application.Features.Users.Commands.UpdateUserInformation;

public record UpdateUserInformationCommand : IRequest
{
    public Guid Id { get; init; }
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
  
}

public class UpdateUserInformationCommandHandler : IRequestHandler<UpdateUserInformationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserInformationCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(UpdateUserInformationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<User>();
        var user = await repository.GetByIdAsync(request.Id);
        Guard.Against.AppNotFound(request.Id, user);

        user.FullName = request.FullName;
        user.Phonenumber = request.PhoneNumber; 

        repository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
