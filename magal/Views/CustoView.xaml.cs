using System;
using System.Windows;
using System.Windows.Controls;
using magal.ViewModels;
using magal.Models;
using magal.Data.Repositories;

namespace magal.Views
{
    public partial class CustoView : UserControl
    {
        // Mantém uma referência fixa ao ViewModel da tela
        private readonly CustoViewModel _viewModel;

        public CustoView()
        {
            InitializeComponent();

            // Instancia o ViewModel apenas uma vez na inicialização
            _viewModel = new CustoViewModel();
            DataContext = _viewModel;
        }

        private void BtnNovoCusto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CadastrarCustoDialog();
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                _viewModel.CarregarCustos();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.DataContext is not CatalogoCusto custo)
                return;

            var dialog = new EditarCustoDialog(custo);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                // Atualiza a listagem de forma limpa
                _viewModel.CarregarCustos();
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.DataContext is not CatalogoCusto custo)
                return;

            var resultado = MessageBox.Show(
                $"Deseja realmente excluir o custo '{custo.nome}'?",
                "Aero Concepts",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                var repo = new CatalogoCustoRepository();
                repo.Excluir(custo.id_catalogo_custo);

                MessageBox.Show(
                    "Custo excluído com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Atualiza a grade reaproveitando a instância
                _viewModel.CarregarCustos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao excluir custo: " + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}