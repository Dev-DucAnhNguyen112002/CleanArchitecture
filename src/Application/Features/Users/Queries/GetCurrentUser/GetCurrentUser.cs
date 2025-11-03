using System;
using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Features.Users.Models;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Abstractions;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.Users.Queries;

public record GetCurrentUserQuery : IQuery<CurrentUserDto>;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IQueryHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResult<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Forbidden(!_currentUser.IsAuthenticated, "User is not authenticated.");

        Guard.Against.BadRequest(!Guid.TryParse(_currentUser.Id, out var userId), "Invalid user identifier.");

        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
        Guard.Against.AppNotFound(userId, user);

        var dto = _mapper.Map<CurrentUserDto>(user);

        var enriched = dto with
        {
            Username = _currentUser.Username ?? dto.Username,
            Email = _currentUser.Email ?? dto.Email,
            FullName = _currentUser.FullName ?? dto.FullName,
            Phonenumber = _currentUser.Phonenumber ?? dto.Phonenumber,
            IsAuthenticated = true
        };

        return enriched;
    }
}
