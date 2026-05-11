using System;
using System.Collections.Generic;
using System.Data;
using magal.Models;
using magal.Data;
using MySql.Data.MySqlClient;

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
                               f.custo_hora,
                               f.tipo_vinculo,
                               f.status,
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
                                    custo_hora = reader.GetDecimal(reader.GetOrdinal("custo_hora")),
                                    tipo_vinculo = reader.GetString(reader.GetOrdinal("tipo_vinculo")),
                                    status = reader.GetString(reader.GetOrdinal("status")),


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
        public void Excluir(int id_funcionario)
        {
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                string sql = "DELETE FROM funcionario WHERE id_funcionario = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id_funcionario);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Inserir(Funcionario funcionario)
        {
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO funcionario (id_cargo, nome, custo_hora, tipo_vinculo, status) 
                       VALUES (@id_cargo, @nome, @custo_hora, @tipo_vinculo, @status)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id_cargo", funcionario.id_cargo);
                    cmd.Parameters.AddWithValue("@nome", funcionario.nome);
                    cmd.Parameters.AddWithValue("@custo_hora", funcionario.custo_hora);
                    cmd.Parameters.AddWithValue("@tipo_vinculo", funcionario.tipo_vinculo);
                    cmd.Parameters.AddWithValue("@status", funcionario.status);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
    
}
