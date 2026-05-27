using System;

namespace magal.Models
{
    public class Custo : BaseModel
    {
        public int id_custo { get; set; }
        public int id_projeto { get; set; }
        public int id_catalogo_custo { get; set; } 
        public string nome { get; set; }
        public string categoria { get; set; }
        public string tipo { get; set; }

        private decimal _valor;
        public decimal valor
        {
            get => _valor;
            set
            {
                if (_valor != value)
                {
                    _valor = value;
                    OnPropertyChanged(nameof(valor));
                }
            }
        }

        public string unidade { get; set; }
        public DateTime data_cadastro { get; set; } = DateTime.Now;
    }
}