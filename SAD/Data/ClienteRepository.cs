using MySql.Data.MySqlClient;
using MySqlConnector;
using SAD.Models;
using System.Collections.Generic;

namespace SAD.Data
{
    public class ClienteRepository
    {
        public List<Cliente> BuscarTodos()
        {
            var lista = new List<Cliente>();
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT * FROM CLIENTE ORDER BY nome", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(MapearCliente(reader));
            return lista;
        }

        public int Salvar(Cliente cliente)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(@"
                INSERT INTO CLIENTE (nome, tipo, cpf_cnpj, cidade, estado, contato)
                VALUES (@nome, @tipo, @cpf, @cidade, @estado, @contato)", conn);

            cmd.Parameters.AddWithValue("@nome", cliente.Nome);
            cmd.Parameters.AddWithValue("@tipo", cliente.Tipo);
            cmd.Parameters.AddWithValue("@cpf", cliente.CpfCnpj);
            cmd.Parameters.AddWithValue("@cidade", cliente.Cidade);
            cmd.Parameters.AddWithValue("@estado", cliente.Estado);
            cmd.Parameters.AddWithValue("@contato", cliente.Contato);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        private static Cliente MapearCliente(MySqlDataReader r) => new()
        {
            IdCliente = r.GetInt32("id_cliente"),
            Nome = r.GetString("nome"),
            Tipo = r.GetString("tipo"),
            CpfCnpj = r.IsDBNull(r.GetOrdinal("cpf_cnpj")) ? "" : r.GetString("cpf_cnpj"),
            Cidade = r.IsDBNull(r.GetOrdinal("cidade")) ? "" : r.GetString("cidade"),
            Estado = r.IsDBNull(r.GetOrdinal("estado")) ? "" : r.GetString("estado"),
            Contato = r.IsDBNull(r.GetOrdinal("contato")) ? "" : r.GetString("contato"),
        };
    }
}