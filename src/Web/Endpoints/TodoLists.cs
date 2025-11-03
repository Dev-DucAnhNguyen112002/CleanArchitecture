using CleanArchitectureTest.Application.Features.TodoLists.Commands.CreateTodoList;
using CleanArchitectureTest.Application.Features.TodoLists.Commands.DeleteTodoList;
using CleanArchitectureTest.Application.Features.TodoLists.Commands.UpdateTodoList;
using CleanArchitectureTest.Application.Features.TodoLists.Queries.GetTodos;
using CleanArchitectureTest.Contract.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureTest.Web.Endpoints;

public class TodoLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateTodoList)
            .MapPut(UpdateTodoList, "{id}")
            .MapDelete(DeleteTodoList, "{id}");
    }


    public async Task<Created<BaseResult<Guid>>> CreateTodoList(ISender sender, CreateTodoListCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(TodoLists)}/{id}", id);
    }

    public async Task<Results<NoContent, BadRequest>> UpdateTodoList(ISender sender, Guid id, UpdateTodoListCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        
        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<NoContent> DeleteTodoList(ISender sender, Guid id)
    {
        await sender.Send(new DeleteTodoListCommand(id));

        return TypedResults.NoContent();
    }
}
