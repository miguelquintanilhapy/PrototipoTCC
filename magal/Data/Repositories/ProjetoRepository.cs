using System;
using MySql.Data.MySqlClient;
using magal.Data;
using magal.Models;
using System.Collections.Generic;

namespace magal.Data.Repositories
{
    public class ProjetoRepository
    {
        public void SalvarProjetoCompleto(Projeto projeto, List<Custo> custosExtras)
        {
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //INSERIR O PROJETO
                        using (var cmd = new MySqlCommand(@"INSERT INTO projeto (nome, id_cliente, id_usuario, data_criacao) 
                                                          VALUES (@nome, @idCliente, @idUsuario, @data);", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@nome", projeto.Nome);
                            cmd.Parameters.AddWithValue("@idCliente", projeto.Cliente?.Id ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@idUsuario", 1); 
                            cmd.Parameters.AddWithValue("@data", DateTime.Now);

                            cmd.ExecuteNonQuery();

                            // Recupera o ID gerado para as tabelas filhas
                            cmd.CommandText = "SELECT LAST_INSERT_ID();";
                            projeto.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        //INSERIR O ORÇAMENTO
                        using (var cmd = new MySqlCommand(@"INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, margem_percentual, valor_final) 
                                                          VALUES (@idProj, @custo, @imp, @marg, @final);", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@idProj", projeto.Id);
                            cmd.Parameters.AddWithValue("@custo", projeto.Orcamento.CustoBase);
                            cmd.Parameters.AddWithValue("@imp", projeto.Orcamento.PercentualImpostos);
                            cmd.Parameters.AddWithValue("@marg", projeto.Orcamento.MargemPercentual);
                            cmd.Parameters.AddWithValue("@final", projeto.Orcamento.ValorFinal);
                            cmd.ExecuteNonQuery();
                        }

                        //INSERIR AS TAREFAS (Mão de Obra)
                        foreach (var tarefa in projeto.Tarefas)
                        {
                            using (var cmd = new MySqlCommand(@"INSERT INTO tarefa (id_projeto, descricao, id_funcionario, horas_estimadas, status) 
                                                              VALUES (@idProj, @desc, @idFunc, @horas, @status);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idProj", projeto.Id);
                                cmd.Parameters.AddWithValue("@desc", tarefa.Descricao);
                                cmd.Parameters.AddWithValue("@idFunc", tarefa.Funcionario?.Id ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@horas", tarefa.HorasEstimadas);
                                cmd.Parameters.AddWithValue("@status", tarefa.Status);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        //INSERIR OS CUSTOS EXTRAS 
                        foreach (var custo in custosExtras)
                        {
                            using (var cmd = new MySqlCommand(@"INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) 
                                                              VALUES (@idProj, @nome, @cat, @tipo, @valor, @unidade);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idProj", projeto.Id);
                                cmd.Parameters.AddWithValue("@nome", custo.Nome);
                                cmd.Parameters.AddWithValue("@cat", custo.Categoria);
                                cmd.Parameters.AddWithValue("@tipo", custo.Tipo ?? "Direto");
                                cmd.Parameters.AddWithValue("@valor", custo.Valor);
                                cmd.Parameters.AddWithValue("@unidade", custo.Unidade ?? "Unid");
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Erro ao salvar no MySQL: " + ex.Message);
                    }
                }
            }
        }
    }
}