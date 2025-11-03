using CleanArchitectureTest.Infrastructure.Identity;

namespace CleanArchitectureTest.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var usersGroup = app.MapGroup(this);
    }
}
