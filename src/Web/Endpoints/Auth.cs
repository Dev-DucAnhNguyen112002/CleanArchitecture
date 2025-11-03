using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Features.Auth.Commands.ChangePassword;
using CleanArchitectureTest.Application.Features.Auth.Commands.Login;
using CleanArchitectureTest.Application.Features.Auth.Commands.Refresh;
using CleanArchitectureTest.Application.Features.Users.Commands.CreateUser;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Contract.Models;
using CleanArchitectureTest.Contract.Models.Auth;
using CleanArchitectureTest.Web.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ISAT.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .WithTags(nameof(Auth))
            .MapPost(Login, "login")
            .MapPost(Refresh, "refresh")
            .MapPost(ChangePassword, "change-password")
            .MapPost(Register, "register");

    }
    public async Task<Created<BaseResult<Guid>>> Register([FromServices] ISender sender, [FromBody] CreateUserCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Created($"/users/{result.Data}", result);
    }
    public async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> Login([FromServices] ISender sender, [FromBody] LoginRequest request, CancellationToken ct)
    {
        var token = await sender.Send(new LoginCommand(request.Username, request.Password), ct);
        return token is not null
            ? TypedResults.Ok(token)
            : TypedResults.Unauthorized();
    }

    public async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> Refresh([FromServices] ISender sender, [FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var token = await sender.Send(new RefreshTokenCommand(request.RefreshToken), ct);
        return token is not null
            ? TypedResults.Ok(token)
            : TypedResults.Unauthorized();
    }

    public async Task<Results<Ok, UnauthorizedHttpResult, UnprocessableEntity>> ChangePassword(
        [FromServices] ISender sender,
        [FromServices] ICurrentUser currentUser,
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct)
    {
        if (currentUser.Id is null || !Guid.TryParse(currentUser.Id, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var ok = await sender.Send(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword), ct);
        return ok ? TypedResults.Ok() : TypedResults.UnprocessableEntity();
    }
}
