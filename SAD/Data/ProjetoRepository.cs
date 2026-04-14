using MySql.Data.MySqlClient;
using MySqlConnector;
using SAD.Models;
using System;
using System.Collections.Generic;

namespace SAD.Data
{
    public class ProjetoRepository
    {
        public List<Projeto> BuscarTodos()
        {
            var lista = new List<Projeto>();
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT * FROM PROJETO ORDER BY data_criacao DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(MapearProjeto(reader));
            return lista;
        }

        public int Salvar(Projeto projeto)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(@"
                INSERT INTO PROJETO (id_usuario, id_cliente, nome, tipo, status, data_conclusao_prevista)
                VALUES (@uid, @cid, @nome, @tipo, @status, @data)", conn);

            cmd.Parameters.AddWithValue("@uid", projeto.IdUsuario);
            cmd.Parameters.AddWithValue("@cid", projeto.IdCliente);
            cmd.Parameters.AddWithValue("@nome", projeto.Nome);
            cmd.Parameters.AddWithValue("@tipo", projeto.Tipo);
            cmd.Parameters.AddWithValue("@status", projeto.Status);
            cmd.Parameters.AddWithValue("@data", projeto.DataConclusaoPrevista);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public void AtualizarStatus(int idProjeto, string novoStatus)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE PROJETO SET status = @status WHERE id_projeto = @id", conn);
            cmd.Parameters.AddWithValue("@status", novoStatus);
            cmd.Parameters.AddWithValue("@id", idProjeto);
            cmd.ExecuteNonQuery();
        }

        private static Projeto MapearProjeto(MySqlDataReader r) => new()
        {
            IdProjeto = r.GetInt32("id_projeto"),
            IdUsuario = r.GetInt32("id_usuario"),
            IdCliente = r.GetInt32("id_cliente"),
            Nome = r.GetString("nome"),
            Tipo = r.GetString("tipo"),
            Status = r.GetString("status"),
            DataCriacao = r.GetDateTime("data_criacao"),
            DataConclusaoPrevista = r.IsDBNull(r.GetOrdinal("data_conclusao_prevista"))
                                    ? null : r.GetDateTime("data_conclusao_prevista"),
        };
    }
}