using magal.Data.Repositories;
using magal.Models;
using magal.ViewModels;
using magal.Views;
using System.Windows;

namespace magal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;

            AbrirOrcamento();
        }

        // Evento do botão "Novo Orçamento"
        private void BtnOrcamentos_Click(object sender, RoutedEventArgs e)
        {
            AbrirOrcamento();
        }


        private void BtnHistorico_Click(object sender, RoutedEventArgs e)
        {
            AbrirHistorico();
        }

        private void AbrirOrcamento()
        {

            MainContent.Content = new OrcamentoView();
        }

        private void AbrirHistorico()
        {
            MainContent.Content = new HistoricoView();
        }

        public void IrParaEdicao(Projeto projetoSimplificado)
        {
            var repo = new ProjetoRepository();
            // Busca tudo do banco (tarefas, custos, orçamento) usando o ID
            Projeto projetoCompleto = repo.CarregarProjetoCompleto(projetoSimplificado.id_projeto);

            var viewModel = new OrcamentoViewModel();


            // Passa o projeto completo para a VM
            viewModel.CarregarProjetoParaEdicao(projetoCompleto);

            // Troca de tela
            var view = new OrcamentoView();
            view.DataContext = viewModel;
            MainContent.Content = view;
        }
    }
}