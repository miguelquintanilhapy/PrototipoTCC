using System;

namespace magal.Models
{
    public class CatalogoCusto : BaseModel
    {
        public int id_catalogo_custo { get; set; }
        public string nome { get; set; }
        public string categoria { get; set; }

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
    }
}