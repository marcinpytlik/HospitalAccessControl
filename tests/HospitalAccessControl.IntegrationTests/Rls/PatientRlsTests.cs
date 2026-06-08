using FluentAssertions;

namespace HospitalAccessControl.IntegrationTests.Rls;

public sealed class PatientRlsTests
{
    private const string ConnectionString =
        "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;";

    [Theory]
    [InlineData(@"HOSPITAL\doctor.cardio", 10)]
    [InlineData(@"HOSPITAL\doctor.ortho", 10)]
    [InlineData(@"HOSPITAL\doctor.neuro", 10)]
    [InlineData(@"HOSPITAL\nurse.ped", 5)]
    [InlineData(@"HOSPITAL\it.admin", 0)]
    [InlineData(@"HOSPITAL\unknown.user", 0)]
    public async Task Should_filter_patients_by_current_user(
        string currentUser,
        int expectedCount)
    {
        var count = await RlsTestHelper.CountVisiblePatientsAsync(
            ConnectionString,
            currentUser);

        count.Should().Be(expectedCount);
    }

    [Theory]
    [InlineData(@"HOSPITAL\doctor.cardio", 1, true)]
    [InlineData(@"HOSPITAL\doctor.cardio", 11, false)]
    [InlineData(@"HOSPITAL\doctor.ortho", 11, true)]
    [InlineData(@"HOSPITAL\doctor.ortho", 1, false)]
    [InlineData(@"HOSPITAL\it.admin", 1, false)]
    public async Task Should_filter_patient_details(
        string currentUser,
        int patientId,
        bool expected)
    {
        var canSee = await RlsTestHelper.CanSeePatientAsync(
            ConnectionString,
            currentUser,
            patientId);

        canSee.Should().Be(expected);
    }
}
