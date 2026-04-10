using Microsoft.Win32;
using SAD.Helpers;
using SAD.Models;
using SAD.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SAD.ViewModels
{
    public class OrcamentoViewModel : INotifyPropertyChanged
    {
        private readonly IPdfService _pdfService;
        private string _cliente = string.Empty;
        private string _projeto = string.Empty;
        private double _overheadPercentual = 20;
        private double _lucroPercentual = 20;
        private double _impostosPercentual = 10;
        private string _statusMessage = "Pronto.";

        public OrcamentoViewModel(IPdfService pdfService)
        {
            _pdfService = pdfService;
            Itens = new ObservableCollection<ItemOrcamento>();
            Itens.CollectionChanged += OnItensChanged;

            AdicionarItemCommand = new RelayCommand(_ => AdicionarItem());
            RemoverItemCommand = new RelayCommand(i => RemoverItem(i as ItemOrcamento), i => i is ItemOrcamento);
            GerarPdfCommand = new RelayCommand(_ => GerarPdf(), _ => PodeGerar());
            LimparCommand = new RelayCommand(_ => Limpar());

            CarregarExemplos();
        }

        public string Cliente
        {
            get => _cliente;
            set { _cliente = value; OnPropertyChanged(); }
        }

        public string Projeto
        {
            get => _projeto;
            set { _projeto = value; OnPropertyChanged(); }
        }

        public double OverheadPercentual
        {
            get => _overheadPercentual;
            set { _overheadPercentual = Math.Max(0, value); OnPropertyChanged(); Recalcular(); }
        }

        public double LucroPercentual
        {
            get => _lucroPercentual;
            set { _lucroPercentual = Math.Max(0, value); OnPropertyChanged(); Recalcular(); }
        }

        public double ImpostosPercentual
        {
            get => _impostosPercentual;
            set { _impostosPercentual = Math.Max(0, value); OnPropertyChanged(); Recalcular(); }
        }

        public ObservableCollection<ItemOrcamento> Itens { get; }

        public decimal Subtotal => Itens.Sum(i => i.Total);
        public decimal ValorOverhead => Subtotal * (decimal)(OverheadPercentual / 100);
        public decimal ValorLucro => (Subtotal + ValorOverhead) * (decimal)(LucroPercentual / 100);
        public decimal ValorImpostos => (Subtotal + ValorOverhead + ValorLucro) * (decimal)(ImpostosPercentual / 100);
        public decimal TotalFinal => Subtotal + ValorOverhead + ValorLucro + ValorImpostos;

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand AdicionarItemCommand { get; }
        public ICommand RemoverItemCommand { get; }
        public ICommand GerarPdfCommand { get; }
        public ICommand LimparCommand { get; }

        private void AdicionarItem()
        {
            var item = new ItemOrcamento { Cargo = "Novo Cargo", Horas = 0, ValorPorHora = 0 };
            item.PropertyChanged += OnItemChanged;
            Itens.Add(item);
        }

        private void RemoverItem(ItemOrcamento? item)
        {
            if (item == null) return;
            item.PropertyChanged -= OnItemChanged;
            Itens.Remove(item);
            Recalcular();
        }

        private void GerarPdf()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"Proposta_{Cliente.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}"
            };
            if (dialog.ShowDialog() != true) return;

            try
            {
                _pdfService.GerarPdf(new Proposta
                {
                    Cliente = Cliente,
                    Projeto = Projeto,
                    DataGeracao = DateTime.Now,
                    Itens = Itens.ToList(),
                    Subtotal = Subtotal,
                    OverheadPercentual = OverheadPercentual,
                    LucroPercentual = LucroPercentual,
                    ImpostosPercentual = ImpostosPercentual,
                    ValorOverhead = ValorOverhead,
                    ValorLucro = ValorLucro,
                    ValorImpostos = ValorImpostos,
                    TotalFinal = TotalFinal
                }, dialog.FileName);

                StatusMessage = $"PDF gerado: {dialog.FileName}";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = dialog.FileName,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Limpar()
        {
            if (MessageBox.Show("Limpar tudo?", "Confirmar", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            foreach (var i in Itens) i.PropertyChanged -= OnItemChanged;
            Itens.Clear();
            Cliente = Projeto = string.Empty;
            OverheadPercentual = LucroPercentual = 20;
            ImpostosPercentual = 10;
            StatusMessage = "Dados limpos.";
        }

        private bool PodeGerar() => !string.IsNullOrWhiteSpace(Cliente) && !string.IsNullOrWhiteSpace(Projeto) && Itens.Count > 0;

        private void Recalcular()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(ValorOverhead));
            OnPropertyChanged(nameof(ValorLucro));
            OnPropertyChanged(nameof(ValorImpostos));
            OnPropertyChanged(nameof(TotalFinal));
        }

        private void OnItemChanged(object? sender, PropertyChangedEventArgs e) => Recalcular();

        private void OnItensChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null) foreach (ItemOrcamento i in e.NewItems) i.PropertyChanged += OnItemChanged;
            if (e.OldItems != null) foreach (ItemOrcamento i in e.OldItems) i.PropertyChanged -= OnItemChanged;
            Recalcular();
        }

        private void CarregarExemplos()
        {
            var exemplos = new[]
            {
                new ItemOrcamento { Cargo = "Gerente de Projeto",   Horas = 40,  ValorPorHora = 180 },
                new ItemOrcamento { Cargo = "Desenvolvedor Sênior", Horas = 120, ValorPorHora = 150 },
                new ItemOrcamento { Cargo = "Desenvolvedor Pleno",  Horas = 160, ValorPorHora = 100 },
                new ItemOrcamento { Cargo = "Desenvolvedor Júnior", Horas = 80,  ValorPorHora = 60  },
                new ItemOrcamento { Cargo = "UX/UI Designer",       Horas = 60,  ValorPorHora = 120 },
            };
            foreach (var i in exemplos) { i.PropertyChanged += OnItemChanged; Itens.Add(i); }
            Cliente = "Empresa Exemplo Ltda.";
            Projeto = "Sistema de Gestão v1.0";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}