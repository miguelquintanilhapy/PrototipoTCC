using System;
using System.Windows;
using System.Windows.Controls;
using magal.ViewModels;

namespace magal.Views
{
    public partial class ClienteView : UserControl
    {
        public ClienteView()
        {
            InitializeComponent();
            DataContext = new ClienteViewModel();
        }

        /// <summary>
        /// Manipulador do evento de clique no botão Voltar.
        /// Direciona o aplicativo de volta para a tela/painel principal na MainWindow.
        /// </summary>
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as magal.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.AbrirGerenciamento();
            }
        }
    }
}