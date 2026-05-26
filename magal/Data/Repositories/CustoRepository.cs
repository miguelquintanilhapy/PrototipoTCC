using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using magal.Models;
using magal.Data;

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
                    conn.Open();

                    // Ordenado por id_custo DESC como estava na sua query original
                    string sql = "SELECT id_custo, id_projeto, id_catalogo_custo, nome, categoria, tipo, valor, data_cadastro FROM custo ORDER BY id_custo DESC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Custo
                                {
                                    id_custo = reader.GetInt32(reader.GetOrdinal("id_custo")),
                                    id_projeto = reader.GetInt32(reader.GetOrdinal("id_projeto")),
                                    nome = reader.GetString(reader.GetOrdinal("nome")),
                                    categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? string.Empty : reader.GetString(reader.GetOrdinal("categoria")),
                                    tipo = reader.IsDBNull(reader.GetOrdinal("tipo")) ? string.Empty : reader.GetString(reader.GetOrdinal("tipo")),
                                    valor = reader.GetDecimal(reader.GetOrdinal("valor")),
                                    data_cadastro = reader.GetDateTime(reader.GetOrdinal("data_cadastro"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro no CustoRepository ao listar: " + ex.Message);
            }

            return lista;
        }

        public void Inserir(Custo custo)
        {
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO custo (
                            id_projeto,
                            id_catalogo_custo,
                            nome,
                            categoria,
                            tipo,
                            valor
                        )
                        VALUES
                        (
                            @id_projeto,
                            @id_catalogo_custo,
                            @nome,
                            @categoria,
                            @tipo,
                            @valor
                        )";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_projeto", custo.id_projeto);

                        // Tratamento para chave estrangeira opcional do catálogo
                        cmd.Parameters.AddWithValue("@id_catalogo_custo", DBNull.Value); // Pode ser alterado caso use vínculo com catálogo posteriormente

                        cmd.Parameters.AddWithValue("@nome", custo.nome);
                        cmd.Parameters.AddWithValue("@categoria", custo.categoria ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@tipo", custo.tipo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@valor", custo.valor);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inserir custo: " + ex.Message);
            }
        }

        public void Atualizar(Custo custo)
        {
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    string sql = @"
                        UPDATE custo
                        SET
                            id_projeto = @id_projeto,
                            nome = @nome,
                            categoria = @categoria,
                            tipo = @tipo,
                            valor = @valor
                        WHERE id_custo = @id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", custo.id_custo);
                        cmd.Parameters.AddWithValue("@id_projeto", custo.id_projeto);
                        cmd.Parameters.AddWithValue("@nome", custo.nome);
                        cmd.Parameters.AddWithValue("@categoria", custo.categoria ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@tipo", custo.tipo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@valor", custo.valor);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar custo: " + ex.Message);
            }
        }

        public void Excluir(int idCusto)
        {
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    string sql = "DELETE FROM custo WHERE id_custo = @id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCusto);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir custo: " + ex.Message);
            }
        }
    }
}