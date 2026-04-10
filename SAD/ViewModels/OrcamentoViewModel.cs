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
    /// <summary>
    /// ViewModel principal do SAD. Contém toda a lógica de negócio,
    /// cálculos e comandos. A View não contém nenhuma lógica.
    /// </summary>
    public class OrcamentoViewModel : INotifyPropertyChanged
    {
        // ── Dependências ──────────────────────────────────────────────────────
        private readonly IPdfService _pdfService;

        // ── Campos privados ──────────────────────────────────────────────────
        private string _cliente = string.Empty;
        private string _projeto = string.Empty;
        private double _overheadPercentual = 20;
        private double _lucroPercentual = 20;
        private double _impostosPercentual = 10;
        private string _statusMessage = "Pronto.";

        // ── Construtor ───────────────────────────────────────────────────────
        public OrcamentoViewModel(IPdfService pdfService)
        {
            _pdfService = pdfService;

            Itens = new ObservableCollection<ItemOrcamento>();
            Itens.CollectionChanged += OnItensCollectionChanged;

            // Dados de exemplo para facilitar o desenvolvimento/apresentação
            AdicionarItemExemplo();

            // Comandos
            AdicionarItemCommand = new RelayCommand(_ => AdicionarItem());
            RemoverItemCommand = new RelayCommand(item => RemoverItem(item as ItemOrcamento),
                                                  item => item is ItemOrcamento);
            GerarPdfCommand = new RelayCommand(_ => GerarPdf(), _ => PodeGerarPdf());
            LimparCommand = new RelayCommand(_ => Limpar());
        }

        // ── Propriedades de entrada ──────────────────────────────────────────
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
            set { _overheadPercentual = Math.Max(0, value); OnPropertyChanged(); RecalcularTotais(); }
        }

        public double LucroPercentual
        {
            get => _lucroPercentual;
            set { _lucroPercentual = Math.Max(0, value); OnPropertyChanged(); RecalcularTotais(); }
        }

        public double ImpostosPercentual
        {
            get => _impostosPercentual;
            set { _impostosPercentual = Math.Max(0, value); OnPropertyChanged(); RecalcularTotais(); }
        }

        // ── Coleção de itens ─────────────────────────────────────────────────
        public ObservableCollection<ItemOrcamento> Itens { get; }

        // ── Propriedades calculadas ──────────────────────────────────────────
        public decimal Subtotal => Itens.Sum(i => i.Total);

        public decimal ValorOverhead => Subtotal * (decimal)(OverheadPercentual / 100);

        public decimal ValorLucro => (Subtotal + ValorOverhead) * (decimal)(LucroPercentual / 100);

        public decimal ValorImpostos => (Subtotal + ValorOverhead + ValorLucro) * (decimal)(ImpostosPercentual / 100);

        public decimal TotalFinal => Subtotal + ValorOverhead + ValorLucro + ValorImpostos;

        // ── Status ───────────────────────────────────────────────────────────
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // ── Comandos ─────────────────────────────────────────────────────────
        public ICommand AdicionarItemCommand { get; }
        public ICommand RemoverItemCommand { get; }
        public ICommand GerarPdfCommand { get; }
        public ICommand LimparCommand { get; }

        // ── Lógica privada ───────────────────────────────────────────────────

        private void AdicionarItem()
        {
            var novoItem = new ItemOrcamento
            {
                Cargo = "Novo Cargo",
                Horas = 0,
                ValorPorHora = 0
            };

            // Assina o evento para recalcular quando qualquer propriedade mudar
            novoItem.PropertyChanged += OnItemPropertyChanged;
            Itens.Add(novoItem);
            StatusMessage = $"Item adicionado. Total de linhas: {Itens.Count}";
        }

        private void RemoverItem(ItemOrcamento item)
        {
            if (item == null) return;
            item.PropertyChanged -= OnItemPropertyChanged;
            Itens.Remove(item);
            RecalcularTotais();
            StatusMessage = $"Item removido. Total de linhas: {Itens.Count}";
        }

        private void GerarPdf()
        {
            if (!PodeGerarPdf())
            {
                MessageBox.Show("Preencha o nome do cliente e do projeto antes de gerar o PDF.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Salvar Proposta Comercial",
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"Proposta_{SanitizarNomeArquivo(Cliente)}_{DateTime.Now:yyyyMMdd}"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                var proposta = MontarProposta();
                _pdfService.GerarPdf(proposta, dialog.FileName);
                StatusMessage = $"PDF gerado com sucesso: {dialog.FileName}";

                // Abre automaticamente após gerar
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = dialog.FileName,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar PDF:\n{ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Erro ao gerar PDF.";
            }
        }

        private void Limpar()
        {
            var resultado = MessageBox.Show("Deseja limpar todos os dados?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes) return;

            // Desinscreve eventos antes de limpar
            foreach (var item in Itens)
                item.PropertyChanged -= OnItemPropertyChanged;

            Itens.Clear();
            Cliente = string.Empty;
            Projeto = string.Empty;
            OverheadPercentual = 20;
            LucroPercentual = 20;
            ImpostosPercentual = 10;
            StatusMessage = "Dados limpos. Pronto para nova proposta.";
        }

        private bool PodeGerarPdf()
            => !string.IsNullOrWhiteSpace(Cliente) && !string.IsNullOrWhiteSpace(Projeto) && Itens.Count > 0;

        private Proposta MontarProposta()
        {
            return new Proposta
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
            };
        }
        private void RecalcularTotais()
        {
            // Dispara atualização de todas as propriedades calculadas de uma vez
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(ValorOverhead));
            OnPropertyChanged(nameof(ValorLucro));
            OnPropertyChanged(nameof(ValorImpostos));
            OnPropertyChanged(nameof(TotalFinal));
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Qualquer mudança em qualquer item recalcula os totais
            if (e.PropertyName == nameof(ItemOrcamento.Total) ||
      e.PropertyName == nameof(ItemOrcamento.Horas) ||
      e.PropertyName == nameof(ItemOrcamento.ValorPorHora))
            {
                RecalcularTotais();
            }
        }
        private void OnItensCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Subscreve evento nos novos itens adicionados diretamente à coleção
            if (e.NewItems != null)
                foreach (ItemOrcamento item in e.NewItems)
                    item.PropertyChanged += OnItemPropertyChanged;

            if (e.OldItems != null)
                foreach (ItemOrcamento item in e.OldItems)
                    item.PropertyChanged -= OnItemPropertyChanged;

            RecalcularTotais();
        }

        private void AdicionarItemExemplo()
        {
            // Itens pré-carregados para demonstração
            var exemplos = new[]
            {
                new ItemOrcamento { Cargo = "Gerente de Projeto",   Horas = 40,  ValorPorHora = 180 },
                new ItemOrcamento { Cargo = "Desenvolvedor Sênior", Horas = 120, ValorPorHora = 150 },
                new ItemOrcamento { Cargo = "Desenvolvedor Pleno",  Horas = 160, ValorPorHora = 100 },
                new ItemOrcamento { Cargo = "Desenvolvedor Júnior", Horas = 80,  ValorPorHora = 60  },
                new ItemOrcamento { Cargo = "UX/UI Designer",       Horas = 60,  ValorPorHora = 120 },
            };

            foreach (var item in exemplos)
            {
                item.PropertyChanged += OnItemPropertyChanged;
                Itens.Add(item);
            }

            Cliente = "Empresa Exemplo Ltda.";
            Projeto = "Sistema de Gestão Interna v1.0";
        }

        private static string SanitizarNomeArquivo(string nome)
            => string.Concat(nome.Split(System.IO.Path.GetInvalidFileNameChars()))
                     .Replace(" ", "_");

        // ── INotifyPropertyChanged ───────────────────────────────────────────
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
