using System;
using System.Collections.Generic;
using System.Data;
using magal.Models;
using magal.Data;

namespace magal.Data.Repositories
{
    public class FuncionarioRepository
    {
        public List<Funcionario> ListarTodos()
        {
            var lista = new List<Funcionario>();
            try
            {
                using (var conn = DbConnectionFactory.CreateConnection())
                {
                    string query = @"
                        SELECT f.id_funcionario, 
                               f.nome, 
                               f.id_cargo, 
                               c.nome AS cargo_nome, 
                               c.custo_medio_hora 
                        FROM funcionario f 
                        INNER JOIN cargo c ON f.id_cargo = c.id_cargo";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var func = new Funcionario
                                {
                                    id_funcionario = reader.GetInt32(reader.GetOrdinal("id_funcionario")),
                                    nome = reader.GetString(reader.GetOrdinal("nome")),
                                    id_cargo = reader.GetInt32(reader.GetOrdinal("id_cargo")),

                                    // Preenche o objeto Cargo
                                    Cargo = new Cargo
                                    {
                                        id_cargo = reader.GetInt32(reader.GetOrdinal("id_cargo")),
                                        nome = reader.GetString(reader.GetOrdinal("cargo_nome")),
                                        custo_medio_hora = reader.GetDecimal(reader.GetOrdinal("custo_medio_hora"))
                                    }
                                };

                                lista.Add(func);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro no FuncionarioRepository: " + ex.Message);
            }
            return lista;
        }
    }
}