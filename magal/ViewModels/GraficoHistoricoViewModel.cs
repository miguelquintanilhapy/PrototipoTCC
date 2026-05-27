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

        private string _periodoSelecionado;

        public List<string> Periodos { get; set; } = new ()
            {
                "7 Dias",
                "30 Dias",
                "3 Meses",
                "6 Meses",
                "1 Ano",
                "Período completo"
            };

        public string PeriodoSelecionado
        {
            get => _periodoSelecionado;
            set
            {
                _periodoSelecionado = value;
                OnPropertyChanged();

                CarregarDados();
            }
        }
        public GraficoHistoricoViewModel()
        {
            _repository = new ProjetoRepository();

            AtualizarCommand = new RelayCommand(_ => CarregarDados());

            Formatter = value => value.ToString("C0", _ptBR);
            
            PeriodoSelecionado = "6 Meses";

            CarregarDados();
        }

        #endregion

        #region Métodos

        public void CarregarDados()
        {
            try
            {
                var projetos = _repository.BuscarTodosPorUsuario(1);

                DateTime dataLimite = DateTime.MinValue;

                switch (PeriodoSelecionado)
                {
                    case "7 Dias":
                        dataLimite = DateTime.Now.AddDays(-7);
                        break;

                    case "30 Dias":
                        dataLimite = DateTime.Now.AddDays(-30);
                        break;

                    case "3 Meses":
                        dataLimite = DateTime.Now.AddMonths(-3);
                        break;

                    case "6 Meses":
                        dataLimite = DateTime.Now.AddMonths(-6);
                        break;

                    case "1 Ano":
                        dataLimite = DateTime.Now.AddYears(-1);
                        break;

                    case "Período completo":
                        dataLimite = DateTime.MinValue;
                        break;
                }

                projetos = projetos
                    .Where(p => p.data_criacao >= dataLimite)
                    .ToList();

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

            SeriesStatus = new SeriesCollection();

            // Paleta mais elegante/coerente com o sistema
            string[] cores =
            {
                "#1E3A8A", // azul forte
                "#0F766E", // verde petróleo
                "#7C3AED", // roxo elegante
                "#EA580C", // laranja queimado
                "#BE123C", // vinho
                "#0891B2", // ciano escuro
                "#4D7C0F", // verde oliva
                "#475569"  // cinza slate
            };

            int index = 0;

            foreach (var item in agrupado)
            {
                var cor = (System.Windows.Media.Color)
                    System.Windows.Media.ColorConverter.ConvertFromString(
                        cores[index % cores.Length]);

                SeriesStatus.Add(new PieSeries
                {
                    Title = item.Nome,
                    Values = new ChartValues<double> { item.Quantidade },
                    DataLabels = true,

                    Fill = new System.Windows.Media.SolidColorBrush(cor),

                    Foreground = System.Windows.Media.Brushes.White,

                    StrokeThickness = 2,
                    Stroke = System.Windows.Media.Brushes.White
                });

                index++;
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

            SeriesTipo = new SeriesCollection();

            string[] cores =
            {
                "#1E3A8A", // azul forte
                "#0F766E", // verde petróleo
                "#7C3AED", // roxo elegante
                "#EA580C", // laranja queimado
                "#BE123C", // vinho
                "#0891B2", // ciano escuro
                "#4D7C0F", // verde oliva
                "#475569"  // cinza slate
            };

            int index = 0;

            foreach (var item in agrupado)
            {
                var cor = (System.Windows.Media.Color)
                    System.Windows.Media.ColorConverter.ConvertFromString(
                        cores[index % cores.Length]);

                SeriesTipo.Add(new PieSeries
                {
                    Title = item.Nome,
                    Values = new ChartValues<double> { item.Quantidade },
                    DataLabels = true,

                    Fill = new System.Windows.Media.SolidColorBrush(cor),

                    Foreground = System.Windows.Media.Brushes.White,

                    StrokeThickness = 2,
                    Stroke = System.Windows.Media.Brushes.White
                });

                index++;
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

            bool agruparPorDia =
                PeriodoSelecionado == "7 Dias" ||
                PeriodoSelecionado == "30 Dias";

            if (agruparPorDia)
            {
                var agrupadoPorDia = projetos
                    .Where(p => p != null && p.Orcamento != null)
                    .GroupBy(p => p.data_criacao.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        Data = g.Key,
                        TotalLucro = g.Sum(p => p.Orcamento?.valor_margem ?? 0),
                        TotalFaturamento = g.Sum(p => p.Orcamento?.valor_final ?? 0)
                    })
                    .ToList();

                ValoresLucroMensal = new ChartValues<double>(
                    agrupadoPorDia.Select(x => (double)x.TotalLucro)
                );

                ValoresFaturamentoMensal = new ChartValues<double>(
                    agrupadoPorDia.Select(x => (double)x.TotalFaturamento)
                );

                LabelsMeses = agrupadoPorDia
                    .Select(x => x.Data.ToString("dd/MM"))
                    .ToArray();
            }
            else
            {
                var agrupadoPorMes = projetos
                    .Where(p => p != null && p.Orcamento != null)
                    .GroupBy(p => new
                    {
                        Ano = p.data_criacao.Year,
                        Mes = p.data_criacao.Month
                    })
                    .OrderBy(g => g.Key.Ano)
                    .ThenBy(g => g.Key.Mes)
                    .Select(g => new
                    {
                        Data = new DateTime(g.Key.Ano, g.Key.Mes, 1),
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
                    .Select(x => x.Data.ToString("MMM/yy"))
                    .ToArray();
            }

            OnPropertyChanged(nameof(ValoresLucroMensal));
            OnPropertyChanged(nameof(ValoresFaturamentoMensal));
            OnPropertyChanged(nameof(LabelsMeses));
        }

        #endregion
    }
}