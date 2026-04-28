using magal.ViewModels;
using magal.Models;// Certifica-te de que o namespace do BaseModel está correto
using System;
using System.Collections.Generic;
using System.Linq;

namespace magal.Models
{
    public class Orcamento : BaseModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private decimal _margemPercentual;
        public decimal MargemPercentual
        {
            get => _margemPercentual;
            set
            {
                _margemPercentual = value;
                OnPropertyChanged();
                // Nota: O cálculo é disparado pela ViewModel ao ouvir este PropertyChanged
            }
        }

        private decimal _percentualImpostos;
        public decimal PercentualImpostos
        {
            get => _percentualImpostos;
            set
            {
                _percentualImpostos = value;
                OnPropertyChanged();
            }
        }

        private decimal _custoBase;
        public decimal CustoBase
        {
            get => _custoBase;
            set { _custoBase = value; OnPropertyChanged(); }
        }

        private decimal _valorFinal;
        public decimal ValorFinal
        {
            get => _valorFinal;
            set { _valorFinal = value; OnPropertyChanged(); }
        }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public void CalcularTotal(List<Tarefa> tarefas, List<Custo> custosExtras)
        {
            if (tarefas == null || custosExtras == null) return;

            decimal totalMaoDeObra = tarefas.Sum(t => t.CustoReal);
            decimal totalCustosExtras = custosExtras.Sum(c => c.Valor);
            decimal novoCustoBase = totalMaoDeObra + totalCustosExtras;
            decimal margem = novoCustoBase * (MargemPercentual / 100);
            decimal valorComMargem = novoCustoBase + margem;
            decimal valorImpostos = valorComMargem * (PercentualImpostos / 100);
            decimal novoValorFinal = valorComMargem + valorImpostos;

            {
                _custoBase = novoCustoBase;
                OnPropertyChanged(nameof(CustoBase));
            }

            if (_valorFinal != novoValorFinal)
            {
                _valorFinal = novoValorFinal;
                OnPropertyChanged(nameof(ValorFinal));
            }
        }
    
        public decimal ValorMargem => CustoBase * (MargemPercentual / 100);
        public decimal ValorImpostos => (CustoBase + ValorMargem) * (PercentualImpostos / 100);
    }
}