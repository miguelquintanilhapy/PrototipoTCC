using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using magal.Models;
using magal.Data;

namespace magal.Data.Repositories
{
    public class CargoRepository
    {
        public List<Cargo> ListarTodos()
        {
            var lista = new List<Cargo>();
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();
                    string sql = "SELECT id_cargo, nome, nivel, custo_medio_hora, descricao FROM cargo";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Cargo
                                {
                                    id_cargo = reader.GetInt32(reader.GetOrdinal("id_cargo")),
                                    nome = reader.GetString(reader.GetOrdinal("nome")),
                                    nivel = reader.GetString(reader.GetOrdinal("nivel")),
                                    custo_medio_hora = reader.GetDecimal(reader.GetOrdinal("custo_medio_hora")),
                                    descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? "" : reader.GetString(reader.GetOrdinal("descricao"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro no CargoRepository: " + ex.Message);
            }
            return lista;
        }
    }
}