using System;

namespace magal.Models
{
    public class HistoricoProjeto : BaseModel
    {
        public int Id { get; set; } 
        public int ProjetoId { get; set; } 
        public string TipoProjeto { get; set; }
        public string Complexidade { get; set; }
        public decimal CustoReal { get; set; }
        public decimal MargemEstimada { get; set; }
        public decimal MargemReal { get; set; }
        public decimal DesvioPercentual { get; set; }
        public DateTime DataConclusao { get; set; }
        public string Observacoes { get; set; }
    }
}