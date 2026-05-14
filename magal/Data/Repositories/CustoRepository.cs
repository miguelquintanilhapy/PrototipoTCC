using System;
using System.Collections.Generic;
using magal.Models;
using magal.Data;
using MySql.Data.MySqlClient;

namespace magal.Data.Repositories
{
    public class CustoRepository
    {
        public List<Custo> ListarTodos()
        {
            var lista = new List<Custo>();
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    // Busca todos os custos, independente do projeto
                    string query = "SELECT * FROM custo ORDER BY id_custo DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Custo
                                {
                                    id_custo = Convert.ToInt32(reader["id_custo"]),
                                    id_projeto = reader["id_projeto"] != DBNull.Value ? Convert.ToInt32(reader["id_projeto"]) : 0,
                                    nome = reader["nome"].ToString(),
                                    categoria = reader["categoria"].ToString(),
                                    tipo = reader["tipo"].ToString(),
                                    valor = Convert.ToDecimal(reader["valor"]),
                                    unidade = reader["unidade"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar custos: " + ex.Message);
            }
            return lista;
        }
    }
}