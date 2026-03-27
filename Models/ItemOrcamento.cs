using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SAD.Models
{
    /// <summary>
    /// Representa uma linha da tabela de orçamento (cargo + horas + valor/hora).
    /// Implementa INotifyPropertyChanged para reatividade total com o DataGrid.
    /// </summary>
    public class ItemOrcamento : INotifyPropertyChanged
    {
        private string _cargo = string.Empty;
        private double _horas;
        private decimal _valorPorHora;

        public string Cargo
        {
            get => _cargo;
            set { _cargo = value; OnPropertyChanged(); }
        }

        public double Horas
        {
            get => _horas;
            set { _horas = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
        }

        public decimal ValorPorHora
        {
            get => _valorPorHora;
            set { _valorPorHora = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
        }

        // Calculado automaticamente — sem setter público
        public decimal Total => (decimal)Horas * ValorPorHora;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
