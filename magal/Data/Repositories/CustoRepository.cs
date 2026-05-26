using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using magal.Models;
using magal.Data;

namespace magal.Data.Repositories
{
    public class CustoRepository
    {
        // Este é o método que a sua CustoView vai chamar para listar na tabela da tela
        public List<CustoExibicaoDto> ListarPorProjeto(int idProjeto)
        {
            var lista = new List<CustoExibicaoDto>();

            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    // Query corrigida que junta as tabelas e traz os IDs preenchidos
                    string sql = @"
                        SELECT 
                            c.id_custo,
                            c.id_projeto,
                            c.id_catalogo_custo,
                            cc.nome,
                            cc.categoria,
                            cc.valor,
                            c.tipo,
                            c.unidade
                        FROM custo c
                        INNER JOIN catalogo_custo cc ON c.id_catalogo_custo = cc.id_catalogo_custo
                        WHERE c.id_projeto = @idProjeto
                        ORDER BY c.id_custo DESC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idProjeto", idProjeto);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new CustoExibicaoDto
                                {
                                    id_custo = reader.GetInt32(reader.GetOrdinal("id_custo")),
                                    id_projeto = reader.GetInt32(reader.GetOrdinal("id_projeto")),
                                    id_catalogo_custo = reader.GetInt32(reader.GetOrdinal("id_catalogo_custo")),
                                    nome = reader.GetString(reader.GetOrdinal("nome")),
                                    categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? string.Empty : reader.GetString(reader.GetOrdinal("categoria")),
                                    valor = reader.GetDecimal(reader.GetOrdinal("valor")),
                                    tipo = reader.IsDBNull(reader.GetOrdinal("tipo")) ? string.Empty : reader.GetString(reader.GetOrdinal("tipo")),
                                    unidade = reader.IsDBNull(reader.GetOrdinal("unidade")) ? string.Empty : reader.GetString(reader.GetOrdinal("unidade"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar os custos do projeto: " + ex.Message);
            }

            return lista;
        }

        // Método para quando o usuário adicionar um item do catálogo no projeto
        public void InserirCustoNoProjeto(int idProjeto, int idCatalogoCusto, string tipo, string unidade)
        {
            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO custo (id_projeto, id_catalogo_custo, tipo, unidade)
                        VALUES (@idProjeto, @idCatalogoCusto, @tipo, @unidade)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idProjeto", idProjeto);
                        cmd.Parameters.AddWithValue("@idCatalogoCusto", idCatalogoCusto);
                        cmd.Parameters.AddWithValue("@tipo", tipo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@unidade", unidade ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao vincular custo ao projeto: " + ex.Message);
            }
        }


        public List<Custo> ListarTodosDoCatalogo()
        {
            var lista = new List<Custo>();

            try
            {
                using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
                {
                    conn.Open();

                    // Buscamos direto da tabela que contém o cadastro base dos itens
                    string sql = "SELECT id_catalogo_custo, nome, categoria, valor FROM catalogo_custo ORDER BY nome ASC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Custo
                                {
                                    // Mapeia o ID do catálogo para sabermos qual item foi escolhido posteriormente
                                    id_custo = reader.GetInt32(reader.GetOrdinal("id_catalogo_custo")),
                                    nome = reader.GetString(reader.GetOrdinal("nome")),
                                    categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? string.Empty : reader.GetString(reader.GetOrdinal("categoria")),
                                    valor = reader.GetDecimal(reader.GetOrdinal("valor")),
                                    tipo = "Direto" // Valor padrão inicial
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar o catálogo de custos: " + ex.Message);
            }

            return lista;
        }

        public void ExcluirCustoDoProjeto(int idCusto)
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
                throw new Exception("Erro ao excluir custo do projeto: " + ex.Message);
            }
        }
    }
}