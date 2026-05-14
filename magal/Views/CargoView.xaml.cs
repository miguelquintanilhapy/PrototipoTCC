using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using magal.ViewModels;

namespace magal.Views
{
    public partial class CargoView : UserControl
    {
        public CargoView()
        {
            InitializeComponent();
            DataContext = new CargoViewModel();
        }

        private void BtnNovoCargo_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CadastrarCargoDialog();
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                DataContext = new CargoViewModel();
            }
        }
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == null)
                return;

            var cargo = button.DataContext as magal.Models.Cargo;

            if (cargo == null)
                return;

            var dialog = new EditarCargoDialog(cargo);

            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                DataContext = new CargoViewModel();
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == null)
                return;

            var cargo = button.DataContext as magal.Models.Cargo;

            if (cargo == null)
                return;

            var resultado = MessageBox.Show(
                $"Deseja realmente excluir o cargo '{cargo.nome}'?",
                "Aero Concepts",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                var repo = new magal.Data.Repositories.CargoRepository();

                repo.Excluir(cargo.id_cargo);

                MessageBox.Show(
                    "Cargo excluído com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DataContext = new CargoViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao excluir: " + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}