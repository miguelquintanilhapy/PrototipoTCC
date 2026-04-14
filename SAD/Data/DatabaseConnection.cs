using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAD.Data
{
    public static class DatabaseConnection
    {
        private const string Server = "localhost";
        private const string Database = "sad_precificação";
        private const string User = "root";
        private const string Password = "miguel1001";

        public static string ConnectionString =>
            $"Server={Server};Database={Database};Uid={User};Pwd={Password};CharSet=utf8mb4;";

        public static MySqlConnection GetConnection()
            => new MySqlConnection(ConnectionString);
    }
}