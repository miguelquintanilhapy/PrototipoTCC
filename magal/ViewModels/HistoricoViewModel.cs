using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;
using magal.Models;
using magal.Data.Repositories;

namespace magal.ViewModels
{
    public class HistoricoViewModel : BaseModel
    {
        private readonly ProjetoRepository _repository;
        private Projeto _projetoSelecionado;
        private string _filtroTexto;
        private string _totalFinanceiro;
        private string _totalLucro;
        private int _quantidadeProjetos;

        public string TotalFinanceiro
        {
            get => _totalFinanceiro;
            set { _totalFinanceiro = value; OnPropertyChanged(); }
        }

        public string TotalLucro
        {
            get => _totalLucro;
            set { _totalLucro = value; OnPropertyChanged(); }
        }

        public int QuantidadeProjetos
        {
            get => _quantidadeProjetos;
            set { _quantidadeProjetos = value; OnPropertyChanged(); }
        }

        public string FiltroTexto
        {
            get => _filtroTexto;
            set
            {
                _filtroTexto = value;
                OnPropertyChanged();
                ProjetosView?.Refresh();
                // Recalcula indicadores baseados no que está visível (filtrado)
                AtualizarIndicadores();
            }
        }

        public Projeto ProjetoSelecionado
        {
            get => _projetoSelecionado;
            set { _projetoSelecionado = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Projeto> Projetos { get; } = new ObservableCollection<Projeto>();
        public ICollectionView ProjetosView { get; private set; }

        public RelayCommand ExcluirCommand { get; }
        public RelayCommand EditarCommand { get; }
        public RelayCommand AtualizarCommand { get; }

        public HistoricoViewModel()
        {
            _repository = new ProjetoRepository();

            ProjetosView = CollectionViewSource.GetDefaultView(Projetos);
            ProjetosView.Filter = FiltroDeProjetos;

            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as Projeto));
            EditarCommand = new RelayCommand(p => ExecutarEdicao(p as Projeto));
            AtualizarCommand = new RelayCommand(_ => CarregarHistorico());

            CarregarHistorico();
        }

        private void CarregarHistorico()
        {
            try
            {
                // busca os projetos do usuário logado 
                var lista = _repository.BuscarTodosPorUsuario(1);

                Projetos.Clear();

                foreach (var p in lista)
                {
                    Projetos.Add(p);
                }

                AtualizarIndicadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar histórico: " + ex.Message);
            }
        }

        private bool FiltroDeProjetos(object obj)
        {
            if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;
            var projeto = obj as Projeto;
            if (projeto == null) return false;

            var busca = FiltroTexto.ToLower().Trim();

            return (projeto.nome?.ToLower().Contains(busca) ?? false) ||
                   (projeto.status?.ToLower().Contains(busca) ?? false) ||
                   (projeto.tipo?.ToLower().Contains(busca) ?? false) ||
                   (projeto.Cliente?.nome?.ToLower().Contains(busca) ?? false);
        }

        private void AtualizarIndicadores()
        {
            // filtra apenas os projetos que passam pelo filtro de busca da tela
            var projetosVisiveis = ProjetosView.Cast<Projeto>().ToList();

            decimal somaFaturamento = projetosVisiveis
                .Where(p => p.Orcamento != null)
                .Sum(p => p.Orcamento.valor_final);

            decimal somaLucro = projetosVisiveis
                .Where(p => p.Orcamento != null)
                .Sum(p => p.Orcamento.valor_margem);

            QuantidadeProjetos = projetosVisiveis.Count;

            var cultura = new System.Globalization.CultureInfo("pt-BR");
            TotalFinanceiro = somaFaturamento.ToString("C2", cultura);
            TotalLucro = somaLucro.ToString("C2", cultura);

            System.Diagnostics.Debug.WriteLine($"Histórico Atualizado: Faturamento {TotalFinanceiro}, Lucro {TotalLucro}");
        }

        private void ExecutarExclusao(Projeto projeto)
        {
            if (projeto == null) return;

            var result = MessageBox.Show($"Deseja realmente excluir o projeto '{projeto.nome}'?",
                                       "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.ExcluirProjeto(projeto.id_projeto);
                    Projetos.Remove(projeto);
                    AtualizarIndicadores();
                }
                catch (Exception ex) { MessageBox.Show("Erro ao excluir: " + ex.Message); }
            }
        }

        private void ExecutarEdicao(Projeto projeto)
        {
            if (projeto == null) return;

            try
            {
                var projetoCompleto = _repository.CarregarProjetoCompleto(projeto.id_projeto);

                if (projetoCompleto != null)
                {
                    var mainWindow = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();

                    mainWindow?.IrParaEdicao(projetoCompleto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados para edição: " + ex.Message);
            }
        }
    }
}