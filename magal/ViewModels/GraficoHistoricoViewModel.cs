using LiveCharts;
using LiveCharts.Wpf;
using magal.Data.Repositories;
using magal.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace magal.ViewModels
{
    /// <summary>
    /// ViewModel responsável por gerenciar a exibição de gráficos analíticos do histórico de projetos.
    /// </summary>
    public class GraficoHistoricoViewModel : BaseModel
    {
        #region Campos Privados

        private readonly ProjetoRepository _repository;

        private string _totalFinanceiro;
        private string _totalLucro;
        private int _quantidadeProjetos;

        #endregion

        #region Cultura

        private static readonly CultureInfo _ptBR = new("pt-BR");

        #endregion

        #region Indicadores

        public string TotalFinanceiro
        {
            get => _totalFinanceiro;
            set
            {
                _totalFinanceiro = value;
                OnPropertyChanged();
            }
        }

        public string TotalLucro
        {
            get => _totalLucro;
            set
            {
                _totalLucro = value;
                OnPropertyChanged();
            }
        }

        public int QuantidadeProjetos
        {
            get => _quantidadeProjetos;
            set
            {
                _quantidadeProjetos = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Gráficos Pizza

        public SeriesCollection SeriesStatus { get; set; } = new();

        public SeriesCollection SeriesTipo { get; set; } = new();

        #endregion

        #region Gráfico Coluna

        public ChartValues<double> ValoresLucroMensal { get; set; } = new();

        public ChartValues<double> ValoresFaturamentoMensal { get; set; } = new();
        public Func<double, string> Formatter { get; set; } =
            value => value.ToString("C0", CultureInfo.GetCultureInfo("pt-BR"));

        public string[] LabelsMeses { get; set; } = Array.Empty<string>();


        #endregion

        #region Comandos

        public RelayCommand AtualizarCommand { get; }

        #endregion

        #region Construtor

        public GraficoHistoricoViewModel()
        {
            _repository = new ProjetoRepository();

            AtualizarCommand = new RelayCommand(_ => CarregarDados());

            Formatter = value => value.ToString("C0", _ptBR);

            CarregarDados();
        }

        #endregion

        #region Métodos

        public void CarregarDados()
        {
            try
            {
                var projetos = _repository.BuscarTodosPorUsuario(1);

                // Evita lista nula
                if (projetos == null)
                {
                    projetos = new List<Projeto>();
                }

                // Corrige objetos nulos
                foreach (var projeto in projetos)
                {
                    if (projeto.Orcamento == null)
                    {
                        projeto.Orcamento = new Orcamento();
                    }

                    // evita data padrão/nula
                    if (projeto.data_criacao == DateTime.MinValue)
                    {
                        projeto.data_criacao = DateTime.Now;
                    }
                }

                AtualizarIndicadores(projetos);
                AtualizarGraficoMensal(projetos);
                AtualizarGraficoStatus(projetos);
                AtualizarGraficoTipo(projetos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar gráficos:\n\n{ex}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void AtualizarIndicadores(List<Projeto> projetos)
        {
            decimal totalFaturamento = projetos
                .Where(p => p.Orcamento != null)
                .Sum(p => p.Orcamento.valor_final);

            decimal totalLucro = projetos
                .Where(p => p.Orcamento != null)
                .Sum(p => p.Orcamento.valor_margem);

            QuantidadeProjetos = projetos.Count;

            TotalFinanceiro = totalFaturamento.ToString("C2", _ptBR);

            TotalLucro = totalLucro.ToString("C2", _ptBR);
        }

        private void AtualizarGraficoStatus(List<Projeto> projetos)
        {
            var agrupado = projetos
                .GroupBy(p => p.status ?? "Sem Status")
                .Select(g => new
                {
                    Nome = g.Key,
                    Quantidade = g.Count()
                })
                .ToList();

            SeriesStatus.Clear();

            foreach (var item in agrupado)
            {
                SeriesStatus.Add(new PieSeries
                {
                    Title = item.Nome,
                    Values = new ChartValues<double> { item.Quantidade },
                    DataLabels = true
                });
            }

            OnPropertyChanged(nameof(SeriesStatus));
        }

        private void AtualizarGraficoTipo(List<Projeto> projetos)
        {
            var agrupado = projetos
                .GroupBy(p => p.tipo ?? "Sem Tipo")
                .Select(g => new
                {
                    Nome = g.Key,
                    Quantidade = g.Count()
                })
                .ToList();

            SeriesTipo.Clear();

            foreach (var item in agrupado)
            {
                SeriesTipo.Add(new PieSeries
                {
                    Title = item.Nome,
                    Values = new ChartValues<double> { item.Quantidade },
                    DataLabels = true
                });
            }

            OnPropertyChanged(nameof(SeriesTipo));
        }

        private void AtualizarGraficoMensal(List<Projeto> projetos)
        {
            if (projetos == null || projetos.Count == 0)
            {
                ValoresLucroMensal = new ChartValues<double>();

                ValoresFaturamentoMensal = new ChartValues<double>();
                LabelsMeses = Array.Empty<string>();

                OnPropertyChanged(nameof(ValoresLucroMensal));
                OnPropertyChanged(nameof(ValoresFaturamentoMensal));
                OnPropertyChanged(nameof(LabelsMeses));

                return;
            }

            var agrupadoPorMes = projetos
                .Where(p =>
                    p != null &&
                    p.Orcamento != null)
                .GroupBy(p => new
                {
                    Ano = p.data_criacao.Year,
                    Mes = p.data_criacao.Month
                })
                .OrderBy(g => g.Key.Ano)
                .ThenBy(g => g.Key.Mes)
                .Select(g => new
                {
                    Mes = new DateTime(g.Key.Ano, g.Key.Mes, 1),
                    TotalLucro = g.Sum(p => p.Orcamento?.valor_margem ?? 0),
                    TotalFaturamento = g.Sum(p => p.Orcamento?.valor_final ?? 0)
                })
                .ToList();

            ValoresLucroMensal = new ChartValues<double>(
                agrupadoPorMes.Select(x => (double)x.TotalLucro)
            );

            ValoresFaturamentoMensal = new ChartValues<double>(
                agrupadoPorMes.Select(x => (double)x.TotalFaturamento)
            );

            LabelsMeses = agrupadoPorMes
                .Select(x => x.Mes.ToString("MMM/yy"))
                .ToArray();

            OnPropertyChanged(nameof(ValoresLucroMensal));
            OnPropertyChanged(nameof(ValoresFaturamentoMensal));
            OnPropertyChanged(nameof(LabelsMeses));
        }

        #endregion
    }
}