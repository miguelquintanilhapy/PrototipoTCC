using System;

namespace magal.Models
{
    public class Custo : BaseModel
    {
        public int Id { get; set; } // id_custo
        public int ProjetoId { get; set; } // id_projeto (FK)
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; } // "Direto/Indireto"
        public decimal Valor { get; set; }
        public string Unidade { get; set; } // "Unitário/Hora/Dia/Mês"
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}