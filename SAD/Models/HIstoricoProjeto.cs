using System;

namespace SAD.Models
{
    public class HistoricoProjeto
    {
        public int IdHistorico { get; set; }
        public int IdProjeto { get; set; }
        public string TipoProjeto { get; set; } = string.Empty;
        public string Complexidade { get; set; } = string.Empty;
        public decimal CustoReal { get; set; }
        public decimal MargemReal { get; set; }
        public decimal DesvioPercentual { get; set; }
        public DateTime DataConclusao { get; set; }
        public string Observacoes { get; set; } = string.Empty;
    }
}