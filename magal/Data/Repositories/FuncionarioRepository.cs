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
                    string query = @"SELECT f.id_funcionario AS Id, 
                        f.nome AS Nome, 
                        f.id_cargo AS CargoId, 
                        c.nome AS CargoNome, 
                        c.custo_medio_hora AS ValorHora 
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
                                // Criamos o objeto Funcionário
                                var func = new Funcionario();

                                // Usamos GetOrdinal para pegar a posição da coluna pelo nome
                                func.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                func.Nome = reader.GetString(reader.GetOrdinal("Nome"));
                                func.CargoId = reader.GetInt32(reader.GetOrdinal("CargoId"));

                                // Criamos o objeto Cargo dentro do Funcionário
                                // Nota: Verifique se na sua classe Funcionario a propriedade se chama 'Cargo'
                                func.Cargo = new Cargo
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CargoId")),
                                    Nome = reader.GetString(reader.GetOrdinal("CargoNome")),
                                    CustoMedioHora = reader.GetDecimal(reader.GetOrdinal("ValorHora"))
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