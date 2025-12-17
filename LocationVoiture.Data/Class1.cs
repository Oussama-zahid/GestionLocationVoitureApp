using System.Data.SqlClient;

namespace LocationVoiture.Data
{
    public static class Database
    {
        // Changez le Server=... selon votre SQL Server local
        private static string connectionString = "Server=LOCALHOST; Database=GestionLocationVoiture; Trusted_Connection=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}