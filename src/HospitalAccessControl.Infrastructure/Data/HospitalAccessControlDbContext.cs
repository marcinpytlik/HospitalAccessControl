using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Data;

public sealed class HospitalAccessControlDbContext : DbContext
{
    public HospitalAccessControlDbContext(DbContextOptions<HospitalAccessControlDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    public DbSet<UserDepartmentAccess> UserDepartmentAccesses => Set<UserDepartmentAccess>();

    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();

    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalAccessControlDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}