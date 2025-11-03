using CleanArchitectureTest.Application.Features.Users.Commands.CreateUser;
using CleanArchitectureTest.Application.Features.Users.Commands.DeleteUser;
using CleanArchitectureTest.Application.Features.Users.Commands.UpdateUser;
using CleanArchitectureTest.Application.Features.Users.Commands.UpdateUserInformation;
using CleanArchitectureTest.Application.Features.Users.Models;
using CleanArchitectureTest.Application.Features.Users.Queries;
using CleanArchitectureTest.Contract.Models;
using ISAT.Application.Features.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ISAT.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var usersGroup = app.MapGroup(this);

        usersGroup
            .MapGet(GetCurrentUser, "me")
            .MapGet(GetUsers)
            .MapGet(GetUserById, "{id}")
            .MapPost(CreateUser)
            .MapPut(UpdateUser, "{id}")
            .MapPut(UpdateUserInformation, "{id}/information")
            .MapDelete(DeleteUser, "{id}");
    }

    
    public async Task<Ok<PagedResult<UserDto>>> GetUsers([FromServices] ISender sender, [AsParameters] GetUsersQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public async Task<Results<Ok<BaseResult<UserDto>>, NotFound>> GetUserById([FromServices] ISender sender, Guid id)
    {
        try
        {
            var user = await sender.Send(new GetUserByIdQuery(id));
            return TypedResults.Ok(user);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    public async Task<Ok<BaseResult<CurrentUserDto>>> GetCurrentUser([FromServices] ISender sender)
    {
        var result = await sender.Send(new GetCurrentUserQuery());
        return TypedResults.Ok(result);
    }

    public async Task<Created<BaseResult<Guid>>> CreateUser([FromServices] ISender sender, [FromBody] CreateUserCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Created($"/users/{result.Data}", result);
    }

    public async Task<Created<BaseResult<Guid>>> Register([FromServices] ISender sender, [FromBody] CreateUserCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Created($"/users/{result.Data}", result);
    }

    public async Task<Results<NoContent, BadRequest, NotFound>> UpdateUser([FromServices] ISender sender, Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        try
        {
            await sender.Send(command);
            return TypedResults.NoContent();
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    public async Task<Results<NoContent, BadRequest, NotFound>> UpdateUserInformation([FromServices] ISender sender, Guid id, [FromBody] UpdateUserInformationCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        try
        {
            await sender.Send(command);
            return TypedResults.NoContent();
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    public async Task<Results<NoContent, NotFound>> DeleteUser([FromServices] ISender sender, Guid id)
    {
        try
        {
            await sender.Send(new DeleteUserCommand(id));
            return TypedResults.NoContent();
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
