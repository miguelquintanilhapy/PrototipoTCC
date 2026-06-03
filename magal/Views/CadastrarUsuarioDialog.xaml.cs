using magal.Data.Repositories;
using magal.Models;
using System;
using System.Net.Mail;
using System.Windows;
using System.Text.RegularExpressions;

namespace magal.Views
{
    public partial class CadastrarUsuarioDialog : Window
    {
        public CadastrarUsuarioDialog()
        {
            InitializeComponent();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica se os campos não estão em branco 
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                string.IsNullOrWhiteSpace(TxtSenha.Password) ||
                string.IsNullOrWhiteSpace(TxtConfirmarSenha.Password))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validação do formato do e-mail via Regex
            if (!ValidarEmail(TxtEmail.Text))
            {
                MessageBox.Show("Por favor, insira um e-mail válido (exemplo@dominio.com).", "E-mail Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verifica se as senhas são iguais
            if (TxtSenha.Password != TxtConfirmarSenha.Password)
            {
                MessageBox.Show("As senhas digitadas não coincidem. Por favor, verifique.", "Senhas Diferentes", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var novoUsuario = new Usuario
                {
                    nome = TxtNome.Text,
                    email = TxtEmail.Text,
                    senha = TxtSenha.Password,
                    status = "Ativo"
                };

                var repo = new UsuarioRepository();
                repo.Salvar(novoUsuario);

                MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método auxiliar para checar se o formato do e-mail é aceitável
        /// </summary>
        private bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string modeloRegex = @"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, modeloRegex);
        }
    }
}