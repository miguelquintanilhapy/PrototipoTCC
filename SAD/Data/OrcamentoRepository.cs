using MySql.Data.MySqlClient;
using MySqlConnector;
using SAD.Models;

namespace SAD.Data
{
    public class OrcamentoRepository
    {
        public int Salvar(Orcamento orcamento)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(@"
                INSERT INTO ORCAMENTO 
                    (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_final)
                VALUES 
                    (@pid, @base, @pct, @vimp, @margem, @final)", conn);

            cmd.Parameters.AddWithValue("@pid", orcamento.IdProjeto);
            cmd.Parameters.AddWithValue("@base", orcamento.CustoBase);
            cmd.Parameters.AddWithValue("@pct", orcamento.PercentualImpostos);
            cmd.Parameters.AddWithValue("@vimp", orcamento.ValorImpostos);
            cmd.Parameters.AddWithValue("@margem", orcamento.MargemPercentual);
            cmd.Parameters.AddWithValue("@final", orcamento.ValorFinal);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public Orcamento? BuscarPorProjeto(int idProjeto)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT * FROM ORCAMENTO WHERE id_projeto = @id", conn);
            cmd.Parameters.AddWithValue("@id", idProjeto);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Orcamento
            {
                IdOrcamento = reader.GetInt32("id_orcamento"),
                IdProjeto = reader.GetInt32("id_projeto"),
                CustoBase = reader.GetDecimal("custo_base"),
                PercentualImpostos = reader.GetDecimal("percentual_impostos"),
                ValorImpostos = reader.GetDecimal("valor_impostos"),
                MargemPercentual = reader.GetDecimal("margem_percentual"),
                ValorFinal = reader.GetDecimal("valor_final"),
                DataCriacao = reader.GetDateTime("data_criacao"),
            };
        }
    }
}