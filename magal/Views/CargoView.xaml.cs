using System;
using System.Collections.Generic;
using System.Linq; // Adicionado para permitir localização da MainWindow na aplicação
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
            // Instancia e vincula a ViewModel ao contexto de dados da View
            this.DataContext = new CargoViewModel();
        }

        /// <summary>
        /// Lógica replicada do botão voltar. 
        /// Localiza a MainWindow ativa e comanda o retorno à tela anterior.
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