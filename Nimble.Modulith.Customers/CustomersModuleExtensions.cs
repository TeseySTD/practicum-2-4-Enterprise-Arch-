using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Customers.Infrastructure.Data;

namespace Nimble.Modulith.Customers;

public static class CustomersModuleExtensions
{
    public static IHostApplicationBuilder AddCustomersModuleServices(this IHostApplicationBuilder builder,
        Serilog.ILogger logger)
    {
        builder.AddSqlServerDbContext<CustomersDbContext>("customersdb");
        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfReadRepository<>));
        builder.Services.AddScoped<Infrastructure.ICustomerAuthorizationService,
                Infrastructure.CustomerAuthorizationService>();
        return builder;
    }

    public static async Task<WebApplication> EnsureCustomersModuleDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
        await context.Database.MigrateAsync();
        return app;
    }
}