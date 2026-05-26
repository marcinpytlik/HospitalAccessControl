using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class ApplicationRoleSeed
{
    public static IReadOnlyList<ApplicationRole> Data => new List<ApplicationRole>
    {
        new()
        {
            ApplicationRoleId = 1,
            Code = "Doctor",
            Name = "Lekarz",
            Description = "Użytkownik medyczny odpowiedzialny za diagnozę i leczenie pacjentów",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 2,
            Code = "Nurse",
            Name = "Pielęgniarka",
            Description = "Użytkownik medyczny z ograniczonym dostępem do dokumentacji pacjenta",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 3,
            Code = "Registration",
            Name = "Rejestracja",
            Description = "Użytkownik odpowiedzialny za obsługę rejestracji pacjentów",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 4,
            Code = "DepartmentManager",
            Name = "Kierownik oddziału",
            Description = "Użytkownik zarządzający danym oddziałem",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 5,
            Code = "Auditor",
            Name = "Audytor",
            Description = "Użytkownik odpowiedzialny za kontrolę i analizę zdarzeń audytowych",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 6,
            Code = "ITAdministrator",
            Name = "Administrator IT",
            Description = "Administrator techniczny bez domyślnego dostępu biznesowego do danych medycznych",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        }
    };
}