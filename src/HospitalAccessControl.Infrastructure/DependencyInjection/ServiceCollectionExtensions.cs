using HospitalAccessControl.Infrastructure.Data;
using HospitalAccessControl.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HospitalAccessControl.Application.Diagnostics;
using HospitalAccessControl.Infrastructure.Diagnostics;

namespace HospitalAccessControl.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HospitalAccessControlDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'HospitalAccessControlDb' was not found.");
        }

        services.AddScoped<SessionContextConnectionInterceptor>();

        services.AddDbContext<HospitalAccessControlDbContext>((serviceProvider, options) =>
        {
            var sessionContextInterceptor =
                serviceProvider.GetRequiredService<SessionContextConnectionInterceptor>();

            options.UseSqlServer(connectionString);
            options.AddInterceptors(sessionContextInterceptor);
        });
services.AddScoped<ISqlSessionContextDiagnostics, SqlSessionContextDiagnostics>();
        return services;
    }
}