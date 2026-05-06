using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace magal.Data
{
    public static class DbConnectionFactory
    {
        public static IDbConnection CreateConnection()
        {
            // Busca o appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            return new MySqlConnection(connectionString);
        }
    }
}