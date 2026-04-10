using System;
using System.Collections.Generic;

namespace SAD.Models
{
    /// <summary>
    /// Modelo de dados da proposta. Usado exclusivamente para transferência de dados
    /// entre ViewModel e Services (ex: PdfService). Sem lógica de UI.
    /// </summary>
    public class Proposta
    {
        public string Cliente { get; set; } = string.Empty;
        public string Projeto { get; set; } = string.Empty;
        public DateTime DataGeracao { get; set; } = DateTime.Now;

        public List<ItemOrcamento> Itens { get; set; } = new List<ItemOrcamento>();

        public decimal Subtotal { get; set; }
        public double OverheadPercentual { get; set; }
        public double LucroPercentual { get; set; }
        public double ImpostosPercentual { get; set; }

        public decimal ValorOverhead { get; set; }
        public decimal ValorLucro { get; set; }
        public decimal ValorImpostos { get; set; }
        public decimal TotalFinal { get; set; }
    }
}
