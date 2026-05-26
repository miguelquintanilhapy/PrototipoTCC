using System;
using System.Windows;
using System.Windows.Controls;
using magal.Data.Repositories;
using magal.Models;

namespace magal.Views
{
    public partial class EditarClienteDialog : Window
    {
        private readonly Cliente _cliente;

        public EditarClienteDialog(Cliente cliente)
        {
            InitializeComponent();

            _cliente = cliente;

            PreencherCampos();
        }

        private void PreencherCampos()
        {
            TxtNome.Text = _cliente.nome;
            TxtCpfCnpj.Text = _cliente.cpf_cnpj;
            TxtCidade.Text = _cliente.cidade;
            TxtEstado.Text = _cliente.estado;
            TxtContato.Text = _cliente.contato;

            // TIPO
            foreach (ComboBoxItem item in ComboTipo.Items)
            {
                if (item.Content.ToString() == _cliente.tipo)
                {
                    ComboTipo.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                ComboTipo.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtCpfCnpj.Text) ||
                string.IsNullOrWhiteSpace(TxtCidade.Text) ||
                string.IsNullOrWhiteSpace(TxtEstado.Text) ||
                string.IsNullOrWhiteSpace(TxtContato.Text))
            {
                MessageBox.Show(
                    "Preencha todos os campos.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                _cliente.nome = TxtNome.Text.Trim();

                _cliente.tipo = ((ComboBoxItem)ComboTipo.SelectedItem)
                    .Content
                    .ToString();

                _cliente.cpf_cnpj = TxtCpfCnpj.Text.Trim();
                _cliente.cidade = TxtCidade.Text.Trim();
                _cliente.estado = TxtEstado.Text.Trim().ToUpper();
                _cliente.contato = TxtContato.Text.Trim();

                var repo = new ClienteRepository();
                repo.Atualizar(_cliente);

                MessageBox.Show(
                    "Cliente atualizado com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar: " + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}