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
using magal.Models;
using magal.Data.Repositories;


namespace magal.Views
{
    public partial class CadastrarCargoDialog : Window
    {
        public CadastrarCargoDialog()
        {
            InitializeComponent();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                string.IsNullOrWhiteSpace(TxtCustoHora.Text))
            {
                MessageBox.Show(
                    "Preencha todos os campos obrigatórios.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!decimal.TryParse(TxtCustoHora.Text, out decimal custoHora))
            {
                MessageBox.Show(
                    "Valor de custo inválido.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                var cargo = new Cargo
                {
                    nome = TxtNome.Text.Trim(),
                    
                    custo_medio_hora = custoHora,
                    
                };

                var repo = new CargoRepository();
                repo.Inserir(cargo);

                MessageBox.Show(
                    "Cargo cadastrado com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar cargo:\n\n" + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}