using System;
using System.Windows;
using System.Windows.Controls;
using magal.ViewModels; 

namespace magal.Views
{
    public partial class GerenciamentoView : UserControl
    {
        private readonly GerenciamentoViewModel _viewModel;

        public GerenciamentoView()
        {
            InitializeComponent();

            _viewModel = new GerenciamentoViewModel();
            this.DataContext = _viewModel;
        }

      
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.CarregarIndicadores();
        }

        private void BtnFuncionarios_Click(object sender, RoutedEventArgs e)
        {
            var janelaPrincipal = Window.GetWindow(this) as MainWindow;
            if (janelaPrincipal != null)
            {
                janelaPrincipal.MainContentControl.Content = new FuncionarioView();
            }
        }

        private void BtnCargos_Click(object sender, RoutedEventArgs e)
        {
            var janelaPrincipal = Window.GetWindow(this) as MainWindow;
            if (janelaPrincipal != null)
            {
                janelaPrincipal.MainContentControl.Content = new CargoView();
            }
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}