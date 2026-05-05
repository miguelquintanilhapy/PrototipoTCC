using System;
using System.Collections.Generic;
using System.Data;
using magal.Models;
using magal.Data;

namespace magal.Data.Repositories
{
    public class ClienteRepository
    {
        public List<Cliente> ListarTodos()
        {
            var lista = new List<Cliente>();
            try
            {
                using (var conn = DbConnectionFactory.CreateConnection())
                {
                    // Query : busca os nomes das colunas do banco
                    string query = @"SELECT id_cliente, 
                                            nome, 
                                            tipo, 
                                            cpf_cnpj, 
                                            cidade, 
                                            estado, 
                                            contato 
                                     FROM cliente";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Mapeando do banco
                                var cliente = new Cliente
                                {
                                    id_cliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),

                                    nome = reader.IsDBNull(reader.GetOrdinal("nome"))
                                           ? "" : reader.GetString(reader.GetOrdinal("nome")),

                                    tipo = reader.IsDBNull(reader.GetOrdinal("tipo"))
                                           ? "" : reader.GetString(reader.GetOrdinal("tipo")),

                                    cpf_cnpj = reader.IsDBNull(reader.GetOrdinal("cpf_cnpj"))
                                               ? "" : reader.GetString(reader.GetOrdinal("cpf_cnpj")),

                                    cidade = reader.IsDBNull(reader.GetOrdinal("cidade"))
                                             ? "" : reader.GetString(reader.GetOrdinal("cidade")),

                                    estado = reader.IsDBNull(reader.GetOrdinal("estado"))
                                             ? "" : reader.GetString(reader.GetOrdinal("estado")),

                                    contato = reader.IsDBNull(reader.GetOrdinal("contato"))
                                              ? "" : reader.GetString(reader.GetOrdinal("contato"))
                                };

                                lista.Add(cliente);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro no ClienteRepository: " + ex.Message);
            }
            return lista;
        }
    }
}