using magal.Models;
using magal.ViewModels;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace magal.Views
{
    public partial class OrcamentoView : UserControl
    {
        public OrcamentoViewModel ViewModel => this.DataContext as OrcamentoViewModel;

        #region Construtores

        public OrcamentoView()
        {
            InitializeComponent();

            var vm = new OrcamentoViewModel();
            this.DataContext = vm;

            vm.PropertyChanged += ViewModel_PropertyChanged;
        }

        public OrcamentoView(Projeto projetoParaEditar)
        {
            InitializeComponent();

            if (LoadingOverlay != null)
            {
                LoadingOverlay.Visibility = Visibility.Visible;
            }

            var vm = new OrcamentoViewModel();
            this.DataContext = vm;

            vm.PropertyChanged += ViewModel_PropertyChanged;
            vm.IsLoading = true;
            this.Loaded += (s, e) =>
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new System.Action(() =>
                {
                    vm.CarregarProjetoParaEdicao(projetoParaEditar);
                }));
            };
        }

        #endregion

        #region Controle do Carregamento (Idêntico ao Histórico)

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrcamentoViewModel.IsLoading) && sender is OrcamentoViewModel vm)
            {
                Dispatcher.Invoke(() =>
                {
                    if (vm.IsLoading)
                    {
                        if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed;
                    }
                });
            }
        }

        #endregion

        #region Validações

        private void ValidarEntradaSemNegativo(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "-")
            {
                MessageBox.Show("Não é permitido valores negativos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
                return;
            }

            Regex regex = new Regex("[^0-9,]+");
            bool temCaractereInvalido = regex.IsMatch(e.Text);

            if (temCaractereInvalido)
            {
                MessageBox.Show("Este campo aceita apenas números positivos.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Information);
                e.Handled = true;
            }
        }

        #endregion
    }
}