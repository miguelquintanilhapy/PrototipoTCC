using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.ComponentModel; // Necessário para ICollectionView
using System.Windows.Data;    // Necessário para CollectionViewSource
using magal.Models;
using magal.Data.Repositories;

namespace magal.ViewModels
{
    public class HistoricoViewModel : BaseModel
    {
        private readonly ProjetoRepository _repository;
        private Projeto _projetoSelecionado;
        private string _filtroTexto;

        // Propriedades para o Dashboard
        private string _totalFinanceiro;
        public string TotalFinanceiro
        {
            get => _totalFinanceiro;
            set { _totalFinanceiro = value; OnPropertyChanged(); }
        }

        private int _quantidadeProjetos;
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
                // Opcional: Atualizar indicadores com base no que está filtrado
                // AtualizarIndicadores(); 
            }
        }

        public Projeto ProjetoSelecionado
        {
            get => _projetoSelecionado;
            set { _projetoSelecionado = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Projeto> Projetos { get; } = new ObservableCollection<Projeto>();

        public ICollectionView ProjetosView { get; set; }

        public RelayCommand ExcluirCommand { get; }
        public RelayCommand EditarCommand { get; }
        public RelayCommand AtualizarCommand { get; }

        public HistoricoViewModel()
        {
            _repository = new ProjetoRepository();

            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as Projeto));
            EditarCommand = new RelayCommand(p => ExecutarEdicao(p as Projeto));
            AtualizarCommand = new RelayCommand(_ => CarregarHistorico());

            CarregarHistorico();
        }

        private void CarregarHistorico()
        {
            try
            {
                var lista = _repository.BuscarTodosPorUsuario(1);

                Projetos.Clear();
                foreach (var p in lista)
                {
                    Projetos.Add(p);
                }

                ProjetosView = CollectionViewSource.GetDefaultView(Projetos);

                ProjetosView.Filter = (obj) =>
                {
                    if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;

                    var projeto = obj as Projeto;
                    var busca = FiltroTexto.ToLower().Trim();

                    bool nomeProjetoOk = projeto.nome?.ToLower().Contains(busca) ?? false;
                    bool nomeClienteOk = projeto.Cliente?.nome?.ToLower().Contains(busca) ?? false;
                    bool statusOk = projeto.status?.ToLower().Contains(busca) ?? false;

                    return nomeProjetoOk || nomeClienteOk || statusOk;
                };

                // Atualiza os cards do topo
                AtualizarIndicadores();

                OnPropertyChanged(nameof(ProjetosView));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar histórico: " + ex.Message);
            }
        }

        private void AtualizarIndicadores()
        {
            QuantidadeProjetos = Projetos.Count;

            // Verifique se o valor_total realmente existe nos itens da lista
            decimal soma = 0;
            foreach (var p in Projetos)
            {
                // Se p.Orcamento for null, o valor não será somado
                if (p.Orcamento != null)
                {
                    soma += p.Orcamento.valor_final;
                }
            }

            TotalFinanceiro = soma.ToString("C2");
        }

        private void ExecutarExclusao(Projeto projeto)
        {
            if (projeto == null) return;

            var msg = $"Tem certeza que deseja excluir o projeto '{projeto.nome}'?\nEsta ação não pode ser desfeita.";
            if (MessageBox.Show(msg, "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.ExcluirProjeto(projeto.id_projeto);
                    Projetos.Remove(projeto);

                    // Recalcula os indicadores após a remoção
                    AtualizarIndicadores();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir: " + ex.Message);
                }
            }
        }

        private void ExecutarEdicao(Projeto projeto)
        {
            if (projeto == null) return;

            var mainWindow = Application.Current.MainWindow as magal.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.IrParaEdicao(projeto);
            }
        }
    }
}