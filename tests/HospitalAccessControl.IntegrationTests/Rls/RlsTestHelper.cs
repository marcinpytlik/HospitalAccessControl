using Microsoft.Data.SqlClient;

namespace HospitalAccessControl.IntegrationTests.Rls;

public static class RlsTestHelper
{
    public static async Task<int> CountVisiblePatientsAsync(
        string connectionString,
        string currentUser)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await SetCurrentUserAsync(connection, currentUser);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM medical.Patients;";

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public static async Task<bool> CanSeePatientAsync(
        string connectionString,
        string currentUser,
        int patientId)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await SetCurrentUserAsync(connection, currentUser);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM medical.Patients WHERE PatientId = @PatientId;";
        command.Parameters.AddWithValue("@PatientId", patientId);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) == 1;
    }

    private static async Task SetCurrentUserAsync(
        SqlConnection connection,
        string currentUser)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            EXEC sys.sp_set_session_context
                @key = N'CurrentUser',
                @value = @CurrentUser;
            """;
        command.Parameters.AddWithValue("@CurrentUser", currentUser);

        await command.ExecuteNonQueryAsync();
    }
}
