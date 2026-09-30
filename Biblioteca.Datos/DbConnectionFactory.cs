using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Datos
{
    public static class DbConnectionFactory
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["BibliotecaDB"]?.ConnectionString 
                ?? "Data Source=.;Initial Catalog=BibliotecaDB;Integrated Security=True;TrustServerCertificate=True";
        }

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
    }
}
