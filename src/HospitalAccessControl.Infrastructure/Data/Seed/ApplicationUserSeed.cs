using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class ApplicationUserSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<ApplicationUser> Data => new List<ApplicationUser>
    {
        new()
        {
            ApplicationUserId = 1,
            DomainLogin = @"HOSPITAL\doctor.cardio",
            SamAccountName = "doctor.cardio",
            DisplayName = "Jan Kardiolog",
            Email = "doctor.cardio@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 2,
            DomainLogin = @"HOSPITAL\doctor.ortho",
            SamAccountName = "doctor.ortho",
            DisplayName = "Marek Ortopeda",
            Email = "doctor.ortho@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 3,
            DomainLogin = @"HOSPITAL\doctor.neuro",
            SamAccountName = "doctor.neuro",
            DisplayName = "Anna Neurolog",
            Email = "doctor.neuro@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 4,
            DomainLogin = @"HOSPITAL\nurse.cardio",
            SamAccountName = "nurse.cardio",
            DisplayName = "Ewa Pielęgniarka Kardiologia",
            Email = "nurse.cardio@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 5,
            DomainLogin = @"HOSPITAL\nurse.ortho",
            SamAccountName = "nurse.ortho",
            DisplayName = "Katarzyna Pielęgniarka Ortopedia",
            Email = "nurse.ortho@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 6,
            DomainLogin = @"HOSPITAL\nurse.ped",
            SamAccountName = "nurse.ped",
            DisplayName = "Magdalena Pielęgniarka Pediatria",
            Email = "nurse.ped@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 7,
            DomainLogin = @"HOSPITAL\registration.user",
            SamAccountName = "registration.user",
            DisplayName = "Karolina Rejestracja",
            Email = "registration.user@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 8,
            DomainLogin = @"HOSPITAL\registration.emer",
            SamAccountName = "registration.emer",
            DisplayName = "Tomasz Rejestracja Izba Przyjęć",
            Email = "registration.emer@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 9,
            DomainLogin = @"HOSPITAL\manager.cardio",
            SamAccountName = "manager.cardio",
            DisplayName = "Piotr Kierownik Kardiologii",
            Email = "manager.cardio@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 10,
            DomainLogin = @"HOSPITAL\manager.ortho",
            SamAccountName = "manager.ortho",
            DisplayName = "Agnieszka Kierownik Ortopedii",
            Email = "manager.ortho@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 11,
            DomainLogin = @"HOSPITAL\auditor.user",
            SamAccountName = "auditor.user",
            DisplayName = "Alicja Audytor",
            Email = "auditor.user@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 12,
            DomainLogin = @"HOSPITAL\it.admin",
            SamAccountName = "it.admin",
            DisplayName = "Adam Administrator IT",
            Email = "it.admin@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        }
    };
}