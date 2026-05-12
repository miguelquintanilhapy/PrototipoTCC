using System.Windows;
using System.Windows.Controls;
using magal.Data.Repositories;
using magal.Models;
using magal.ViewModels;
using magal.Views;

namespace magal
{
    public partial class MainWindow : Window
    {
        private HomeView _homeView;
        private HistoricoView _historicoView;
        private OrcamentoView _orcamentoView;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            AbrirHome();
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            AbrirHome();
        }

        private void BtnOrcamentos_Click(object sender, RoutedEventArgs e)
        {
            AbrirOrcamento();
        }

        private void BtnHistorico_Click(object sender, RoutedEventArgs e)
        {
            AbrirHistorico();
        }

        private void BtnGerenciamento_Click(object sender, RoutedEventArgs e)
        {
            AbrirGerenciamento();
        }

        public void AbrirHome()
        {
            if (_homeView == null)
                _homeView = new HomeView();

            MainContent.Content = _homeView;
        }

        public void AbrirOrcamento()
        {
            if (_orcamentoView == null)
                _orcamentoView = new OrcamentoView();

            MainContent.Content = _orcamentoView;
        }

        public void AbrirHistorico()
        {
            if (_historicoView == null)
                _historicoView = new HistoricoView();

            MainContent.Content = _historicoView;
        }

        public void AbrirGerenciamento()
        {
            MainContent.Content = new GerenciamentoView();
        }

        public ContentControl MainContentControl
        {
            get { return MainContent; }
        }

        public void IrParaEdicao(Projeto projetoSimplificado)
        {
            var repo = new ProjetoRepository();
            Projeto projetoCompleto = repo.CarregarProjetoCompleto(projetoSimplificado.id_projeto);

            var viewModel = new OrcamentoViewModel();
            viewModel.CarregarProjetoParaEdicao(projetoCompleto);

            var view = new OrcamentoView();
            view.DataContext = viewModel;
            MainContent.Content = view;
        }
    }
}