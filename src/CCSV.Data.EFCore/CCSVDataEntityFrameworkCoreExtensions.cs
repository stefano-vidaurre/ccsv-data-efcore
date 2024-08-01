using CCSV.Data.EFCore.Middlewares;
using CCSV.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace CCSV.Data.EFCore;

public static class CCSVDataEntityFrameworkCoreExtensions
{
    private static ValueConverter<DateTime, DateTime> DateTimeConverter = 
        new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    
    private static ValueConverter<DateTime?, DateTime?> NullableDateTimeConverter = 
        new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

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

    public static void UseUTCDateTimeConverter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            SetConverterToEntity(entityType);
        }
    }

    private static void SetConverterToEntity(IMutableEntityType entityType)
    {
        if (entityType.IsKeyless)
        {
            return;
        }

        foreach (var property in entityType.GetProperties())
        {
            SetConverterToProperty(property);
        }
    }

    private static void SetConverterToProperty(IMutableProperty property)
    {
        if (property.ClrType == typeof(DateTime))
        {
            property.SetValueConverter(DateTimeConverter);
        }
        else if (property.ClrType == typeof(DateTime?))
        {
            property.SetValueConverter(NullableDateTimeConverter);
        }
    }
}
