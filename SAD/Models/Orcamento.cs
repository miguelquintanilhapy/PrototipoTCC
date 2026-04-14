using System;

namespace SAD.Models
{
    public class Orcamento
    {
        public int IdOrcamento { get; set; }
        public int IdProjeto { get; set; }
        public decimal CustoBase { get; set; }
        public decimal PercentualImpostos { get; set; }
        public decimal ValorImpostos { get; set; }
        public decimal MargemPercentual { get; set; }
        public decimal ValorFinal { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}