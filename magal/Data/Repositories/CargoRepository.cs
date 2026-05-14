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

                    string sql = "SELECT id_cargo, nome, custo_medio_hora FROM cargo";

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
                                    custo_medio_hora = reader.GetDecimal(reader.GetOrdinal("custo_medio_hora"))
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

        public void Inserir(Cargo cargo)
        {
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO cargo (
                            nome,
                            custo_medio_hora
                        )
                        VALUES
                        (
                            @nome,
                            @custo
                        )";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", cargo.nome);
                        cmd.Parameters.AddWithValue("@custo", cargo.custo_medio_hora);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inserir cargo: " + ex.Message);
            }
        }

        public void Atualizar(Cargo cargo)
        {
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    string sql = @"
                        UPDATE cargo
                        SET
                            nome = @nome,
                            custo_medio_hora = @custo
                        WHERE id_cargo = @id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", cargo.id_cargo);
                        cmd.Parameters.AddWithValue("@nome", cargo.nome);
                        cmd.Parameters.AddWithValue("@custo", cargo.custo_medio_hora);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar cargo: " + ex.Message);
            }
        }

        public void Excluir(int idCargo)
        {
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    string sql = "DELETE FROM cargo WHERE id_cargo = @id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCargo);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir cargo: " + ex.Message);
            }
        }
    }
}