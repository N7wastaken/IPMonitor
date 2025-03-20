using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

public static class MySqlHelper
{
    private static string connectionString;

    // Inicjalizacja connection string
    static MySqlHelper()
    {
        connectionString = "server=ma-mf10-dev;database=ipm_dev;user=ipm_user;password=I<3database:D";
    }

    // Asynchroniczne pobranie połączenia
    public static async Task<MySqlConnection> GetConnectionAsync()
    {
        var connection = new MySqlConnection(connectionString);
        try
        {
            await connection.OpenAsync(); // Asynchroniczne otwarcie połączenia
            return connection;
        }
        catch (Exception ex)
        {
            // Logowanie błędu
            Console.WriteLine($"Błąd podczas otwierania połączenia z bazą danych: {ex.Message}");
            throw; // Rzucamy wyjątek dalej, aby można było go przechwycić w wyższej warstwie
        }
    }
}