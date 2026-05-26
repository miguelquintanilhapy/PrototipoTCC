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
        public CustoView()
        {
            InitializeComponent();
            DataContext = new CustoViewModel();
        }

        private void BtnNovoCusto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CadastrarCustoDialog();
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                // Recarrega o ViewModel para atualizar a listagem na tela
                DataContext = new CustoViewModel();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            var custo = button.DataContext as Custo;
            if (custo == null)
                return;

            var dialog = new EditarCustoDialog(custo);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                // Recarrega o ViewModel para refletir as alterações
                DataContext = new CustoViewModel();
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            var custo = button.DataContext as Custo;
            if (custo == null)
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
                var repo = new CustoRepository();

                // Adapte para a propriedade exata do ID na sua model de Custo (ex: id_custo)
                repo.Excluir(custo.id_custo);

                MessageBox.Show(
                    "Custo excluído com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Atualiza a grade após a exclusão
                DataContext = new CustoViewModel();
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