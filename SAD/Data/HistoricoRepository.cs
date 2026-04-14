using MySql.Data.MySqlClient;
using MySqlConnector;
using SAD.Models;
using System;

namespace SAD.Data
{
    /// <summary>
    /// Só cria histórico quando status do projeto = "Concluído".
    /// </summary>
    public class HistoricoRepository
    {
        public void CriarSeNecessario(int idProjeto, HistoricoProjeto historico)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            // Verifica se o projeto está Concluído
            using var check = new MySqlCommand(
                "SELECT status FROM PROJETO WHERE id_projeto = @id", conn);
            check.Parameters.AddWithValue("@id", idProjeto);
            var status = check.ExecuteScalar()?.ToString();

            if (status != "Concluído")
                throw new InvalidOperationException(
                    "Histórico só pode ser criado para projetos com status 'Concluído'.");

            using var cmd = new MySqlCommand(@"
                INSERT INTO HISTORICO_PROJETO
                    (id_projeto, tipo_projeto, complexidade, custo_real, margem_real, desvio_percentual, data_conclusao, observacoes)
                VALUES
                    (@pid, @tipo, @complex, @custo, @margem, @desvio, @data, @obs)", conn);

            cmd.Parameters.AddWithValue("@pid", historico.IdProjeto);
            cmd.Parameters.AddWithValue("@tipo", historico.TipoProjeto);
            cmd.Parameters.AddWithValue("@complex", historico.Complexidade);
            cmd.Parameters.AddWithValue("@custo", historico.CustoReal);
            cmd.Parameters.AddWithValue("@margem", historico.MargemReal);
            cmd.Parameters.AddWithValue("@desvio", historico.DesvioPercentual);
            cmd.Parameters.AddWithValue("@data", historico.DataConclusao);
            cmd.Parameters.AddWithValue("@obs", historico.Observacoes);
            cmd.ExecuteNonQuery();
        }
    }
}