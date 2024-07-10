using CCSV.Data.EFCore.Middlewares;
using CCSV.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CCSV.Data.EFCore;

public static class CCSVDataEntityFrameworkCoreExtensions
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection services)
    {
        services.AddScoped<ITransaction, Transaction>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UnitOfWorkMiddleware>();
    }
}
