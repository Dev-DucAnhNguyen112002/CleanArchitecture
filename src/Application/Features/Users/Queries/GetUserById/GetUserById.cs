using AutoMapper;
using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Features.Users.Models;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Abstractions;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Domain.Entities;

namespace CleanArchitectureTest.Application.Features.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<UserDto>;

public class GetUserByIdQueryHandler(IUnitOfWork uow, IMapper mapper) : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResult<UserDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _uow.Repository<User>().GetByIdAsync(query.Id);
        Guard.Against.AppNotFound(query.Id, user);
        var dto = _mapper.Map<UserDto>(user);
        return dto;
    }
}
