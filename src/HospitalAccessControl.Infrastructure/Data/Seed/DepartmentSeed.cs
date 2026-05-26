using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class DepartmentSeed
{
    public static IReadOnlyList<Department> Data => new List<Department>
    {
        new()
        {
            DepartmentId = 1,
            Code = "CARD",
            Name = "Kardiologia",
            Description = "Oddział kardiologiczny",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            DepartmentId = 2,
            Code = "ORTH",
            Name = "Ortopedia",
            Description = "Oddział ortopedyczny",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            DepartmentId = 3,
            Code = "NEUR",
            Name = "Neurologia",
            Description = "Oddział neurologiczny",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            DepartmentId = 4,
            Code = "EMER",
            Name = "Izba Przyjęć",
            Description = "Izba przyjęć i obsługa nagłych przypadków",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            DepartmentId = 5,
            Code = "PED",
            Name = "Pediatria",
            Description = "Oddział pediatryczny",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        }
    };
}