using System;
using System.Collections.Generic;
using System.Linq; 
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
using magal.Data.Repositories;

namespace magal.Views
{
    public partial class FuncionarioView : UserControl
    {
        public FuncionarioView()
        {
            InitializeComponent();
            this.DataContext = new magal.ViewModels.FuncionarioViewModel();
        }

        /// <summary>
        /// Trata o evento de clique do botão voltar.
        /// </summary>
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {
                mainWindow.AbrirGerenciamento();
            }
        }
    }
}