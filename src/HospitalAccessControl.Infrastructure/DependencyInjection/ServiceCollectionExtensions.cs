using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Diagnostics;
using HospitalAccessControl.Application.Patients;
using HospitalAccessControl.Infrastructure.Audit;
using HospitalAccessControl.Infrastructure.Data;
using HospitalAccessControl.Infrastructure.Diagnostics;
using HospitalAccessControl.Infrastructure.Patients;
using HospitalAccessControl.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<IPatientReadService, PatientReadService>();
        services.AddScoped<IMedicalRecordWriteService, MedicalRecordWriteService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAuditReadService, AuditReadService>();
        services.AddScoped<IUserRoleReadService, UserRoleReadService>();
        services.AddScoped<ICurrentUserAccessReadService, CurrentUserAccessReadService>();

        return services;
    }
}
