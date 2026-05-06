using System;
using MySql.Data.MySqlClient;
using magal.Data;
using magal.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
                        if (projeto.id_projeto == 0)
                        {
                            // --- INSERIR NOVO PROJETO ---
                            using (var cmd = new MySqlCommand(@"INSERT INTO projeto (nome, id_cliente, id_usuario, data_criacao, tipo, status) 
                                                              VALUES (@nome, @idCliente, @idUsuario, @data, @tipo, @status);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@nome", projeto.nome);
                                cmd.Parameters.AddWithValue("@idCliente", projeto.id_cliente);
                                cmd.Parameters.AddWithValue("@idUsuario", projeto.id_usuario == 0 ? 1 : projeto.id_usuario);
                                cmd.Parameters.AddWithValue("@data", projeto.data_criacao == DateTime.MinValue ? DateTime.Now : projeto.data_criacao);
                                cmd.Parameters.AddWithValue("@tipo", projeto.tipo ?? "Serviço");
                                cmd.Parameters.AddWithValue("@status", projeto.status ?? "Rascunho");
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = "SELECT LAST_INSERT_ID();";
                                projeto.id_projeto = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                        else
                        {
                            // --- ATUALIZAR PROJETO EXISTENTE ---
                            using (var cmd = new MySqlCommand(@"UPDATE projeto SET nome=@nome, id_cliente=@idCliente, status=@status, tipo=@tipo 
                                                              WHERE id_projeto=@idProj", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@nome", projeto.nome);
                                cmd.Parameters.AddWithValue("@idCliente", projeto.id_cliente);
                                cmd.Parameters.AddWithValue("@status", projeto.status);
                                cmd.Parameters.AddWithValue("@tipo", projeto.tipo);
                                cmd.Parameters.AddWithValue("@idProj", projeto.id_projeto);
                                cmd.ExecuteNonQuery();
                            }

                            // Limpeza de tabelas filhas para reinserção (estratégia mais segura para edição)
                            new MySqlCommand($"DELETE FROM orcamento WHERE id_projeto={projeto.id_projeto}", conn, transaction).ExecuteNonQuery();
                            new MySqlCommand($"DELETE FROM tarefa WHERE id_projeto={projeto.id_projeto}", conn, transaction).ExecuteNonQuery();
                            new MySqlCommand($"DELETE FROM custo WHERE id_projeto={projeto.id_projeto}", conn, transaction).ExecuteNonQuery();
                        }

                        // --- INSERIR/REINSERIR ORÇAMENTO ---
                        using (var cmd = new MySqlCommand(@"INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, margem_percentual, valor_final) 
                                                          VALUES (@idProj, @custo, @imp, @marg, @final);", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@idProj", projeto.id_projeto);
                            cmd.Parameters.AddWithValue("@custo", projeto.Orcamento.custo_base);
                            cmd.Parameters.AddWithValue("@imp", projeto.Orcamento.percentual_impostos);
                            cmd.Parameters.AddWithValue("@marg", projeto.Orcamento.margem_percentual);
                            cmd.Parameters.AddWithValue("@final", projeto.Orcamento.valor_final);
                            cmd.ExecuteNonQuery();
                        }

                        // --- INSERIR/REINSERIR TAREFAS ---
                        foreach (var tarefa in projeto.Tarefas)
                        {
                            using (var cmd = new MySqlCommand(@"INSERT INTO tarefa (id_projeto, descricao, id_funcionario, horas_estimadas, status) 
                                                              VALUES (@idProj, @desc, @idFunc, @horas, @status);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idProj", projeto.id_projeto);
                                cmd.Parameters.AddWithValue("@desc", tarefa.descricao);
                                cmd.Parameters.AddWithValue("@idFunc", tarefa.id_funcionario);
                                // Correção de tipo: Convertendo para decimal para bater com a Model
                                cmd.Parameters.AddWithValue("@horas", Convert.ToDecimal(tarefa.horas_estimadas));
                                cmd.Parameters.AddWithValue("@status", tarefa.status ?? "Pendente");
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // --- INSERIR/REINSERIR CUSTOS (Usando o nome da lista da Model: 'custosExtras' vindo do parâmetro) ---
                        foreach (var custo in custosExtras)
                        {
                            using (var cmd = new MySqlCommand(@"INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) 
                                                              VALUES (@idProj, @nome, @cat, @tipo, @valor, @unidade);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idProj", projeto.id_projeto);
                                cmd.Parameters.AddWithValue("@nome", custo.nome);
                                cmd.Parameters.AddWithValue("@cat", custo.categoria);
                                cmd.Parameters.AddWithValue("@tipo", custo.tipo ?? "Direto");
                                cmd.Parameters.AddWithValue("@valor", custo.valor);
                                cmd.Parameters.AddWithValue("@unidade", custo.unidade ?? "Unitário");
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Erro ao processar transação no MySQL: " + ex.Message);
                    }
                }
            }
        }

        public Projeto CarregarProjetoCompleto(int idProjeto)
        {
            var projeto = new Projeto();
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();

                // 1. Dados Básicos e Orçamento
                string sqlProj = @"SELECT p.*, o.custo_base, o.percentual_impostos, o.margem_percentual, o.valor_final 
                                 FROM projeto p 
                                 LEFT JOIN orcamento o ON p.id_projeto = o.id_projeto 
                                 WHERE p.id_projeto = @id";

                using (var cmd = new MySqlCommand(sqlProj, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idProjeto);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            projeto.id_projeto = idProjeto;
                            projeto.nome = reader["nome"].ToString();
                            projeto.id_cliente = Convert.ToInt32(reader["id_cliente"]);
                            projeto.id_usuario = Convert.ToInt32(reader["id_usuario"]);
                            projeto.data_criacao = Convert.ToDateTime(reader["data_criacao"]);
                            projeto.status = reader["status"].ToString();
                            projeto.tipo = reader["tipo"].ToString();
                            projeto.Orcamento = new Orcamento
                            {
                                custo_base = Convert.ToDecimal(reader["custo_base"]),
                                percentual_impostos = Convert.ToDecimal(reader["percentual_impostos"]),
                                margem_percentual = Convert.ToDecimal(reader["margem_percentual"]),
                                valor_final = Convert.ToDecimal(reader["valor_final"])
                            };
                        }
                    }
                }

                // 2. Carregar Tarefas
                projeto.Tarefas = new ObservableCollection<Tarefa>();
                using (var cmd = new MySqlCommand("SELECT * FROM tarefa WHERE id_projeto = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", idProjeto);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projeto.Tarefas.Add(new Tarefa
                            {
                                id_tarefa = Convert.ToInt32(reader["id_tarefa"]),
                                descricao = reader["descricao"].ToString(), // <-- Verifique se esta linha existe
                                horas_estimadas = Convert.ToDecimal(reader["horas_estimadas"]), // <-- E esta
                                id_funcionario = Convert.ToInt32(reader["id_funcionario"]),
                                status = reader["status"].ToString()
                            });
                        }
                    }
                }

                // 3. Carregar Custos (Nome da propriedade na sua Model: Custos)
                projeto.Custos = new ObservableCollection<Custo>();
                using (var cmd = new MySqlCommand("SELECT * FROM custo WHERE id_projeto = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", idProjeto);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projeto.Custos.Add(new Custo
                            {
                                id_custo = Convert.ToInt32(reader["id_custo"]),
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
            return projeto;
        }

        public List<Projeto> BuscarTodosPorUsuario(int idUsuario)
        {
            var lista = new List<Projeto>();
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                // ADICIONADO: o.valor_final e o LEFT JOIN com a tabela orcamento
                string sql = @"SELECT p.*, c.nome as nome_cliente, o.valor_final 
                       FROM projeto p 
                       INNER JOIN cliente c ON p.id_cliente = c.id_cliente 
                       LEFT JOIN orcamento o ON p.id_projeto = o.id_projeto 
                       WHERE p.id_usuario = @idUser 
                       ORDER BY p.data_criacao DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@idUser", idUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var projeto = new Projeto
                            {
                                id_projeto = Convert.ToInt32(reader["id_projeto"]),
                                nome = reader["nome"].ToString(),
                                id_cliente = Convert.ToInt32(reader["id_cliente"]),
                                data_criacao = Convert.ToDateTime(reader["data_criacao"]),
                                status = reader["status"].ToString(),
                                tipo = reader["tipo"].ToString(),

                                // PREENCHENDO O OBJETO CLIENTE (para o filtro funcionar melhor)
                                Cliente = new Cliente { nome = reader["nome_cliente"].ToString() }
                            };

                            // PREENCHENDO O ORÇAMENTO (Para o Dashboard somar corretamente)
                            if (reader["valor_final"] != DBNull.Value)
                            {
                                projeto.Orcamento = new Orcamento
                                {
                                    valor_final = Convert.ToDecimal(reader["valor_final"])
                                };
                            }
                            else
                            {
                                projeto.Orcamento = new Orcamento { valor_final = 0 };
                            }

                            lista.Add(projeto);
                        }
                    }
                }
            }
            return lista;
        }

        public void ExcluirProjeto(int idProjeto)
        {
            using (var conn = (MySqlConnection)DbConnectionFactory.CreateConnection())
            {
                conn.Open();
                string sql = "DELETE FROM projeto WHERE id_projeto = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idProjeto);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}