using magal.ViewModels;
using magal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace magal.Models
{
    public class Orcamento : BaseModel
    {
        private int _id_orcamento;
        public int id_orcamento
        {
            get => _id_orcamento;
            set { _id_orcamento = value; OnPropertyChanged(); }
        }

        private decimal _margem_percentual;
        public decimal margem_percentual
        {
            get => _margem_percentual;
            set
            {
                _margem_percentual = value;
                OnPropertyChanged();
                NotificarMudancasCalculadas();
            }
        }

        private decimal _percentual_impostos;
        public decimal percentual_impostos
        {
            get => _percentual_impostos;
            set
            {
                _percentual_impostos = value;
                OnPropertyChanged();
                NotificarMudancasCalculadas();
            }
        }

        private decimal _custo_base;
        public decimal custo_base
        {
            get => _custo_base;
            set
            {
                _custo_base = value;
                OnPropertyChanged();
                NotificarMudancasCalculadas();
            }
        }

        private decimal? _valor_margem_manual;
        public decimal valor_margem
        {
            get => _valor_margem_manual ?? (custo_base * (margem_percentual / 100));
            set { _valor_margem_manual = value; OnPropertyChanged(); }
        }

        private decimal? _valor_impostos_manual;
        public decimal valor_impostos
        {
            get => _valor_impostos_manual ?? ((custo_base + valor_margem) * (percentual_impostos / 100));
            set { _valor_impostos_manual = value; OnPropertyChanged(); }
        }

        private decimal? _valor_final_manual;
        public decimal valor_final
        {
            get
            {
                // se o custo base for maior que zero, ele tenta calcular o total atualizado
                // se for zero (como na lista de histórico), ele usa o valor salvo no banco
                decimal calculado = custo_base + valor_margem + valor_impostos;
                return (calculado > 0) ? calculado : (_valor_final_manual ?? 0);
            }
            set
            {
                _valor_final_manual = value;
                OnPropertyChanged();
            }
        }

        public DateTime data_criacao { get; set; } = DateTime.Now;

        private void NotificarMudancasCalculadas()
        {
            _valor_margem_manual = null;
            _valor_impostos_manual = null;
            _valor_final_manual = null;

            OnPropertyChanged(nameof(valor_margem));
            OnPropertyChanged(nameof(valor_impostos));
            OnPropertyChanged(nameof(valor_final));
        }

        public void CalcularTotal(List<Tarefa> tarefas, List<Custo> custosExtras)
        {
            decimal totalMaoDeObra = tarefas?.Sum(t => t.custo_real) ?? 0;
            decimal totalCustosExtras = custosExtras?.Sum(c => c.valor) ?? 0;

            // dispara o setter de custo_base, que limpa os manuais e recalcula tudo
            custo_base = totalMaoDeObra + totalCustosExtras;
        }
    }
}