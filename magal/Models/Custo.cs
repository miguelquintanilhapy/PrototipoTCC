using System;

namespace magal.Models
{
    public class Custo : BaseModel
    {
        public int Id { get; set; } 
        public int ProjetoId { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; } 
        public decimal Valor { get; set; }
        public string Unidade { get; set; } 
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}