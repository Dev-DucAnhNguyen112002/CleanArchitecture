using System.Reflection;
using CleanArchitectureTest.Application.Common.Interfaces;
using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Application.Interfaces.Repositories;
using CleanArchitectureTest.Domain.Constants;
using CleanArchitectureTest.Infrastructure.Data;
using CleanArchitectureTest.Infrastructure.Data.Interceptors;
using CleanArchitectureTest.Infrastructure.Identity;
using CleanArchitectureTest.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("CleanArchitectureTestDb");
        Guard.Against.Null(connectionString, message: "Connection string 'CleanArchitectureTestDb' not found.");
    
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            //options.UseSqlServer(connectionString).AddAsyncSeeding(sp);
            options.UseNpgsql(connectionString, o => o.UseNetTopologySuite());

        });
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.RegisterRepositories(builder.Configuration);
        //builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        //builder.Services.AddScoped<ApplicationDbContextInitialiser>();
        builder.Services.AddSingleton(TimeProvider.System);

        // Auth service + JWT options
        //builder.Services.AddAuthentication()
        //    .AddBearerToken(IdentityConstants.BearerScheme);

        //builder.Services.AddAuthorizationBuilder();

        //builder.Services
        //    .AddIdentityCore<ApplicationUser>()
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>()
        //    .AddApiEndpoints();
        //builder.Services.AddTransient<IIdentityService, IdentityService>();
        //builder.Services.AddAuthorization(options =>
        //    options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
    }

    private static void RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var interfaceType = typeof(IGenericRepository<>);
        var interfaces = Assembly.GetAssembly(interfaceType)!.GetTypes()
            .Where(p => p.GetInterface(interfaceType.Name) != null);

        var implementations = Assembly.GetAssembly(typeof(GenericRepository<>))!.GetTypes();

        foreach (var item in interfaces)
        {
            var implementation = implementations.FirstOrDefault(p => p.GetInterface(item.Name) != null);

            if (implementation is not null)
                services.AddScoped(item, implementation);
        }
    }
}
