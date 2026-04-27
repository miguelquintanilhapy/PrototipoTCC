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
                    // Query atualizada com TODAS as colunas do seu DER
                    // Usamos o AS para converter o snake_case do banco para o PascalCase do C#
                    string query = @"SELECT id_cliente AS Id, 
                                            nome AS Nome, 
                                            tipo AS Tipo, 
                                            cpf_cnpj AS CpfCnpj, 
                                            cidade AS Cidade, 
                                            estado AS Estado, 
                                            contato AS Contato 
                                     FROM cliente";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cliente = new Cliente();

                                // Mapeamento seguro usando GetOrdinal
                                cliente.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                cliente.Nome = reader.IsDBNull(reader.GetOrdinal("Nome")) ? "" : reader.GetString(reader.GetOrdinal("Nome"));
                                cliente.Tipo = reader.IsDBNull(reader.GetOrdinal("Tipo")) ? "" : reader.GetString(reader.GetOrdinal("Tipo"));

                                // CPF/CNPJ (conforme seu novo Model)
                                int colCpfCnpj = reader.GetOrdinal("CpfCnpj");
                                cliente.CpfCnpj = reader.IsDBNull(colCpfCnpj) ? "" : reader.GetString(colCpfCnpj);

                                // Novos campos do DER
                                int colCidade = reader.GetOrdinal("Cidade");
                                cliente.Cidade = reader.IsDBNull(colCidade) ? "" : reader.GetString(colCidade);

                                int colEstado = reader.GetOrdinal("Estado");
                                cliente.Estado = reader.IsDBNull(colEstado) ? "" : reader.GetString(colEstado);

                                int colContato = reader.GetOrdinal("Contato");
                                cliente.Contato = reader.IsDBNull(colContato) ? "" : reader.GetString(colContato);

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