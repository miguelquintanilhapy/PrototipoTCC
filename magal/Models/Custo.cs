using System;

namespace magal.Models
{
    public class Custo : BaseModel
    {
        public int id_custo { get; set; }
        public int id_projeto { get; set; }
        public string nome { get; set; }
        public string categoria { get; set; }
        public string tipo { get; set; }     //"Direto/Indireto"
        public decimal valor { get; set; }
        public string unidade { get; set; } //"Unitário/Hora/Dia/Mês"
        public DateTime data_cadastro { get; set; } = DateTime.Now;
    }
}