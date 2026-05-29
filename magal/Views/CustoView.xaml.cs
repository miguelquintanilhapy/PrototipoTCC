using System;
using System.Windows;
using System.Windows.Controls;
using magal.ViewModels;

namespace magal.Views
{
    public partial class CustoView : UserControl
    {
        public CustoView()
        {
            InitializeComponent();
            DataContext = new CustoViewModel();
        }

        /// <summary>
        /// Executa o retorno de navegação limpando o painel de visualização atual 
        /// e trazendo o usuário de volta ao painel principal ou Home da aplicação.
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